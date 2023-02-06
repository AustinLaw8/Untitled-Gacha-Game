using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Accuracy
{
    Perfect, Great, Good, Bad, Miss
}

[RequireComponent(typeof(Collider2D))]
public class Lane : MonoBehaviour
{
    // Queue containing the notes that are in the "hittable" range
    private Queue<GameObject> notes;

    void Start()
    {
        // FIXME: move the queries hit triggers to some other game manager
        Physics2D.queriesHitTriggers = true;
        notes = new Queue<GameObject>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{other.gameObject.name} reaching {this.gameObject.name}");
        notes.Enqueue(other.gameObject);
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        /**
         * Note by austin
         *  Theoretically, `notes` can never be empty when OnTriggerExit is called. There are two scenarios for dequeing `notes`:
         *  1) Player pressed it. In this case, then, the note should be destroyed, and OnTriggerExit should not be called since the object never left the trigger (cant leave the trigger if its destroyed)
         *  2) Player missed it. Then, `notes` never got dequeued since the original enqueue so `notes` cannot be empty
         */
        if(notes.Count == 0)
        {
            Debug.LogWarning("not sure how this is getting called, but something odd is happening...");
            Debug.LogWarning(other.gameObject.name);
            return;
        }
        if (other.gameObject != notes.Dequeue())
        {
            Debug.LogError($"Object leaving {this.gameObject.name} is not the same as object removed from queue!! This means something probably went wrong or got desynced! If this is happening often, it means the code is buggy and probably should be rethought.");
        }
        else
        {
            Debug.Log($"{other.gameObject.name} leaving {this.gameObject.name}");
        }
        Destroy(other.gameObject);
        DamageEvent evt = Events.DamageEvent;
        evt.amount = HPBar.MISS_NOTE_AMOUNT;
        EventManager.Broadcast(evt);
    }

    /**
     * Leverages the Unity builtin OnMouseDown to handle tap notes
     * Since tap notes only require a single tap, we can use OnMouseDown to track that happening.
     */
    void OnMouseDown()
    {
        Note note = PeekFirstNote();
        if (note && note.GetType() == typeof(Note))
        {
            OnNotePressed(note);
        }
        else
        {
            Debug.Log($"No notes detected {this.gameObject.name}");
        }
    }

    /**
     * TODO: Unsure about this yet, but this will probably handle flick and hold notes
     */
    void OnMouseDrag()
    {

        // foreach(Touch touch in Input.touches)
        // {
        // if (touch.phase == iPhoneTouchPhase.Moved) {
        //     if (touch.deltaPosition.y > 50) {
        //     //Do Something with upward flick
        //     }
        // }
        // }
    }

    /* Helpers */

    /**
     * Although detection for each note type has to be unique, once we know a note has been pressed,
     * the score, health, and combo calcs should be the same irrespective of the note type
     * ... although that isn't necessarily set in stone.
     */
    private void OnNotePressed(Note note)
    {
        // Note by austin... i feel like this line should be here... needs more testing...
        // notes.Dequeue();

        Debug.Log($"Note pressed at {this.gameObject.name}");

        // TODO: Score, health, and combo calcs
        // this is all justin here, probably
        Accuracy accuracy = GetAccuracy(note);
        float accuracyMultiplier = 0;
        switch (accuracy)
        { 
            case Accuracy.Perfect:
                accuracyMultiplier = 5; // TODO placeholder
                break;
            case Accuracy.Great:
                accuracyMultiplier = 4; // TODO
                break;
            case Accuracy.Good:
                accuracyMultiplier = 3;
                break;
            case Accuracy.Bad:
                accuracyMultiplier = 2;
                Debug.Log("Score, health, and combo calculation calcs in Lane.cs are placeholdered; please define functionality.");
                break;
                case Accuracy.Miss:
                    ScoreManager.Instance.ResetCombo();
            break;
            default:
                    Debug.LogWarning("Invalid switch path taken, this should never be called...");
            break;
        }
        // Tiffany: placeholder - can decide which accuracy breaks combo later
        ScoreManager.Instance.IncCombo();


        // Probable pseudocode

        int baseScore = note.GetScoreValue();

        // TODO: get combo from... ScoreManager
        int curCombo = ScoreManager.Instance.GetCombo();

        // TODO: define some sort of mapping from curCombo -> multiplier
        float comboMultiplier = ScoreManager.Instance.GetComboMultiplier(curCombo);

        // TODO: define a mapping from accuracy -> multiplier
        //float accuracyMultiplier = accuracyMultiplierMap[accuracy];

        // TODO: conduct full score calcs
        int deltaScore = (int)Mathf.Ceil(baseScore * comboMultiplier * accuracyMultiplier);

        // TODO: tell a score manager or some other similar manager to increase the score
        ScoreManager.Instance.IncScore(deltaScore);
        Debug.Log(ScoreManager.Instance.GetScore());

        Debug.Log($"Destroying {note.gameObject.name}");
    Destroy(note.gameObject);
}


    // Wrapper around Queue.Peek() to conduct validation
    // Removes and returns "first"/"lowest" note in lane if it exists, otherwise null
    private Note PeekFirstNote()
    { 
        if (notes.Count == 0) return null;
        else return notes.Peek().GetComponent<Note>();
    }

    // TODO: Calculate accuracy based on distance from fall line
    private Accuracy GetAccuracy(Note note)
    {
        Debug.Log("GetAccuracy in Lane.cs is placeholdered; please define functionality.");
        return Accuracy.Great;
    }
}

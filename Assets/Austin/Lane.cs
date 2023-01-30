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
        Debug.Log($"Note reaching {this.gameObject.name}");
        notes.Enqueue(other.gameObject);
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != notes.Dequeue())
        {
            Debug.LogError($"Object leaving {this.gameObject.name} is not the same as object removed from queue!! This means something probably went wrong or got desynced! If this is happening often, it means the code is buggy and probably should be rethought.");
        }
        else
        {
            Debug.Log($"Note reaching {this.gameObject.name}");
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
        notes.Dequeue();
        Debug.Log($"Note pressed at {this.gameObject.name}");
        // TODO: Score, health, and combo calcs
        // this is all justin here, probably
        Accuracy accuracy = GetAccuracy(note);
        switch(accuracy)
        {
            case Accuracy.Perfect:
            case Accuracy.Great:
            case Accuracy.Good:
            case Accuracy.Bad:
                Debug.Log("Score, health, and combo calculation calcs in Lane.cs are placeholdered; please define functionality.");
                /* Probable pseudocode

                int baseScore = note.GetScoreValue();

                // TODO: get combo from... somewhere
                int curCombo = GetCombo(); 

                // TODO: define some sort of mapping from curCombo -> multiplier
                float comboMultiplier = GetComboMultiplier(curCombo); 

                // TODO: define a mapping from accuracy -> multiplier
                float accuracyMultiplier = accuracyMultiplierMap[accuracy];
                
                // TODO: conduct full score calcs
                int deltaScore = Math.Ceiling(baseScore * comboMultiplier * accuracyMultiplier);
                
                // TODO: tell a score manager or some other similar manager to increase the score
                */
                break;
            case Accuracy.Miss:
            default:
                Debug.LogWarning("Invalid switch path taken, this should never be called...");
                break;
        }
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

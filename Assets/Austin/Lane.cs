using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Accuracy
{
    Perfect, Great, Good, Bad, Miss
}

[RequireComponent(typeof(Collider2D))]
public class Lane : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Collider2D col;

    // Queue containing the notes that are in the "hittable" range
    private Queue<Note> notes;

    // For flick notes
    private static float FLICK_THRESHOLD = .1f;
    private Vector2 startPos;
    private int touchId;

    void Start()
    {
        // FIXME: move the queries hit triggers to some other game manager
        Physics2D.queriesHitTriggers = true;
        col = GetComponent<Collider2D>();
        notes = new Queue<Note>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log($"{other.gameObject.name} reaching {this.gameObject.name}");
        notes.Enqueue(other.gameObject.GetComponent<Note>());
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
            // Debug.LogWarning("not sure how this is getting called, but something odd is happening...");
            // Debug.LogWarning(other.gameObject.name);
            return;
        }
        if (other.gameObject != notes.Dequeue())
        {
            // Debug.LogError($"Object leaving {this.gameObject.name} is not the same as object removed from queue!! This means something probably went wrong or got desynced! If this is happening often, it means the code is buggy and probably should be rethought.");
        }
        else
        {
            // Debug.Log($"{other.gameObject.name} leaving {this.gameObject.name}");
        }
        Destroy(other.gameObject);

        // TODO: ima get rid of all this event stuff and just replace it with singletons everywhere
        DamageEvent evt = Events.DamageEvent;
        evt.amount = HPBar.MISS_NOTE_AMOUNT;
        EventManager.Broadcast(evt);
    }

    /**
     * Leverages the Unity builtin OnMouseDown to handle tap notes
     * Since tap notes only require a single tap, we can use OnMouseDown to track that happening.
     */
    public void OnPointerDown(PointerEventData e)
    {
        Note note = PeekFirstNote();
        if (note)
        {
            // Handle tap notes
            if (note.GetType() == typeof(Note))
            {
                OnNotePressed(note);
            }
            else
            {
                // Require flick input for flick note
                // Debug.Log($"Note {note.gameObject.name} detected at {this.gameObject.name}, but wrong input was given");
                startPos = e.pointerCurrentRaycast.worldPosition;
                // foreach(Touch touch in Input.touches)
                // {
                //     float y = Camera.main.ScreenToWorldPoint(touch.position).y;
                //     if(col.bounds.min.y <= y && y <= col.bounds.max.y)
                //     {
                //         startPos = touch.position;
                //         touchId = touch.fingerId;
                //         break;
                //     }
                // }
            }
        }
        else
        {
            // Debug.Log($"No notes detected {this.gameObject.name}");
        }
    }

    public void OnDrag(PointerEventData e)
    {
        Note note = PeekFirstNote();
        if (note)
        {
            // Handle flick notes
            if (note.GetType() == typeof(FlickNote))
            {
                if (Vector3.Distance(e.pointerCurrentRaycast.worldPosition, startPos) > FLICK_THRESHOLD)
                {
                    OnNotePressed(note);
                }
            }
        }
        else
        {
            // Debug.Log($"No notes detected {this.gameObject.name}");
        }

    }

    /* Helpers */

    /**
     * Although detection for each note type has to be unique, once we know a note has been pressed,
     * the score, health, and combo calcs should be the same irrespective of the note type
     * ... although that isn't necessarily set in stone.
     */
    // Gets accuracy, tells ScoreManager to increase the score, and destroys the pressed Note.
    private void OnNotePressed(Note note)
    {
        // Debug.Log($"Note pressed at {this.gameObject.name}");

        Accuracy accuracy = GetAccuracy(note);
        ScoreManager.scoreManager.IncreaseScore(accuracy);

        // Debug.Log($"Destroying {note.gameObject.name}");
        Destroy(note.gameObject);
    }

    // Wrapper around Queue.Peek() to conduct validation
    // Removes and returns "first"/"lowest" note in lane if it exists, otherwise null
    private Note PeekFirstNote()
    { 
        if (notes.Count == 0) return null;
        else return notes.Peek();
    }

    // TODO: Calculate accuracy based on distance from fall line
    private Accuracy GetAccuracy(Note note)
    {
        // Debug.Log("GetAccuracy in Lane.cs is placeholdered; please define functionality.");
        return Accuracy.Great;
    }
}

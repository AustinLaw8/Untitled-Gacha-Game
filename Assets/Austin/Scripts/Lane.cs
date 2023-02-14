using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;

public enum Accuracy
{
    Perfect, Great, Good, Bad, Miss
}

public class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
{
    private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
    private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

    public Map()
    {
        Forward = new Indexer<T1, T2>(_forward);
        Reverse = new Indexer<T2, T1>(_reverse);
    }

    public Indexer<T1, T2> Forward { get; private set; }
    public Indexer<T2, T1> Reverse { get; private set; }

    public void Add(T1 t1, T2 t2)
    {
        _forward.Add(t1, t2);
        _reverse.Add(t2, t1);
    }

    public void Remove(T1 t1)
    {
        T2 revKey = Forward[t1];
        _forward.Remove(t1);
        _reverse.Remove(revKey);
    }
    
    public void Remove(T2 t2)
    {
        T1 forwardKey = Reverse[t2];
        _reverse.Remove(t2);
        _forward.Remove(forwardKey);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return _forward.GetEnumerator();
    }

    public class Indexer<T3, T4>
    {
        private readonly Dictionary<T3, T4> _dictionary;

        public Indexer(Dictionary<T3, T4> dictionary)
        {
            _dictionary = dictionary;
        }

        public T4 this[T3 index]
        {
            get { return _dictionary[index]; }
            set { _dictionary[index] = value; }
        }

        public bool Contains(T3 key)
        {
            return _dictionary.ContainsKey(key);
        }
    }
}

[RequireComponent(typeof(Collider2D))]
public class Lane : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private static List<float> LANE_LOCATIONS = new List<float>(){ -20, 20 };
    private Collider2D col;

    // Queue containing the notes that are in the "hittable" range
    private Dictionary<int, Queue<Note>> notes;

    // For flick notes
    private static float FLICK_THRESHOLD = .1f;
    private Dictionary<int, Vector3> touchStartPositions;

    // For drag notes
    private Dictionary<Note, Vector3> dragNotes;


    void Start()
    {
        // FIXME: move the queries hit triggers to some other game manager
        Physics2D.queriesHitTriggers = true;
        EnhancedTouchSupport.Enable();
        col = GetComponent<Collider2D>();

        // init dicts
        notes = new Dictionary<int, Queue<Note>>();
        touchStartPositions = new Dictionary<int, Vector3>();
        dragNotes = new Dictionary<Note, Vector3>();

        for(int i = 0; i < LANE_LOCATIONS.Count; i++) {
            notes.Add(i, new Queue<Note>());
            touchStartPositions.Add(i + 1, Vector3.zero);
        }
    }
    
    void FixedUpdate()
    {
        foreach(KeyValuePair<Note, Vector3> pair in dragNotes)
        {   
            if(pair.Value != Vector3.zero)
            {

                if(pair.Key.GetComponent<Collider2D>().OverlapPoint(pair.Value))
                {
                    Debug.Log("u hit pog");
                }
                else
                {
                    /*missing the drag*/
                    Debug.Log("u missed");
                }
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        int lane = GetLane(other);
        Note note = other.gameObject.GetComponent<Note>();
        if (note.GetNoteType() == NoteType.Hold)
            dragNotes.Add(note, Vector3.zero);
        notes[lane].Enqueue(note);
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        /**
         * Note by austin
         *  Theoretically, `notes` can never be empty when OnTriggerExit is called. There are two scenarios for dequeing `notes`:
         *  1) Player pressed it. In this case, then, the note should be destroyed, and OnTriggerExit should not be called since the object never left the trigger (cant leave the trigger if its destroyed)
         *  2) Player missed it. Then, `notes` never got dequeued since the original enqueue so `notes` cannot be empty
         */
        if(notes.Count == 0) return;

        if(other.gameObject.GetComponent<Note>().GetNoteType() == NoteType.Hold)
        {
            dragNotes.Remove(other.gameObject.GetComponent<Note>());
        }
        Destroy(other.gameObject);

        // TODO: ima get rid of all this event stuff and just replace it with singletons everywhere
        DamageEvent evt = Events.DamageEvent;
        evt.amount = HPBar.MISS_NOTE_AMOUNT;
        EventManager.Broadcast(evt);
    }

    /**
     * Leverages the Unity builtin OnPointerDown event to handle tap notes
     * Since tap notes only require a single tap, we can use OnPointerDown  to track that happening.
     */
    public void OnPointerDown(PointerEventData e)
    {
        int lane = GetLane(e);
        Note note = PeekFirstInLane(lane);
        if (note)
        {
            switch (note.GetNoteType())
            {
                case NoteType.Tap:
                    OnNotePressed(note);
                    break;
                case NoteType.Flick:
                    touchStartPositions[lane] = e.pointerCurrentRaycast.worldPosition;
                    break;
                case NoteType.Hold:
                    if(dragNotes.ContainsKey(note))
                        dragNotes[note] = e.pointerCurrentRaycast.worldPosition;
                    else
                        dragNotes.Add(note, e.pointerCurrentRaycast.worldPosition);
                    OnNotePressed(note);
                    break;
                default:
                    return;

            }
        }
    }

    public void OnDrag(PointerEventData e)
    {
        int lane = GetLane(e);
        Note note = PeekFirstInLane(lane);
        if (note && 
            note.GetNoteType() == NoteType.Flick &&
            touchStartPositions[lane] != Vector3.zero &&
            Vector3.Distance(e.pointerCurrentRaycast.worldPosition, touchStartPositions[lane]) > FLICK_THRESHOLD)
        {
            OnNotePressed(note);
            touchStartPositions[lane] = Vector3.zero;
        }
        foreach(KeyValuePair<Note, Vector3> pair in dragNotes)
        {   
            if(pair.Key.GetComponent<Collider2D>().OverlapPoint(e.pointerCurrentRaycast.worldPosition))
            {
                dragNotes[pair.Key] = e.pointerCurrentRaycast.worldPosition;
            }
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        // if(currentlyHeldNotes.ContainsKey(e.pointerId))
        // {
        //     Debug.Log("released hold");
        // }
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
        Accuracy accuracy = GetAccuracy(note);
        ScoreManager.scoreManager.IncreaseScore(accuracy);
        if (note.GetNoteType() != NoteType.Hold)
        {
            notes[GetLane(note.transform.position)].Dequeue();
            Destroy(note.gameObject);
        }
    }

    // Maps location to lane
    private int GetLane(Collider2D col)
    {
        return ~LANE_LOCATIONS.BinarySearch(col.gameObject.transform.position.x);
    }

    // Maps location to lane
    private int GetLane(Vector3 loc)
    {
        return ~LANE_LOCATIONS.BinarySearch(loc.x);
    }

    // Maps location to lane
    private int GetLane(PointerEventData e)
    {
        return ~LANE_LOCATIONS.BinarySearch(e.pointerCurrentRaycast.worldPosition.x);
    }

    // Wrapper around Queue.Peek() to peek into a specific lane validation 
    // Removes and returns "first"/"lowest" note in lane if it exists, otherwise null
    private Note PeekFirstInLane(int lane)
    { 
        if (notes[lane].Count == 0) return null;
        else return notes[lane].Peek();
    }

    // TODO: Calculate accuracy based on distance from fall line
    private Accuracy GetAccuracy(Note note)
    {
        return Accuracy.Great;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class Lane : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    public static List<Line> LANE_LINES = new List<Line>();
    public static List<Line> LANE_LINES_FOR_OFFSET = new List<Line>();
    public static List<float> LANE_LOCATIONS = new List<float>();

    public readonly static float TOP_WIDTH = 1f;
    public readonly static float BOTTOM_WIDTH = 11.7f;

    public readonly static int NUM_LANES = 7;
    public readonly static float DISTANCE =  BeatManager.SPAWN_POINT - BeatManager.PLAY_POINT;
    public readonly static float TOP_DISTANCE_PER_LANE = TOP_WIDTH / NUM_LANES;
    public readonly static float BOTTOM_DISTANCE_PER_LANE = BOTTOM_WIDTH / NUM_LANES;
    public readonly static float TOP_TO_BOTTOM_DISTANCE_PER_LANE = (BOTTOM_WIDTH - TOP_WIDTH) / NUM_LANES;

    private Collider2D col;
    private static List<float> ACCURACY_BREAK_POINTS = new List<float>(){
            .25f, // Perfect
            .5f, // Great
            .7f, // Good
        };
    // Queue containing the notes that are in the "hittable" range
    private List<Queue<Note>> notes;

    void Start()
    {
        // Physics2D.queriesHitTriggers = true;
        // EnhancedTouchSupport.Enable();
        if (LANE_LOCATIONS.Count == 0)
        {
            SetLaneLocations();
        }

        col = GetComponent<Collider2D>();
        notes = new List<Queue<Note>>();
        for(int i = 0; i < LANE_LOCATIONS.Count; i++) {
            notes.Add(new Queue<Note>());
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Note note = other.gameObject.GetComponent<Note>();
        if (note.isFlick)
        {
            Debug.Log(note.lane);
        }
        notes[note.lane].Enqueue(note);
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        /**
         * Note by austin
         *  Theoretically, `notes` can never be empty when OnTriggerExit is called. There are two scenarios for dequeing `notes`:
         *  1) Player pressed it. In this case, then, the note should be destroyed, and OnTriggerExit should not be called since the object never left the trigger (cant leave the trigger if its destroyed)
         *  2) Player missed it. Then, `notes` never got dequeued since the original enqueue so `notes` cannot be empty
         */

        if (!(other == null || other.gameObject == null))
        {
            Note temp;
            if (other.gameObject.GetComponent<Note>() == notes[other.gameObject.GetComponent<Note>().lane].TryPeek(out temp))
            {
                notes[other.gameObject.GetComponent<Note>().lane].Dequeue();
                HealthManager.healthManager.DecreaseHealth(HealthManager.MISS_NOTE_AMOUNT);
                StartCoroutine(DelayedDestroy(other.gameObject));
            }
        }
    }

    /**
     * Leverages the Unity builtin OnPointerDown event to handle tap notes
     * Since tap notes only require a single tap, we can use OnPointerDown  to track that happening.
     */
    public void OnPointerDown(PointerEventData e)
    {
        int lane = GetLane(e);
        Note note;
        if (notes[lane].TryPeek(out note))
        {  
            if (!note.isFlick)
            {
                notes[lane].Dequeue();
                OnNotePressed(note);
            }
        }
    }

    public void OnDrag(PointerEventData e)
    {
        int lane = GetLane(e.pointerPressRaycast.worldPosition.x);
        Note note;
        Debug.Log($"drag at {lane}");
        if (notes[lane].TryPeek(out note))
        {  
            Debug.Log("dragging bruh");
            notes[lane].Dequeue();
            OnNotePressed(note);
        }
    }

    /* Helpers */

    // Maps location to lane
    private int GetLane(PointerEventData e)
    {
        return (~LANE_LOCATIONS.BinarySearch(e.pointerCurrentRaycast.worldPosition.x)) - 1;
    }

    // Maps location to lane
    private int GetLane(float x)
    {
        return (~LANE_LOCATIONS.BinarySearch(x)) - 1;
    }

    /*
     * Although detection for each note type has to be unique, once we know a note has been pressed,
     * the score, health, and combo calcs should be the same irrespective of the note type
     * ... although that isn't necessarily set in stone.
     */
    // Gets accuracy, tells ScoreManager to increase the score, and destroys the pressed Note.
    public void OnNotePressed(Note note)
    {
        ParticleManager.particleManager.EmitParticlesOnPress(
            new Vector3((note.lane - Lane.NUM_LANES / 2) * BOTTOM_DISTANCE_PER_LANE, 0, 0)
        );
        Accuracy accuracy = GetAccuracy(note);
        ScoreManager.scoreManager.IncreaseScore(accuracy);
        Destroy(note.gameObject);
    }

    protected Accuracy GetAccuracy(Note note)
    {
        float timeFromLine = Mathf.Abs(note.transform.position.y - BeatManager.PLAY_POINT) / Note.fallSpeed;
        int acc = ACCURACY_BREAK_POINTS.BinarySearch(timeFromLine);
        if (acc < 0)
            acc = ~acc;
        switch(acc)
        {
            case 0:
                return Accuracy.Perfect;
            case 1:
                return Accuracy.Great;
            case 2:
                return Accuracy.Good;
            case 3:
            default:
                HealthManager.healthManager.DecreaseHealth(HealthManager.BAD_NOTE_AMOUNT);
                return Accuracy.Bad;
        }
    }

    private IEnumerator DelayedDestroy(GameObject obj)
    {
        yield return new WaitForSeconds(.5f);
        Destroy(obj);
    }

    private void SetLaneLocations()
    {
        for (int i = 0; i < 8; i++)
        {
            LANE_LOCATIONS.Add((i - NUM_LANES / 2f) * BOTTOM_DISTANCE_PER_LANE);

            LANE_LINES_FOR_OFFSET.Add(new Line(
                new Vector2((i - NUM_LANES / 2f) * TOP_DISTANCE_PER_LANE, BeatManager.SPAWN_POINT),
                new Vector2((i - NUM_LANES / 2f) * BOTTOM_DISTANCE_PER_LANE, BeatManager.PLAY_POINT)));
        }
        
        for (int i = 0; i < NUM_LANES; i++)
        {
            LANE_LINES.Add(new Line(
                new Vector2((i - NUM_LANES / 2) * TOP_DISTANCE_PER_LANE, BeatManager.SPAWN_POINT),
                new Vector2((i - NUM_LANES / 2) * BOTTOM_DISTANCE_PER_LANE, BeatManager.PLAY_POINT)));
        }
    }
}
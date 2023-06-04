using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;

/**
 * Stores the slope intercept form of a line
 * Exists to calculate the midpoint X value of a given note at any Y point
 * I'm pretty sure this should get refactored to be a wrapper around Vector2.Lerp
 */
public class Line
{
    private Vector2 p1;
    private Vector2 p2;
    private float y;
    public Line(Vector2 p1, Vector2 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.y = p1.y - p2.y;
    }

    public float getX(float Y)
    {
        return Vector2.LerpUnclamped(p1, p2, (p1.y - Y)/y).x;
    }
    
    public float Lerp(float t)
    {
        return Vector2.Lerp(p1, p2, t).x;
    }
}

[RequireComponent(typeof(Collider2D))]
public class Lane : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
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
            .125f, // Perfect
            .25f, // Great
            .35f, // Good
        };

    // Queue containing the notes that are in the "hittable" range
    private List<Queue<Note>> notes;
    private List<HoldNote> holdNotes;
    private HashSet<PointerEventData> holds;

    private int notesTillNextHeal;

    [SerializeField] private Animator[] laneAnimators;
    [SerializeField] private AudioSource audioSourceOneShots;
    [SerializeField] private AudioClip tapSfx;
    [SerializeField] private AudioClip flickSfx;
    [SerializeField] private AudioClip holdSfx;

    void Awake()
    {
        Vibration.Init();
        // Physics2D.queriesHitTriggers = true;
        // EnhancedTouchSupport.Enable();
        if (LANE_LOCATIONS.Count == 0)
            SetLaneLocations();

        col = GetComponent<Collider2D>();
        notes = new List<Queue<Note>>();
        for(int i = 0; i < LANE_LOCATIONS.Count; i++)
            notes.Add(new Queue<Note>());
        holdNotes = new List<HoldNote>();
        holds = new HashSet<PointerEventData>();
        notesTillNextHeal = 20;
    }
    
    void Update()
    {
        holdNotes.RemoveAll(x=>x==null);
        foreach (HoldNote holdNote in holdNotes)
        {
            holdNote.holding = false;
            foreach (PointerEventData touch in holds)
            {
                Vector2 loc = new Vector2(touch.pointerCurrentRaycast.worldPosition.x, BeatManager.PLAY_POINT);
                if (holdNote.GetComponent<Collider2D>().OverlapPoint(loc))
                {
                    holdNote.holding = true;
                    break;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Note note = other.gameObject.GetComponent<Note>();
        if (note) notes[note.lane].Enqueue(note);
        else holdNotes.Add(other.gameObject.GetComponent<HoldNote>());
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
            Note note = other.gameObject.GetComponent<Note>();
            if (!note) 
            {
                StartCoroutine(DelayedDestroy(other.gameObject));
            }
            else 
            {
                if (!note.hit)
                {
                    HealthManager.healthManager.DecreaseHealth(HealthManager.MISS_NOTE_AMOUNT);
                    ScoreManager.scoreManager.IncreaseScore(Accuracy.Miss);
                    StartCoroutine(DelayedDestroy(other.gameObject));
                }
                notes[note.lane].Dequeue();
            }
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        int lane = GetLane(e);
        Note note;
        if (0 <= lane && lane < NUM_LANES)
        {  
            laneAnimators[lane].Play("Flash");
            if(notes[lane].TryPeek(out note))
            {
                if (note.isHold != null && !note.isEnd)
                {
                    note.isHold.holding = true;
                    OnNotePressed(note, lane);
                }
                if (!note.isFlick)
                {
                    OnNotePressed(note, lane);
                }
            }
        }
        holds.Add(e);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if(e.rawPointerPress != null)
        {
            holds.Add(e);
        }
    }
    
    public void OnPointerUp(PointerEventData e)
    {
        int lane = GetLane(e);
        Note note;
        if (0 <= lane && lane < NUM_LANES && notes[lane].TryPeek(out note))
        {  
            if (note.isHold != null && note.isEnd)
            {
                // laneAnimators[lane].Play("Flash");
                Destroy(note.isHold.gameObject);
                OnNotePressed(note, lane);
            }
        }
        holds.Remove(e);
    }

    public void OnPointerExit(PointerEventData e)
    {
        holds.Remove(e);
    }

    // Leverages the Unity builtin OnDrag event to handle flick notes
    //  and flick notes at the end of a hold
    //  and also tap notes, but that should never happen
    public void OnDrag(PointerEventData e)
    {
        int lane = GetLane(e.pointerPressRaycast.worldPosition.x);
        Note note;
        if (0 <= lane && lane < NUM_LANES && notes[lane].TryPeek(out note))
        {  
            if (note.isHold == null && note.isFlick)
            {
                OnNotePressed(note, lane);
            }
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

    // Gets accuracy, tells ScoreManager to increase the score, and destroys the pressed Note.
    private void OnNotePressed(Note note, int lane)
    {
        if (BeatManager.beatManager.IsPlaying)
        {
            ParticleManager.particleManager.EmitParticlesOnPress(
                new Vector3((note.lane - Lane.NUM_LANES / 2) * BOTTOM_DISTANCE_PER_LANE, 0, 0)
            );
            Accuracy accuracy = GetAccuracy(note);
            ScoreManager.scoreManager.IncreaseScore(accuracy);
            Vibration.VibratePeek();
            note.hit = true;

            if (note.isFlick) audioSourceOneShots.PlayOneShot(flickSfx);
            else audioSourceOneShots.PlayOneShot(tapSfx);

            notesTillNextHeal -= 1;
            if (notesTillNextHeal == 0)
            {
                HealthManager.healthManager.IncreaseHealth(SkillManager.skillManager.healAmount);
                SkillManager.skillManager.AnimateSkill(Zodiac.Rabbit);
            }
    
            Destroy(note.gameObject);
        }
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
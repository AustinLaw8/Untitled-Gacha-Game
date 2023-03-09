using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * Base class for all notes
 * Handles main update loop of moving note
 * And checks its position relative to interactable zone
 */
[RequireComponent(typeof(Collider2D))]
public abstract class Note : MonoBehaviour
{
    /* Reference Values */
    protected static float TOP_WIDTH = 1f;
    protected static float BOTTOM_WIDTH = 11.7f;
    protected static int NUM_LANES = 7;
    private static float START_SCALE = .05f;
    private static float END_SCALE = .75f;
    protected static float DISTANCE =  BeatManager.SPAWN_POINT - BeatManager.PLAY_POINT;
    protected static float TOP_DISTANCE_PER_LANE = TOP_WIDTH / NUM_LANES;
    protected static float BOTTOM_DISTANCE_PER_LANE = BOTTOM_WIDTH / NUM_LANES;
    private static float TOP_TO_BOTTOM_DISTANCE_PER_LANE = (BOTTOM_WIDTH - TOP_WIDTH) / NUM_LANES;

    // TODO: Figure out these values
    private static List<float> ACCURACY_BREAK_POINTS = new List<float>(){
        .25f, // Perfect
        .5f, // Great
        .7f, // Good
    };

    [SerializeField] public static float fallSpeed = 2f;

    protected float fallTime;
    protected int laneOffset;
    protected float scaleRate;
    protected float xSpeed;
    protected float ySpeed;
    protected float smoothing;


    // If the note is within the interactable zone
    private bool interactable;
    public bool IsInteractable { get { return interactable; } } 

    // Collider of the lane (interactable zone) and the Note itself
    protected Collider2D playLine;
    protected Collider2D col;

    protected float timer;

    private bool clicked;

    protected virtual void Awake()
    {
        playLine = GameObject.Find("Lane").GetComponent<Collider2D>();
        col = GetComponent<Collider2D>();
        interactable = false;
        fallTime = DISTANCE / fallSpeed;
        scaleRate = (END_SCALE - START_SCALE) / fallTime;
        ySpeed = fallSpeed;
        this.transform.localScale = new Vector3(START_SCALE, START_SCALE,START_SCALE);
    }
    
    // Drops note, and determines if it is interactable yet
    protected virtual void Update()
    {
        timer += Time.deltaTime;

        // Calculates smoothing based on time spent falling
        smoothing = DISTANCE * fallTime / (fallSpeed * 2 * timer);

        // Calculates ySpeed, xSpeed, and scalingRate based on smoothing
        ySpeed = DISTANCE / smoothing;
        xSpeed = (laneOffset * TOP_TO_BOTTOM_DISTANCE_PER_LANE) / smoothing;
        scaleRate = (END_SCALE - START_SCALE) / smoothing;

        // Modifies transforms based on x/y speed, and scaling rate
        Offset(Time.deltaTime);

        // Checks if touching lane
        interactable = col.IsTouching(playLine);
    }

    // If the note passes the interactable zone, destroy it
    void OnTriggerExit2D(Collider2D other)
    {
        if (other == playLine && !clicked)
        {
            StartCoroutine(DelayedDestroy());
            HealthManager.healthManager.DecreaseHealth(HealthManager.MISS_NOTE_AMOUNT);
        }
    }

    public virtual void SetLane(int lane)
    {
        laneOffset = (lane - NUM_LANES / 2);
        this.transform.position += new Vector3(laneOffset * TOP_DISTANCE_PER_LANE, 0, 0);
        xSpeed = (laneOffset * TOP_TO_BOTTOM_DISTANCE_PER_LANE) / fallTime;
    }

    public void Offset(float time)
    {
        this.transform.position += new Vector3(xSpeed, -ySpeed, 0) * time;
        this.transform.localScale += new Vector3(scaleRate, scaleRate, scaleRate) * time;
    }

    /* Helpers */

    /**
     * Although detection for each note type has to be unique, once we know a note has been pressed,
     * the score, health, and combo calcs should be the same irrespective of the note type
     * ... although that isn't necessarily set in stone.
     */
    // Gets accuracy, tells ScoreManager to increase the score, and destroys the pressed Note.
    protected void OnNotePressed()
    {
        if (interactable)
        {
            clicked = true;
            ParticleManager.particleManager.EmitParticlesOnPress(this.transform.position);
            Accuracy accuracy = GetAccuracy();
            ScoreManager.scoreManager.IncreaseScore(accuracy);
            Destroy(this.gameObject);
        }
    }

    protected Accuracy GetAccuracy()
    {
        float timeFromLine = Mathf.Abs(this.transform.position.y - BeatManager.PLAY_POINT) / fallSpeed;
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

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(.1f);
        Destroy(this.gameObject);
    }
}

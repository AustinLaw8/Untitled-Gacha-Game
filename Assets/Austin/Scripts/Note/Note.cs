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
    // Reference values
    // Hard coding this here for reference but distance from spawn to line is 8.4
    
    private static float BUMP = .145f;    
    private static float START_SCALE = .05f;
    private static float END_SCALE = .7f;
    private static float DISTANCE = 8.4f;

    [SerializeField] public static float fallSpeed = 2f;
    private static float fallTime;
    private float scaleRate;

    private float xSpeed;
    private float ySpeed;

    // If the note is within the interactable zone
    private bool interactable;
    public bool IsInteractable { get { return interactable; } } 

    // Collider of the lane (interactable zone) and the Note itself
    protected Collider2D lane;
    protected Collider2D col;
    private int noteLane;

    private float timer;

    protected virtual void Awake()
    {
        lane = GameObject.Find("Lane").GetComponent<Collider2D>();
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
        ySpeed = fallSpeed * (timer / (fallTime / 2));

        xSpeed = ((noteLane - 3) * 1.55f) / (DISTANCE / ySpeed);
        scaleRate = (END_SCALE - START_SCALE) / (DISTANCE / ySpeed);

        Offset(Time.deltaTime);
        interactable = col.IsTouching(lane);
    }

    // If the note passes the interactable zone, destroy it
    void OnTriggerExit2D(Collider2D other)
    {
        if (other == lane)
        {
            StartCoroutine(DelayedDestroy());
            // TODO: Deal damage to player
        }
    }

    public virtual void SetLane(int lane)
    {
        this.noteLane = lane;
        this.transform.position += new Vector3((noteLane - 3) * BUMP, 0, 0);
        this.xSpeed = ((noteLane - 3) * 1.55f) / fallTime;
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
            ParticleManager.particleManager.EmitParticlesOnPress(this.transform.position);
            Accuracy accuracy = GetAccuracy();
            ScoreManager.scoreManager.IncreaseScore(accuracy);
            Destroy(this.gameObject);
        }
    }

    // TODO: Calculate accuracy based on distance from fall line/time difference between press and intended
    protected Accuracy GetAccuracy()
    {
        return Accuracy.Great;
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(.1f);
        Destroy(this.gameObject);
    }
}

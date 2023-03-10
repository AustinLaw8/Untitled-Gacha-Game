using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * Base class for all notes
 * Handles main update loop of moving note
 * And checks its position relative to interactable zone
 */
// [RequireComponent(typeof(Collider2D))]
public class Note : MonoBehaviour
{
    [SerializeField] private bool flick;
    [SerializeField] private Sprite leftLane;
    [SerializeField] private Sprite middleLane;
    [SerializeField] private Sprite rightLane;

    /* Reference Values */
    private static float START_SCALE = .05f;
    private static float END_SCALE = .75f;

    public static float fallSpeed = 2f;

    private float fallTime;
    private int laneOffset;
    private float scaleRate;
    private float xSpeed;
    private float ySpeed;
    private float smoothing;

    public bool isFlick { get=>flick; }
    public int lane { get=> laneOffset + Lane.NUM_LANES / 2; }

    protected float timer;

    void Awake()
    {
        fallTime = Lane.DISTANCE / fallSpeed;
        this.transform.localScale = new Vector3(START_SCALE, START_SCALE,START_SCALE);
    }
    
    // Drops note
    void Update()
    {
        timer += Time.deltaTime;

        // Calculates smoothing based on time spent falling
        smoothing = Lane.DISTANCE * fallTime / (fallSpeed * 2 * timer);

        // Calculates ySpeed, xSpeed, and scalingRate based on smoothing
        ySpeed = Lane.DISTANCE / smoothing;
        xSpeed = (laneOffset * Lane.TOP_TO_BOTTOM_DISTANCE_PER_LANE) / smoothing;
        scaleRate = (END_SCALE - START_SCALE) / smoothing;

        // Modifies transforms based on x/y speed, and scaling rate
        Offset(Time.deltaTime);
    }

    public void SetLane(int lane)
    {
        laneOffset = (lane - Lane.NUM_LANES / 2);
        this.transform.position += new Vector3(laneOffset * Lane.TOP_DISTANCE_PER_LANE, 0, 0);
        xSpeed = (laneOffset * Lane.TOP_TO_BOTTOM_DISTANCE_PER_LANE) / fallTime;

        if (lane < 3)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = leftLane;
        }
        else if (lane == 3)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = middleLane;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = rightLane;
        }
    }

    public void Offset(float time)
    {
        this.transform.position += new Vector3(xSpeed, -ySpeed, 0) * time;
        this.transform.localScale += new Vector3(scaleRate, scaleRate, scaleRate) * time;
    }
}

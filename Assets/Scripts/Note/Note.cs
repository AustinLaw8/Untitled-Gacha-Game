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
    [SerializeField] private HoldNote hold;
    [SerializeField] private bool end;
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
    public bool isEnd { get=>end; set=>end=value; }
    public HoldNote isHold { get=>hold; set=>hold=value; }
    public bool hit;
    public int lane { get=> laneOffset + Lane.NUM_LANES / 2; }

    protected float timer;

    void Awake()
    {
        hit = false;
        fallTime = Lane.DISTANCE / fallSpeed;
        timer = 0f;
        this.transform.localScale = new Vector3(START_SCALE, START_SCALE,START_SCALE);
    }
    
    // Drops note
    void Update()
    {
        timer += Time.deltaTime;

        // Calculates smoothing based on time spent falling
        smoothing = Mathf.Lerp(0,2,timer/fallTime);

        // Calculates ySpeed, xSpeed, and scalingRate based on smoothing
        ySpeed = Lane.DISTANCE / fallTime * smoothing;
        xSpeed = (laneOffset * Lane.TOP_TO_BOTTOM_DISTANCE_PER_LANE) / fallTime * smoothing;
        scaleRate = (END_SCALE - START_SCALE) / fallTime * smoothing;

        // Move and scale
        this.transform.position += new Vector3(xSpeed, -ySpeed, 0) * Time.deltaTime;
        this.transform.localScale += new Vector3(scaleRate, scaleRate, scaleRate) * Time.deltaTime;
    }

    // Sets laneOffset, position, and sprite based on lane
    public void SetLane(int lane)
    {
        laneOffset = (lane - Lane.NUM_LANES / 2);
        this.transform.position += new Vector3(laneOffset * Lane.TOP_DISTANCE_PER_LANE, 0, 0);

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

    // Bumps note based on deltaTime
    // Used to account for slight time difference between intended spawn time and update
    public void Bump(float deltaTime)
    {
        timer += deltaTime;
        float temp = Mathf.Pow(deltaTime/fallTime, 2);
        this.transform.position += 
            new Vector3(
                temp * Lane.TOP_DISTANCE_PER_LANE,
                temp * Lane.DISTANCE,
                0f
            );
    }
}

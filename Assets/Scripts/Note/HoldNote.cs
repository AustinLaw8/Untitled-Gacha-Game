using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Stores the slope intercept form of a line
 * Exists to calculate the midpoint X value of a given note at any Y point
 */
public class Line
{
    private float slope;
    private float intercept;
    private float x;

    public Line(Vector2 p1, Vector2 p2)
    {
        slope = (p2.y - p1.y) / (p2.x - p1.x);
        x = p1.x;
        intercept = p1.y - slope * p1.x;
    }

    public float getX(float Y)
    {
        if (!float.IsFinite(slope))
        {
            return x;
        }
        return (Y - intercept) / slope;
    }

    public override string ToString()
    {
        return $"slope: {slope}, intercept: {intercept}";
    }
}

// Given the lines a hold note follows, handles the interactions of tapping, particle effects, and holding
public class HoldNote : Note, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static List<Line> LANE_LINES = new List<Line>();
    public static List<Line> LANE_LINES_FOR_OFFSET = new List<Line>();

    private bool holding;
    private ParticleSystem particles;

    // The lines about which this hold note will follow
    private List<Line> midLines;
    // The distances between each pivot point (if a note is made of multiple lines)
    private List<float> distances;
    // Pointer to whatever line is the currently interactable one
    private int currentLine;
    // Current amount of distance note has travelled
    private float distanceTravelled;

    private DrawRect drawer;

    protected override void Awake()
    {
        base.Awake();
        if (LANE_LINES.Count == 0)
        { 
            SetLaneLines();
        }
        currentLine = 0;
        holding = false;
        particles = null;
        drawer = this.gameObject.GetComponent<DrawRect>();

        distanceTravelled = -.5f;
        midLines = new List<Line>();
        distances = new List<float>();
        this.transform.localScale = Vector3.one;
    }
    
    /**
     * Once the note is tapped, gives points and plays particles while it is being held 
     */
    protected override void Update()
    {
        if (IsInteractable)
        {
            distanceTravelled += fallSpeed * Time.fixedDeltaTime;
            if (holding)
            {
                GiveHoldPoints();
                if(!particles.isPlaying) particles.Play();
                // Vector3 newPos = BoundsAtCurrentY();
                // if (newPos != Vector3.zero)
                // {
                //     particles.transform.position = newPos;
                // }
                // else
                // {
                //     particles.Stop();
                // }
            }
            else
            {
                if (particles) particles.Stop();
            }
        }
    }

    private void GiveHoldPoints()
    {
        Accuracy accuracy = GetAccuracy();
        ScoreManager.scoreManager.IncreaseScore(accuracy);
    }

    /**
     * Checks if player released the hold note at the end of the hold, and gives points if so
     * Also destroys the hold note since interaction is finished
     */
    public void OnPointerUp(PointerEventData e)
    {
        if (IsInteractable)
        {
            if (playLine.OverlapPoint(col.bounds.max))
            {
                Accuracy accuracy = GetAccuracy();
                ScoreManager.scoreManager.IncreaseScore(accuracy);
                Destroy(this.gameObject);
            }
        }
    }

    /**
     * When the player presses the hold note
     *  If interaction is at the beginning of the hold, gives the extra tap points
     *  Otherwise, it means the player released at some point and is pressing again now, so give hold points
     *  If a particle system doesn't exist yet, requests a particle system
     */
    public void OnPointerDown(PointerEventData e)
    {
        if (IsInteractable)
        {
            holding = true;
            if (particles == null)
            {
                particles = ParticleManager.particleManager.AttachParticlesForHold(this.transform);
            }
            if (playLine.OverlapPoint(col.bounds.min))
            {
                Accuracy accuracy = GetAccuracy();
                ScoreManager.scoreManager.IncreaseScore(accuracy);
            }
            else
            {
                GiveHoldPoints();
            }
        }
    }

    /**
     * Occurs when the player moves their finger from somewhere not on the hold into the hold
     *  Particle system null check occurs if player missed the initial tap
     */
    public void OnPointerEnter(PointerEventData e)
    {
        holding = e.rawPointerPress != null;
        if (holding && particles == null)
        {
            particles = ParticleManager.particleManager.AttachParticlesForHold(this.transform);
        }
    }
    
    // When player releases or their finger moves out of the hold at any point during its lifetime
    public void OnPointerExit(PointerEventData e)
    {
        holding = false;
    }

    // Returns the particles back to the particle manager if they were acquired
    void OnDestroy()
    {
        if (particles) ParticleManager.particleManager.DetachParticlesForHold(particles);
    }

    /** 
     * Interface to set the points the hold note will connect when initialized
     *  Calculates distances between points, lines, and calls the drawer to draw the mesh
     */
    public void SetPoints(List<(float, int)> points)
    {
        float offset = points[0].Item1;
        points = points.ConvertAll<(float, int)>( delegate ((float timeX, int laneX) x) {
            return (x.timeX - offset, x.laneX);
        });

        drawer.Init(points);
        // DrawRect drawer = GetComponent<DrawRect>();
        // List<Vector2> newPoints = new List<Vector2>();

        // newPoints.Add(new Vector2(LANE_LINES[points[0].lane].getX(5f), 5f));
        // for (int i = 1; i < points.Count; i++)
        // {
        //     float timeFromLast = points[i].time - points[i-1].time;
        //     Vector2 temp = new Vector2(LANE_LINES[points[i].lane].getX(5f), 5f + timeFromLast * Note.fallSpeed);
        //     newPoints.Add(temp);
        // }

        // drawer.SetPoints(newPoints);
        // drawer.Draw();
        // distances.Add(0);
        // for(int i = 0; i < newPoints.Count - 1; i++)
        // {
        //     midLines.Add(new Line(newPoints[i], newPoints[i+1]));
        //     distances.Add(newPoints[i + 1].y - newPoints[i].y + distances[i]);
        // }
        // distances.RemoveAt(0);
    }

    // private void UpdatePoints()
    // {
    //     drawer.UpdateVertices(points.ConvertAll<Vector3>( delegate ((float time, int lane) x) {
    //             float timeOffset = timer-x.time;
    //             float y = BeatManager.SPAWN_POINT-timeOffset*fallSpeed;
    //             return new Vector3(LANE_LINES_FOR_OFFSET[x.lane].getX(y), y , 0f);
    //         }).ToArray());
    // }

    // Acquires the (midpoint) x position of the mesh at the given y position
    // private Vector3 BoundsAtCurrentY()
    // {
    //     while (currentLine < distances.Count && distanceTravelled > distances[currentLine])
    //     {
    //         currentLine++;
    //     }
    //     if(currentLine == distances.Count)
    //     {
    //         return Vector3.zero;
    //     }
    //     else
    //     {
    //         return new Vector3(
    //             midLines[currentLine].getX(distanceTravelled),
    //             lane.transform.position.y,
    //             0f
    //         );
    //     }
    // }

    private void SetLaneLines()
    {
        for (int i = 0; i < Note.NUM_LANES; i++)
        {
            LANE_LINES.Add(new Line(
                new Vector2((i - Note.NUM_LANES / 2) * Note.TOP_DISTANCE_PER_LANE, BeatManager.SPAWN_POINT),
                new Vector2((i - Note.NUM_LANES / 2) * Note.BOTTOM_DISTANCE_PER_LANE, BeatManager.PLAY_POINT)));
            LANE_LINES_FOR_OFFSET.Add(new Line(
                new Vector2((i - Note.NUM_LANES * 1f / 2) * Note.TOP_DISTANCE_PER_LANE, BeatManager.SPAWN_POINT),
                new Vector2((i - Note.NUM_LANES * 1f / 2) * Note.BOTTOM_DISTANCE_PER_LANE, BeatManager.PLAY_POINT)));
        }
        LANE_LINES_FOR_OFFSET.Add(new Line(
            new Vector2((8 - Note.NUM_LANES * 1f / 2) * Note.TOP_DISTANCE_PER_LANE, BeatManager.SPAWN_POINT),
            new Vector2((8 - Note.NUM_LANES * 1f / 2) * Note.BOTTOM_DISTANCE_PER_LANE, BeatManager.PLAY_POINT)));
    }
}

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

    public Line(Vector2 p1, Vector2 p2)
    {
        slope = (p2.y - p1.y) / (p2.x - p1.x);
        intercept = p1.y - slope * p1.x;
    }

    public float getX(float Y)
    {
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

    protected override void Awake()
    {
        base.Awake();
        currentLine = 0;
        holding = false;
        particles = null;
        distanceTravelled = -.5f;
        midLines = new List<Line>();
        distances = new List<float>();
    }
    
    /**
     * Once the note is tapped, gives points and plays particles while it is being held 
     */
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsInteractable)
        {
            distanceTravelled += fallSpeed * Time.fixedDeltaTime;
            if (holding)
            {
                GiveHoldPoints();
                if(!particles.isPlaying) particles.Play();
                Vector3 newPos = BoundsAtCurrentY();
                if (newPos != Vector3.zero)
                {
                    particles.transform.position = newPos;
                }
                else
                {
                    particles.Stop();
                }
            }
            else
            {
                if (particles) particles.Stop();
            }
        }
    }

    /**
     * Checks if player released the hold note at the end of the hold, and gives points if so
     * Also destroys the hold note since interaction is finished
     */
    public void OnPointerUp(PointerEventData e)
    {
        if (IsInteractable)
        {
            if (lane.OverlapPoint(col.bounds.max))
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
            if (lane.OverlapPoint(col.bounds.min))
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

    /** 
     * Interface to set the points the hold note will connect when initialized
     *  Calculates distances between points, lines, and calls the drawer to draw the mesh
     */
    public void SetPoints(List<Vector2> newPoints)
    {
        DrawRect drawer = GetComponent<DrawRect>();
        drawer.SetPoints(newPoints);
        drawer.Draw();
        distances.Add(0);
        for(int i = 0; i < newPoints.Count - 1; i++)
        {
            midLines.Add(new Line(newPoints[i], newPoints[i+1]));
            distances.Add(newPoints[i + 1].y - newPoints[i].y + distances[i]);
        }
        distances.RemoveAt(0);
    }

    // Acquires the (midpoint) x position of the mesh at the given y position
    private Vector3 BoundsAtCurrentY()
    {
        while (currentLine < distances.Count && distanceTravelled > distances[currentLine])
        {
            currentLine++;
        }
        if(currentLine == distances.Count)
        {
            return Vector3.zero;
        }
        else
        {
            return new Vector3(
                midLines[currentLine].getX(distanceTravelled),
                lane.transform.position.y,
                0f
            );
        }
    }

    private void GiveHoldPoints()
    {
        Accuracy accuracy = GetAccuracy();
        ScoreManager.scoreManager.IncreaseScore(accuracy);
    }

    // Returns the particles back to the particle manager if they were acquired
    void OnDestroy()
    {
        if (particles) ParticleManager.particleManager.DetachParticlesForHold(particles);
    }
}

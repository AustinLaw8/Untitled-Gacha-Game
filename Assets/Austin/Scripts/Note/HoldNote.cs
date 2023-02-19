using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

public class HoldNote : Note, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    private bool holding;
    private ParticleSystem particles;

    private List<Line> midLines;
    private List<float> distances;
    private int currentLine;
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

    public void OnPointerEnter(PointerEventData e)
    {
        holding = e.rawPointerPress != null;
        if (holding && particles == null)
        {
            particles = ParticleManager.particleManager.AttachParticlesForHold(this.transform);
        }
    }
    
    public void OnPointerExit(PointerEventData e)
    {
        holding = false;
    }

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

    void OnDestroy()
    {
        if (particles) ParticleManager.particleManager.DetachParticlesForHold(particles);
    }
}

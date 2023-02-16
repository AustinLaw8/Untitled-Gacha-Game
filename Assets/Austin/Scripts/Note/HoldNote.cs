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
    private Line midpointLine;
    public float length;

    void Start()
    {
        PhysicsShapeGroup2D shapes = new PhysicsShapeGroup2D();
        col.GetShapes(shapes);
        midpointLine = new Line(
            (shapes.GetShapeVertex(0,0) + shapes.GetShapeVertex(0,1)) / 2,
            (shapes.GetShapeVertex(0,2) + shapes.GetShapeVertex(0,3)) / 2
        );
        length = Mathf.Abs(shapes.GetShapeVertex(0,2).y - shapes.GetShapeVertex(0,1).y);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsInteractable)
        {
            if (holding)
            {
                GiveHoldPoints();
                if(!particles.isPlaying) particles.Play();
                particles.transform.position = BoundsAtCurrentY();
                // Debug.Log(BoundsAtCurrentY());
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

    private Vector3 BoundsAtCurrentY()
    {
        return new Vector3(
            midpointLine.getX(lane.transform.position.y - (transform.position.y - length)) - length,
            lane.transform.position.y,
            0f
        );
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

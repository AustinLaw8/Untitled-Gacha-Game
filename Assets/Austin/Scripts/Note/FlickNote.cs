using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlickNote : Note, IPointerDownHandler, IDragHandler
{
    private static float FLICK_THRESHOLD = .1f;

    private int pointerId;
    private Vector3 startPos;

    [SerializeField] private Sprite leftLane;
    [SerializeField] private Sprite middleLane;
    [SerializeField] private Sprite rightLane;

    // On tap, records tap location
    public void OnPointerDown(PointerEventData e)
    {
        startPos = e.pointerCurrentRaycast.worldPosition;
        pointerId = e.pointerId;
    }

    // On drag, checks if distance between initial tap position and current position registers as far enough to flick
    public void OnDrag(PointerEventData e)
    {
        if (e.pointerId == pointerId &&
            Vector3.Distance(startPos, e.pointerCurrentRaycast.worldPosition) > FLICK_THRESHOLD
        ) {
            OnNotePressed();
        }
    }

    public override void SetLane(int lane)
    {
        base.SetLane(lane);
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
}

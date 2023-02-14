using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlickNote : Note, IPointerDownHandler, IDragHandler
{
    private static float FLICK_THRESHOLD = .1f;

    private int pointerId;
    private Vector3 startPos;

    public void OnPointerDown(PointerEventData e)
    {
        startPos = e.pointerCurrentRaycast.worldPosition;
        pointerId = e.pointerId;
    }

    public void OnDrag(PointerEventData e)
    {
        if (e.pointerId == pointerId &&
            Vector3.Distance(startPos, e.pointerCurrentRaycast.worldPosition) > FLICK_THRESHOLD
        ) {
            OnNotePressed();
        }
    }
}

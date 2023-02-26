using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlickNote : Note, IPointerDownHandler, IDragHandler
{
    private static float FLICK_THRESHOLD = .1f;

    private int pointerId;
    private Vector3 startPos;

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
}

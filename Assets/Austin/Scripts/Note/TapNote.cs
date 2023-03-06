using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapNote : Note, IPointerDownHandler
{
    [SerializeField] private Sprite leftLane;
    [SerializeField] private Sprite middleLane;
    [SerializeField] private Sprite rightLane;

    // when you write a class that just calls the base class...
    public void OnPointerDown(PointerEventData e)
    {
        OnNotePressed();
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

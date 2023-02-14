using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldNote : Note, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool holding;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsInteractable && holding)
        {
            GiveHoldPoints();
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
    }
    
    public void OnPointerExit(PointerEventData e)
    {
        holding = false;
    }

    private void GiveHoldPoints()
    {
        Accuracy accuracy = GetAccuracy();
        ScoreManager.scoreManager.IncreaseScore(accuracy);

    }
}

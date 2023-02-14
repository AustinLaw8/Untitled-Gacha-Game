using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapNote : Note, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData e)
    {
        OnNotePressed();
    }
}

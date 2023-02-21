using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapNote : Note, IPointerDownHandler
{
    // when you write a class that just calls the base class...
    public void OnPointerDown(PointerEventData e)
    {
        OnNotePressed();
    }
}

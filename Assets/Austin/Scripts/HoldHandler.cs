using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class HoldHandler : MonoBehaviour, IDragHandler
{
    // Mapping between touches and notes being touched
    // Minor optimization making it directly map to collider2d to avoid a few extra calls
    private Dictionary<int, Collider2D> touchedNotes;
    private Collider2D col;


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        touchedNotes = new Dictionary<int, Collider2D>();
    }

    public void OnDrag(PointerEventData e)
    {
        Debug.Log("draggin");
        if(touchedNotes.ContainsKey(e.pointerId))
        {
            if(!col.IsTouching(touchedNotes[e.pointerId]))
            {
                Debug.Log("Miss!");
            }
            else
            {
                Debug.Log("Touching!");
            }
        }
    }

    public void SetTouch(int id, Collider2D note) { touchedNotes.Add(id, note); }
    public Dictionary<int, Collider2D> GetTouch() { return touchedNotes; }
}


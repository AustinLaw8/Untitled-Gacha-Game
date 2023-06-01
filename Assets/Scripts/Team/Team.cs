using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Team : MonoBehaviour
{
    private selectedCard script;
    private int pos;

    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(delegate () { OnCardSlotSelected(); });
        pos = b.name[b.name.Length-1] - '0';
    }
    
    public void OnCardSlotSelected()
    {
        // send message to selectedCard script
        int id = gameObject.GetComponent<CardIDIdentifier>().cardID;
        script = FindObjectOfType<selectedCard>();
        script.OnCardSelected(id, pos);
    }
}

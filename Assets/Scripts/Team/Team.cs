using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Team : MonoBehaviour
{
    //[SerializeField] private CardSO card;
    private selectedCard script;
    private int pos;
    //private TeamInventorySelected scriptRight;

    // Start is called before the first frame update
    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(delegate () { OnCardSlotSelected(); });
        pos = b.name[b.name.Length-1] - '0';
        //b.image.sprite = card.cardIcon;
    }
    
    public void OnCardSlotSelected()
    {
        // send message to selectedCard script
        int id = gameObject.GetComponent<CardIDIdentifier>().cardID;
        script = FindObjectOfType<selectedCard>();
        script.OnCardSelected(id, pos);
    }
}

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

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnCardSlotSelected()
    {
        //Debug.Log(card.artist);
        // send message to selectedCard script
        int id = gameObject.GetComponent<CardIDIdentifier>().cardID;
        Debug.Log("id of selected card " + id);
        script = FindObjectOfType<selectedCard>();
        script.OnCardSelected(id, pos);

        /*scriptRight = FindObjectOfType<TeamInventorySelected>();
        scriptRight.ClearImage();*/


    }
}

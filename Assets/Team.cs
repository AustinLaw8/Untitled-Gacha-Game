using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Team : MonoBehaviour
{
    [SerializeField] private CardSO card;
    private selectedCard script;

    // Start is called before the first frame update
    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(delegate () { OnCardSlotSelected(card); });
        b.image.sprite = card.cardIcon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnCardSlotSelected(CardSO card)
    {
        Debug.Log(card.artist);
        // send message to selectedCard script
        script = FindObjectOfType<selectedCard>();
        script.OnCardSelected(card);
    }
}

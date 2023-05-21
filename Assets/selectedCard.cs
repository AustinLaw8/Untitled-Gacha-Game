using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// card selected to be swapped out from team
public class selectedCard : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCardSelected(CardSO card)
    {
        Button b = gameObject.GetComponent<Button>();
        b.image.sprite = card.cardIcon;
    }
}

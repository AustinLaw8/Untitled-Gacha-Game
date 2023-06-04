using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TeamInventorySelected : MonoBehaviour
{
    void Start()
    {
        ClearImage();
    }
    
    public void OnCardSelected(int id)
    {
        FindObjectOfType<MemberSwap>().SetTeamId(id);
        Sprite s = CardManager.cardManager.cardDB[id].cardIcon;
        gameObject.GetComponent<Image>().sprite = s;
    }

    // for clearing the image of the button when first entering the page
    public void ClearImage()
    {
        gameObject.GetComponent<Image>().sprite = CardManager.cardManager.emptyCard.cardIcon;
    }
}

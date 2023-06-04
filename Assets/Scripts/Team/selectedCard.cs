using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// card selected to be swapped out from team
public class selectedCard : MonoBehaviour
{
    private MemberSwap script;

    public void OnCardSelected(int id, int pos)
    {
        FindObjectOfType<MemberSwap>().SetTeamPos(pos);
        if (id != 3900)
            gameObject.GetComponent<Image>().sprite = CardManager.cardManager.cardDB[id].cardIcon;
        else
            gameObject.GetComponent<Image>().sprite = CardManager.cardManager.emptyCard.cardIcon;

    }

    public void ClearImage()
    {
        gameObject.GetComponent<Image>().sprite = null;
    }
}

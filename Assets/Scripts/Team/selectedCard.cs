using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// card selected to be swapped out from team
public class selectedCard : MonoBehaviour
{
    //[SerializeField] TeamManager teamManager;
    [SerializeField] CardManager cardManager;
    private MemberSwap script;

    public void OnCardSelected(int id, int pos)
    {
        FindObjectOfType<MemberSwap>().SetTeamPos(pos);
        if (id != 3900)
            gameObject.GetComponent<Image>().sprite = cardManager.cardDB[id].cardIcon;
        else
            gameObject.GetComponent<Image>().sprite = cardManager.emptyCard.cardIcon;

    }

    public void ClearImage()
    {
        gameObject.GetComponent<Image>().sprite = null;
    }
}

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
        /*Debug.Log(teamManager);
        Debug.Log(teamManager.cardDB[0]);*/
        script = FindObjectOfType<MemberSwap>();
        script.SetTeamID(id);
        script.SetTeamPos(pos);


        Sprite s = null;
        Button b = gameObject.GetComponent<Button>();
        for (int i = 0; i < cardManager.cardDB.Length; i++)
        {
            if (id == cardManager.cardDB[i].ID)
            {
                s = cardManager.cardDB[i].cardIcon;
            }
        }
        b.image.sprite = s;
    }

    public void ClearImage()
    {
        Button b = gameObject.GetComponent<Button>();
        b.image.sprite = null;
    }
}

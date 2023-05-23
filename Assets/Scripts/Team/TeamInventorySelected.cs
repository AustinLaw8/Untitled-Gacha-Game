using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TeamInventorySelected : MonoBehaviour
{
    [SerializeField] CardManager invManager;
    private MemberSwap script;

    public void OnCardSelected(int id)
    {
        //Debug.Log(id);
        script = FindObjectOfType<MemberSwap>();
        script.SetInvID(id);
        Sprite s = invManager.cardDB[0].cardIcon;
        Button b = gameObject.GetComponent<Button>();
        for (int i = 0; i < invManager.cardDB.Length; i++)
        {
            if (id == invManager.cardDB[i].ID)
            {
                s = invManager.cardDB[i].cardIcon;
            }
        }
        b.image.sprite = s;
    }

    // for clearing the image of the button when first entering the page
    // TODO clear image
    public void ClearImage()
    {
        Button b = gameObject.GetComponent<Button>();
        b.image.sprite = null;
    }
}

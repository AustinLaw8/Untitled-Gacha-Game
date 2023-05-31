using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TeamInventorySelected : MonoBehaviour
{
    [SerializeField] CardManager invManager;

    void Start()
    {
        ClearImage();
    }
    
    public void OnCardSelected(int id)
    {
        FindObjectOfType<MemberSwap>().SetTeamId(id);
        Sprite s = invManager.cardDB[id].cardIcon;
        gameObject.GetComponent<Button>().image.sprite = s;
    }

    // for clearing the image of the button when first entering the page
    // TODO clear image
    public void ClearImage()
    {
        gameObject.GetComponent<Image>().sprite = invManager.emptyCard.cardIcon;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetHomeBkgd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int id = TeamManager.GetTeam()[0];
        CardSO card = CardManager.cardManager.cardDB[id];
        if (id != 3900)
        {
            GetComponent<RawImage>().texture = card.cardArt;
        }
    }
}

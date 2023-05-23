using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : MonoBehaviour
{
    public void OpenProfile()
    {
        ModalManager.instance.ShowModal(GetComponent<CardIDIdentifier>().cardID);
    }
}

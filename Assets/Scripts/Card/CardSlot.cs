using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pressing;
    private float timer=0f;

    private TeamManager teamManager;

    void Start()
    {
        GameObject temp = GameObject.Find("TeamData");
        if (temp != null)
        {
            teamManager = temp.GetComponent<TeamManager>();
        }
        else
        {
            teamManager = null;
        }
    }

    void Update()
    {
        if (pressing)
        {
            timer += Time.deltaTime;
            if (timer > .5f)
            {
                if(!ModalManager.instance.Active()) OpenProfile();
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        pressing = true;
    }

    public void OnPointerUp(PointerEventData e)
    {
        pressing = false;
        if (timer < .5f)
        {
            if(teamManager) OnCardSlotSelected();
        }
        else
        {
            if(!ModalManager.instance.Active()) OpenProfile();
        }
    }
    
    void OpenProfile()
    {
        // CardManager.cardManager.PlayButtonSFX();
        int id = GetComponent<CardIDIdentifier>().cardID;
        if (CardManager.cardManager.cardDB[id].numCopies == 0) return;
        ModalManager.instance.ShowModal(id);
    }

    void OnCardSlotSelected()
    {
        // CardManager.cardManager.PlayButtonSFX();
        int id = gameObject.GetComponent<CardIDIdentifier>().cardID;
        int ind = teamManager.InTeam(id);
        if (ind != -1) FindObjectOfType<MemberSwap>().SetSwap(ind);
        FindObjectOfType<TeamInventorySelected>().OnCardSelected(id);
    }
}

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
                OpenProfile();
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (teamManager)
        {
            pressing = true;
        }
        else
        {
            OpenProfile();
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (teamManager)
        {
            pressing = false;
            if (timer < .5f)
            {
                OnCardSlotSelected();
            }
        }
        else
        {
            OpenProfile();
        }
    }
    
    void OpenProfile()
    {
        ModalManager.instance.ShowModal(GetComponent<CardIDIdentifier>().cardID);
    }

    void OnCardSlotSelected()
    {
        int id = gameObject.GetComponent<CardIDIdentifier>().cardID;
        if (GameObject.Find("TeamData").GetComponent<TeamManager>().InTeam(id)) return;
        FindObjectOfType<TeamInventorySelected>().OnCardSelected(id);
    }
}

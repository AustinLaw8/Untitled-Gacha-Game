using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamInventory : MonoBehaviour
{
    //game objects accessed in code
    [SerializeField] public GameObject cardSlotPrefab;
    [SerializeField] public GameObject cardScreen;

    //List of cards to display
    public List<CardSO> teamCards = new List<CardSO>();

    //List of card slots
    public List<GameObject> cardSlots = new List<GameObject>();

    //card manager for access to all cards the player has on their team
    [SerializeField] TeamManager teamManager;
    [SerializeField] CardManager cardManager;
    [SerializeField] CardSO emptyCard;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCards();

        cardSlots.Add(GameObject.Find("CardSlotTall 0"));
        cardSlots.Add(GameObject.Find("CardSlotTall 1"));
        cardSlots.Add(GameObject.Find("CardSlotTall 2"));
        cardSlots.Add(GameObject.Find("CardSlotTall 3"));
        cardSlots.Add(GameObject.Find("CardSlotTall 4"));


        /*for (int i = 0; i < teamManager.teamIDs.Length; i++)
        {
            //UpdateCards();
            UpdateTeamMember(teamCards[i], cardSlots[i]);
        }*/
        //UpdateCards();
    }

    // TODO - may have to fix thiss
    void Update()
    {
        UpdateCards();
        for (int i = 0; i < teamCards.Count; i++)
        {
            //Debug.Log(teamCards[i]);
            UpdateTeamMember(teamCards[i], cardSlots[i]);
        }
    }

    // Adds current team's ID & icon to the slots
    void UpdateTeamMember(CardSO card, GameObject slot)
    {
        slot.GetComponent<CardIDIdentifier>().cardID = (int)card.ID;
        slot.GetComponent<Image>().sprite = card.cardIcon;
    }

    // Updates current team list (cards)
    public void UpdateCards()
    {
        teamCards.Clear();
        for (int i = 0; i < teamManager.teamIDs.Length; i++)
        {
            teamCards.Add(FindCard(teamManager.teamIDs[i]));
        }
    }

    CardSO FindCard(int ID)
    {
        if (ID < cardManager.cardDB.Length)
        {
            return cardManager.cardDB[ID];
        }
        return emptyCard;
    }

}

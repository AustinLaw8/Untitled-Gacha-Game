using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamInventory : MonoBehaviour
{
    public static int TEAM_SIZE=5;

    //game objects accessed in code
    [SerializeField] public GameObject cardSlotPrefab;
    [SerializeField] public GameObject cardScreen;

    //List of cards to display
    public CardSO[] teamCards;

    //List of card slots
    [SerializeField] GameObject[] cardSlots;

    //card manager for access to all cards the player has on their team
    [SerializeField] TeamManager teamManager;
    [SerializeField] CardManager cardManager;
    [SerializeField] CardSO emptyCard;

    // Start is called before the first frame update
    void Start()
    {
        teamCards = new CardSO[TEAM_SIZE];
        UpdateCards();
    }

    // Adds current team's ID & icon to the slots
    void UpdateTeamMember(CardSO card, GameObject slot)
    {
        slot.GetComponent<CardIDIdentifier>().cardID = (int)card.ID;
        slot.GetComponent<Image>().sprite = card.cardIcon;
        slot.transform.GetChild(0).gameObject.SetActive(card.ID != 3900);
        slot.transform.GetChild(1).gameObject.SetActive(card.ID != 3900);
        slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{card.numCopies}";
    }

    // Updates current team list (cards)
    public void UpdateCards()
    {
        for (int i = 0; i < TEAM_SIZE; i++)
        {
            teamCards[i] = (FindCard(teamManager.teamIDs[i]));
            UpdateTeamMember(teamCards[i], cardSlots[i]);
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

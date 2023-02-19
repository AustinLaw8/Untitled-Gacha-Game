using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCardClass : MonoBehaviour
{
    [System.Serializable]
    public class Card
    {
        public int cardID;
        public Sprite fullCard;
        public Sprite cardIcon;
        public int numCopies = 0;
        public int cardRarity = 3;
        public int cardPower = 0;
    }

    [SerializeField] public Card[] allCards;

    public void addCard (int cardId)
    {
        if (allCards[cardId].numCopies == 0)
        {
            allCards[cardId].numCopies++;
            allCards[cardId].cardPower = (allCards[cardId].cardRarity * 1000);
        }
        else if (allCards[cardId].numCopies < 5)
        {  
            allCards[cardId].numCopies++;
            allCards[cardId].cardPower += (allCards[cardId].cardRarity * 500);
        }
        else
        {
            Debug.Log("Too many dupes (rip) time to give the players something else LOL");
        }        
    }
}

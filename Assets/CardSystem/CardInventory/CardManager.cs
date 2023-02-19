using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] public CardSO[] cardDB;

    public void Awake()
    {
        //Load cards in here
    }

    public void addCard (int cardId)
    {
        if (cardDB[cardId].numCopies == 0)
        {
            cardDB[cardId].numCopies++;
            if (cardDB[cardId].rarity == Rarity.Three)
            {
                cardDB[cardId].power = (3 * 1000);
            }
            else if (cardDB[cardId].rarity == Rarity.Four)
            {
                cardDB[cardId].power = (4 * 1000);
            }
            else if (cardDB[cardId].rarity == Rarity.Five)
            {
                cardDB[cardId].power = (5 * 1000);
            }
            else
            {
                cardDB[cardId].power = (6 * 1000);
            }
            
        }
        else if (cardDB[cardId].numCopies < 5)
        {  
            cardDB[cardId].numCopies++;
            if (cardDB[cardId].rarity == Rarity.Three)
            {
                cardDB[cardId].power += (3 * 500);
            }
            else if (cardDB[cardId].rarity == Rarity.Four)
            {
                cardDB[cardId].power += (4 * 500);
            }
            else if (cardDB[cardId].rarity == Rarity.Five)
            {
                cardDB[cardId].power += (5 * 500);
            }
            else
            {
                cardDB[cardId].power += (6 * 500);
            }
        }
        else
        {
            Debug.Log("Too many dupes (rip) time to give the players something else LOL");
        }        
    }


    



}

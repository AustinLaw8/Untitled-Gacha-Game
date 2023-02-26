using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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

    public static int[] FromJson(string json)
	{
		Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
		return wrapper.Items;
	}

	public static string ToJson(int[] array)
	{
		Wrapper wrapper = new Wrapper();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper);
	}

	[Serializable]
	private class Wrapper
	{
		public int[] Items;
	}


    string filepath { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "playerCards.json" ;} }
    public void SaveCards()
    {
        int[] cardAmounts = new int[cardDB.Length];
        int i = 0;
        while (i < cardDB.Length)
        {
            cardAmounts[i] = cardDB[i].numCopies;
            i++;
        }
        var cardData = ToJson(cardAmounts);
        Debug.Log(cardData);
        System.IO.File.WriteAllText(filepath, cardData, System.Text.Encoding.UTF8);
        Debug.Log(cardData);
    }

    public void LoadCards()
    {
        int[] cardAmounts = new int[cardDB.Length];
        var loadedCardData = System.IO.File.ReadAllText(filepath, System.Text.Encoding.UTF8);
        cardAmounts = FromJson(loadedCardData);
        int i = 0;
        while (i < cardDB.Length)
        {
            //update number of copies
            cardDB[i].numCopies = cardAmounts[i];
            //adjust card power accordingly
            if (cardDB[i].rarity == Rarity.Three)
            {
                if(cardAmounts[i] > 0)
                {
                    cardDB[i].power = (3 * 1000) + (3 * 500 * (cardAmounts[i]-1));
                }
                else
                {
                    cardDB[i].power = 0;
                }
                
            }
            else if (cardDB[i].rarity == Rarity.Four)
            {
                if(cardAmounts[i] > 0)
                {
                    cardDB[i].power = (4 * 1000) + (4 * 500 * (cardAmounts[i]-1));
                }
                else
                {
                    cardDB[i].power = 0;
                }
            }
            else if (cardDB[i].rarity == Rarity.Five)
            {
                if(cardAmounts[i] > 0)
                {
                    cardDB[i].power = (5 * 1000) + (5 * 500 * (cardAmounts[i]-1));
                }
                else
                {
                    cardDB[i].power = 0;
                }
            }
            else
            {
                if(cardAmounts[i] > 0)
                {
                    cardDB[i].power = (6 * 1000) + (6 * 500 * (cardAmounts[i]-1));
                }
                else
                {
                    cardDB[i].power = 0;
                }
            }
            i++;
        }
    }

}

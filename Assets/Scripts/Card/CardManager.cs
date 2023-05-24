using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CardManager : MonoBehaviour
{
    [SerializeField] public CardSO[] cardDB;
    public static CardManager cardManager { get; private set;  }
    static string cardFilepath { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "playerCards.json" ;} }
    
    public void Awake()
    {
        if (cardManager != null && cardManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            cardManager = this;
            DontDestroyOnLoad(this.gameObject);
        }

        try {
            LoadCards();
        } catch {
            SaveCards();
            LoadCards();
        }
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
        // Debug.Log(cardData);
        System.IO.File.WriteAllText(cardFilepath, cardData, System.Text.Encoding.UTF8);
        // Debug.Log(cardData);
    }


    public int calculatePower(int numCopies, Rarity rarity)
    {
        if (numCopies == 0)
        {
            return 0;
        }
        if (rarity == Rarity.Three)
        {
            return ((3 * 1000 + 3 * 500 * (numCopies - 1)));
        }
        else if (rarity == Rarity.Four)
        {
            return ((4 * 1000 + 4 * 500 * (numCopies - 1)));
        }
        else if (rarity == Rarity.Five)
        {
            return ((5 * 1000 + 5 * 500 * (numCopies - 1)));
        }
        else if (rarity == Rarity.Six)
        {
            return ((5 * 1000 + 5 * 500 * (numCopies - 1)));
        }
        return 0;
    }

    public void LoadCards()
    {
        int[] cardAmounts = new int[cardDB.Length];
        var loadedCardData = System.IO.File.ReadAllText(cardFilepath, System.Text.Encoding.UTF8);
        cardAmounts = FromJson(loadedCardData);
        int i = 0;
        while (i < cardDB.Length)
        {
            //update number of copies
            cardDB[i].numCopies = cardAmounts[i];
            cardDB[i].power = calculatePower(cardDB[i].numCopies, cardDB[i].rarity);
            //adjust card power accordingly
            i++;
        }
        // Array.Sort(cardDB, delegate (CardSO first, CardSO second) { return first.ID < second.ID ? -1 : 1; });
    }

}

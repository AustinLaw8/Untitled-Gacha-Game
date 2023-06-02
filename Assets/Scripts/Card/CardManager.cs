using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CardManager : MonoBehaviour
{
    [SerializeField] public CardSO[] cardDB;
    [SerializeField] public CardSO emptyCard;
    public static CardManager cardManager { get; private set;  }
    static string cardFilepath { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "playerCards.json" ;} }
    
    [Header("Button SFX Hack")]
    [SerializeField] public AudioClip buttonClick;
    private AudioSource audioSource;

    public static void ResetData()
    {
        if (cardManager == null) Debug.LogError("how did we get here?");
        foreach(var card in cardManager.cardDB)
        {
            card.numCopies = 0;
            card.power = 0;
        }
        cardManager.SaveCards();
        cardManager.LoadCards();
        // FIXME: give players their defaults
    }

    public void Awake()
    {
        if (cardManager != null && cardManager != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            cardManager = this;
            DontDestroyOnLoad(this.gameObject);
        }

        try {
            #if UNITY_EDITOR
            SaveCards();
            #else
            LoadCards();
            #endif
        } catch {
            SaveCards();
            LoadCards();
        }

        audioSource = this.gameObject.GetComponent<AudioSource>();
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
            else if (cardDB[cardId].rarity == Rarity.Five || cardDB[cardId].rarity == Rarity.Six)
            {
                cardDB[cardId].power = (5 * 1000);
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
            else if (cardDB[cardId].rarity == Rarity.Five || cardDB[cardId].rarity == Rarity.Six)
            {
                cardDB[cardId].power += (5 * 500);
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
        System.IO.File.WriteAllText(cardFilepath, cardData, System.Text.Encoding.UTF8);
    }


    public int calculatePower(int numCopies, Rarity rarity)
    {
        if (numCopies == 0)
        {
            return 0;
        }
        int multiplier;
        switch (rarity)
        {
            case Rarity.Three:
                multiplier = 3;
                break;
            case Rarity.Four:
                multiplier = 4;
                break;
            case Rarity.Five:
            case Rarity.Six:
                multiplier = 5;
                break;
            default:
                multiplier = 0;
                break;
        }
        return ((multiplier * 1000 + multiplier * 500 * (numCopies - 1)));
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
    }

    public void PlayButtonSFX()
    {
        audioSource.Play();
    }
}

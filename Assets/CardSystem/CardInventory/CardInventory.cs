using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInventory : MonoBehaviour
{
    public float firstX;
    public float firstY;
    public float xSpaceBetweenItem;
    public int numColumns;
    public float ySpaceBetweenItem;
    public int spaceBetween;

    [SerializeField] public GameObject cardSlotPrefab;
    [SerializeField] public GameObject cardScreen;
    [SerializeField] public GameObject scrollyBoxContents;
    
    // 0 - 3 Stars
    // 1 - 4 Stars
    // 2 - 5 Stars
    // 3 - 6 Stars
    // 4 - 1 copy
    // 5 - 2 copies
    // 6 - 3 copies
    // 7 - 4 copies
    // 8 - 5 copies
    // 9 - Rabbit
    // 10 - Dragon
    // 11 - Tiger
    // 12 - Horse
    [SerializeField] public bool[] filters;

    public List<CardSO> ownedCards = new List<CardSO>();
    //private List<CardSO> allCards = new List<CardSO>();

    [SerializeField] CardManager cardManager;

    public enum SortCategories
    {
        Rarity, NumberOfCopies, CardNum, Zodiac
    }

    // Start is called before the first frame update
    void Start()
    {
        
        Vector3[] v = new Vector3[4];
        cardScreen.GetComponent<RectTransform>().GetLocalCorners(v);

        xSpaceBetweenItem = - ((v[1].x - v[2].x) / 22) * 3;
        ySpaceBetweenItem = xSpaceBetweenItem;
        spaceBetween = (int) xSpaceBetweenItem / 3;

        //firstX = v[1].x + (xSpaceBetweenItem * 2 / 3);
        //firstY = (v[1].y) - (xSpaceBetweenItem * 2/ 3);

        //firstY = cardScreen.GetComponent<RectTransform>().offsetMin.y;
        for(int i = 0; i < cardManager.cardDB.Length; i++)
        {
            if (cardManager.cardDB[i].numCopies > 0)
            {
                ownedCards.Add(cardManager.cardDB[i]);
                //allCards.Add(cardManager.cardDB[i]);
            }
        }

        scrollyBoxContents.GetComponent<GridLayoutGroup>().cellSize = new Vector2(2*(xSpaceBetweenItem/3), 2*(xSpaceBetweenItem/3));
        scrollyBoxContents.GetComponent<GridLayoutGroup>().spacing = new Vector2(spaceBetween, spaceBetween);
        scrollyBoxContents.GetComponent<GridLayoutGroup>().padding = new RectOffset(spaceBetween, spaceBetween, spaceBetween, spaceBetween);
        scrollyBoxContents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (spaceBetween * (((ownedCards.Count + 6)/7) * 3 + 1)));
        Debug.Log(ownedCards.Count);
        Debug.Log(((ownedCards.Count + 6)/7));
        
        DisplayCards();
        
    }

    public void DisplayCards()
    {        
        for (int i = 0; i < ownedCards.Count; i++)
        {
            var obj = Instantiate(cardSlotPrefab, Vector2.zero, Quaternion.identity, scrollyBoxContents.transform);
            //obj.transform.SetParent(scrollyBoxContents.transform, true);
            // obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            //obj.GetComponent<RectTransform>().sizeDelta = new Vector2(2*(xSpaceBetweenItem/3), 2*(xSpaceBetweenItem/3));
            obj.GetComponent<Image>().sprite = ownedCards[i].cardIcon;
            
        }
    }

    public void SortBy(int categoryID)
    {
        Debug.Log(categoryID);
        //Category ID:
        // 0 - Rarity
        // 1 - Number of Copies
        // 2 - Card Number
        // 3 - Zodiac
        if (categoryID == 0)
        {
            ownedCards.Sort(SortByRarity);
        }
        else if (categoryID == 1)
        {
            ownedCards.Sort(SortByNumCopies);
        }
        else if (categoryID == 2)
        {
            ownedCards.Sort(SortByCardNum);
        }
        else
        {
            ownedCards.Sort(SortByZodiac);
        }

        //******* replace with UpdateDisplay(); after testing done ********
        foreach(Transform child in scrollyBoxContents.transform)
        {
            Destroy(child.gameObject);
        }
        DisplayCards();
        //*****************************************************************
    }

    public void FilterBy()
    {
        ownedCards.Clear();
        
        // 0 - 3 Stars
        // 1 - 4 Stars
        // 2 - 5 Stars
        // 3 - 6 Stars
        // 4 - 1 copy
        // 5 - 2 copies
        // 6 - 3 copies
        // 7 - 4 copies
        // 8 - 5 copies
        // 9 - Rabbit
        // 10 - Dragon
        // 11 - Tiger
        // 12 - Horse
        
        
    }

    public int SortByRarity(CardSO card1, CardSO card2)
    {
        if(card1.rarity == Rarity.Three)
        {
            if(card2.rarity == Rarity.Three)
            {
                return card1.ID.CompareTo(card2.ID);
            }
            return -1;
        }
        else if(card1.rarity == Rarity.Four)
        {
            if(card2.rarity == Rarity.Three)
            {
                return 1;
            }
            else if(card2.rarity == Rarity.Four)
            {
                return card1.ID.CompareTo(card2.ID);
            }
            else
            {
                return -1;
            }
        }
        else if(card1.rarity == Rarity.Five)
        {
            if(card2.rarity == Rarity.Six)
            {
                return -1;
            }
            else if(card2.rarity == Rarity.Five)
            {
                return card1.ID.CompareTo(card2.ID);
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if(card2.rarity == Rarity.Six)
            {
                return card1.ID.CompareTo(card2.ID);
            }
            return 1;
        }
    }

    public int SortByNumCopies(CardSO card1, CardSO card2)
    {
        if (card1.numCopies == card2.numCopies)
        {
            return card1.ID.CompareTo(card2.ID);
        }
        else
        {
            return card1.numCopies.CompareTo(card2.numCopies);
        }
    }

    public int SortByZodiac(CardSO card1, CardSO card2)
    {
        if (card1.zodiac == card2.zodiac)
        {
            return card1.ID.CompareTo(card2.ID);
        }
        else
        {
            if (card1.zodiac == Zodiac.Rabbit)
            {
                return -1;
            }
            else if (card1.zodiac == Zodiac.Dragon)
            {
                if (card2.zodiac == Zodiac.Rabbit)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (card1.zodiac == Zodiac.Tiger)
            {
                if (card2.zodiac == Zodiac.Horse)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
    }

    public int SortByCardNum(CardSO card1, CardSO card2)
    {
        return card1.ID.CompareTo(card2.ID);
    }


    public void UpdateDisplay()
    {
        foreach(Transform child in cardScreen.transform)
        {
            Destroy(child.gameObject);
        }
        //Debug.Log(transform.childCount);
        //remove UpdateCards line after testing
        UpdateCards();
        DisplayCards();
    }

    public void UpdateCards()
    {
        ownedCards.Clear();
        for(int i = 0; i < cardManager.cardDB.Length; i++)
        {
            if (cardManager.cardDB[i].numCopies > 0)
            {
                ownedCards.Add(cardManager.cardDB[i]);
            }
        }
    }

    public Vector2 GetPosition(int i)
    {
        return new Vector2((firstX + xSpaceBetweenItem * (i % numColumns)), (firstY + (-ySpaceBetweenItem * (i/numColumns))));
    }
}

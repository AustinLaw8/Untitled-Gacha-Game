using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInventory : MonoBehaviour
{
    //used to determine spacing
    private float firstX;
    private float firstY;
    private float xSpaceBetweenItem;
    private int numColumns;
    private float ySpaceBetweenItem;
    private int spaceBetween;
    private int sortCategory; 

    //game objects accessed in code
    [SerializeField] public bool forTeamFormation = false;
    [SerializeField] public GameObject cardSlotPrefab;
    [SerializeField] public GameObject cardScreen;
    [SerializeField] public GameObject scrollView;
    [SerializeField] public GameObject scrollyBoxContents;
    [SerializeField] public GameObject filterPanel;
    [SerializeField] public TextMeshProUGUI ascButton;

    //[SerializeField] public GameObject profilePanel;


    //Used for filter toggles
    private bool threeStarsState = true;
    private bool fourStarsState = true;
    private bool fiveStarsState = true;
    private bool sixStarsState = true;
    private bool oneCopyState = true;
    private bool twoCopiesState = true;
    private bool threeCopiesState = true;
    private bool fourCopiesState = true;
    private bool fiveCopiesState = true;
    private bool rabbitState = true;
    private bool dragonState = true;
    private bool tigerState = true;
    private bool horseState = true;
    private bool snakeState = true;
    [SerializeField] public Toggle threeStars;
    [SerializeField] public Toggle fourStars;
    [SerializeField] public Toggle fiveStars;
    [SerializeField] public Toggle sixStars;
    [SerializeField] public Toggle oneCopy;
    [SerializeField] public Toggle twoCopies;
    [SerializeField] public Toggle threeCopies;
    [SerializeField] public Toggle fourCopies;
    [SerializeField] public Toggle fiveCopies;
    [SerializeField] public Toggle rabbit;
    [SerializeField] public Toggle dragon;
    [SerializeField] public Toggle tiger;
    [SerializeField] public Toggle horse;
    [SerializeField] public Toggle snake;

    [SerializeField] public Text horseText;
    [SerializeField] public Text snakeText;
    [SerializeField] public Text sixStarText;


    private bool ascState = true;

    //List of cards to display
    public List<CardSO> ownedCards = new List<CardSO>();

    //card manager for access to all cards the player has/ possibly can have
    [SerializeField] CardManager cardManager;
    [SerializeField] TeamManager teamManager;

    public enum SortCategories
    {
        Rarity, NumberOfCopies, CardNum, Zodiac
    }

    void Awake()
    {
       if (cardManager == null) cardManager = GameObject.Find("CardData").GetComponent<CardManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bool hasSnake = false;
        bool hasHorse = false;

        Vector3[] v = new Vector3[4];

        cardScreen.GetComponent<RectTransform>().GetLocalCorners(v);


        float screenWidth = Screen.width;


        xSpaceBetweenItem = - ((v[1].x - v[2].x) / 22) * 3;
        ySpaceBetweenItem = xSpaceBetweenItem;
        spaceBetween = (int) xSpaceBetweenItem / 3;

        for(int i = 0; i < cardManager.cardDB.Length; i++)
        {
            if (cardManager.cardDB[i].numCopies > 0)
            {
                if (cardManager.cardDB[i].zodiac == Zodiac.Snake)
                {
                    hasSnake = true;
                }
                else if (cardManager.cardDB[i].zodiac == Zodiac.Horse)
                {
                    hasHorse = true;
                }
                ownedCards.Add(cardManager.cardDB[i]);
            }
        }

        if(!hasSnake)
        {
            snake.interactable = false;
            snakeText.text = "?????";
        }
        if(!hasHorse)
        {
            horse.interactable = false;
            sixStars.interactable = false;
            horseText.text = "?????";
            sixStarText.text = "?????";
        }




        scrollyBoxContents.GetComponent<GridLayoutGroup>().cellSize = new Vector2(2*(xSpaceBetweenItem/3), 2*(xSpaceBetweenItem/3));
        scrollyBoxContents.GetComponent<GridLayoutGroup>().spacing = new Vector2(spaceBetween, spaceBetween);
        scrollyBoxContents.GetComponent<GridLayoutGroup>().padding = new RectOffset(spaceBetween, spaceBetween, spaceBetween, spaceBetween);
        scrollyBoxContents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (spaceBetween * (((ownedCards.Count + 6)/7) * 3 + 1)));

        if (forTeamFormation)
        {
            teamManager = GameObject.Find("TeamData").GetComponent<TeamManager>();
            //screenView.GetComponent<RectTransform>().sizeDelta = new Vector((Screen.width/2 - 50), 25);
            xSpaceBetweenItem = - ((v[1].x - v[2].x) / 16) * 3;
            ySpaceBetweenItem = xSpaceBetweenItem;
            spaceBetween = (int) xSpaceBetweenItem / 3;
            scrollyBoxContents.GetComponent<GridLayoutGroup>().cellSize = new Vector2(2*(xSpaceBetweenItem/3), 2*(xSpaceBetweenItem/3));
            scrollyBoxContents.GetComponent<GridLayoutGroup>().spacing = new Vector2(spaceBetween, spaceBetween);
            scrollyBoxContents.GetComponent<GridLayoutGroup>().padding = new RectOffset(spaceBetween, spaceBetween, spaceBetween, spaceBetween);
        }
       
        // Debug.Log(ownedCards.Count);
        // Debug.Log(((ownedCards.Count + 6)/7));
        
        UpdateDisplay();
    }


    public void ToggleAscDesc()
    {
        ascState = !ascState;
        if(ascState)
        {
            ascButton.text = "Ascending";
        }
        else
        {
            ascButton.text = "Descending";
        }
        
        UpdateDisplay();
    }



    //
    public void SortBy(int categoryID)
    {
        Debug.Log(categoryID);
        //Category ID:
        // 0 - Rarity
        // 1 - Number of Copies
        // 2 - Card Number
        // 3 - Zodiac
        sortCategory = categoryID;
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

        UpdateDisplay();
    }

    public void FilterOpener()
    {
        threeStarsState = threeStars.isOn;
        fourStarsState = fourStars.isOn;
        fiveStarsState = fiveStars.isOn;
        sixStarsState = sixStars.isOn;
        oneCopyState = oneCopy.isOn;
        twoCopiesState = twoCopies.isOn;
        threeCopiesState = threeCopies.isOn;
        fourCopiesState = fourCopies.isOn;
        fiveCopiesState = fiveCopies.isOn;
        rabbitState = rabbit.isOn;
        dragonState = dragon.isOn;
        tigerState = tiger.isOn;
        horseState = horse.isOn;
        snakeState = snake.isOn;
        
        if (filterPanel != null)
        {
            //bool isActive = filterPanel.activeSelf;
            filterPanel.SetActive(true);
        }
    }

    public void FilterCloser()
    {
        threeStars.isOn = threeStarsState;
        fourStars.isOn = fourStarsState;
        fiveStars.isOn = fiveStarsState;
        sixStars.isOn = sixStarsState;
        oneCopy.isOn = oneCopyState;
        twoCopies.isOn = twoCopiesState;
        threeCopies.isOn = threeCopiesState;
        fourCopies.isOn = fourCopiesState;
        fiveCopies.isOn = fiveCopiesState;
        rabbit.isOn = rabbitState;
        dragon.isOn = dragonState;
        tiger.isOn = tigerState;
        horse.isOn = horseState;
        snake.isOn = snakeState;
        if (filterPanel != null)
        {
            //bool isActive = filterPanel.activeSelf;
            filterPanel.SetActive(false);
        }
    }

    //Confirm filter button
    public void ConfirmFilter()
    {
        ownedCards.Clear();
        
        CardSO cardCheck;
        //bool cardAdded = false;
        bool zodiacCorrect = false;
        bool rarityCorrect = false;
        bool numCopiesCorrect = false;
        
        for (int i = 0; i < cardManager.cardDB.Length; i++)
        {
            cardCheck = cardManager.cardDB[i];
            
            switch (cardCheck.rarity)
            {
                case Rarity.Three:
                    {
                        if (threeStars.isOn)
                        {
                            rarityCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case Rarity.Four:
                    {
                        if (fourStars.isOn)
                        {
                            rarityCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case Rarity.Five:
                    {
                        if (fiveStars.isOn)
                        {
                            rarityCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case Rarity.Six:
                    {
                        if (sixStars.isOn)
                        {
                            rarityCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
            }
            // if (!cardAdded)
            // {
            switch (cardCheck.numCopies)
            {
                case 1:
                    {
                        if (oneCopy.isOn)
                        {
                            numCopiesCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case 2:
                    {
                        if (twoCopies.isOn)
                        {
                            numCopiesCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case 3:
                    {
                        if (threeCopies.isOn)
                        {
                            numCopiesCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case 4:
                    {
                        if (fourCopies.isOn)
                        {
                            numCopiesCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case 5:
                    {
                        if (fiveCopies.isOn)
                        {
                            numCopiesCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
            }
            // }
            // if (!cardAdded)
            // {
            switch (cardCheck.zodiac)
            {
                case Zodiac.Rabbit:
                    {
                        if (rabbit.isOn)
                        {
                            zodiacCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case Zodiac.Dragon:
                    {
                        if (dragon.isOn)
                        {
                            zodiacCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case Zodiac.Tiger:
                    {
                        if (tiger.isOn)
                        {
                            zodiacCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case Zodiac.Horse:
                    {
                        if (horse.isOn)
                        {
                            zodiacCorrect = true;
                            //ownedCards.Add(cardCheck);
                        }
                        break;
                    }
                case Zodiac.Snake:
                    {
                        if (snake.isOn)
                        {
                            zodiacCorrect = true;
                        }
                        break;
                    }
                // }
            }

            if (zodiacCorrect && rarityCorrect && numCopiesCorrect)
            {
                ownedCards.Add(cardCheck);
            }
            zodiacCorrect = false;
            rarityCorrect = false;
            numCopiesCorrect = false;

        }

        SortBy(sortCategory);

        threeStarsState = threeStars.isOn;
        fourStarsState = fourStars.isOn;
        fiveStarsState = fiveStars.isOn;
        sixStarsState = sixStars.isOn;
        oneCopyState = oneCopy.isOn;
        twoCopiesState = twoCopies.isOn;
        threeCopiesState = threeCopies.isOn;
        fourCopiesState = fourCopies.isOn;
        fiveCopiesState = fiveCopies.isOn;
        rabbitState = rabbit.isOn;
        dragonState = dragon.isOn;
        tigerState = tiger.isOn;
        horseState = horse.isOn;
        
        FilterCloser();
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
                if (card2.zodiac == Zodiac.Horse || card2.zodiac == Zodiac.Snake)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (card1.zodiac == Zodiac.Snake)
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

    //adds card icons into inventory
    public void DisplayCards()
    {   
        if (ascState)
        {
            for (int i = 0; i < ownedCards.Count; i++)
            {
                var obj = Instantiate(cardSlotPrefab, Vector2.zero, Quaternion.identity, scrollyBoxContents.transform);
                obj.GetComponent<Image>().sprite = ownedCards[i].cardIcon;
                obj.GetComponent<CardIDIdentifier>().cardID = (int) ownedCards[i].ID;
                
                if(ownedCards[i].numCopies == 1)
                {
                    obj.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false;
                    obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
                else
                {
                    obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = ownedCards[i].numCopies.ToString();
                }
                    
                
                if (forTeamFormation && teamManager.InTeam(i) != -1)
                {
                    obj.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                    obj.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                    obj.transform.GetChild(2).gameObject.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                }
            }
        }
        else{
            for (int i = ownedCards.Count - 1; i >= 0; i--)
            {
                var obj = Instantiate(cardSlotPrefab, Vector2.zero, Quaternion.identity, scrollyBoxContents.transform);
                obj.GetComponent<Image>().sprite = ownedCards[i].cardIcon;
                obj.GetComponent<CardIDIdentifier>().cardID = (int) ownedCards[i].ID;

                if(ownedCards[i].numCopies == 1)
                {
                    obj.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false;
                    obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
                else
                {
                    obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = ownedCards[i].numCopies.ToString();
                }

                if (forTeamFormation && teamManager.InTeam(i) != -1)
                {
                    obj.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                    obj.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                    obj.transform.GetChild(2).gameObject.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                }
            }
        }       
    }

    public void UpdateDisplay()
    {
        foreach(Transform child in scrollyBoxContents.transform)
        {
            Destroy(child.gameObject);
        }
        
        DisplayCards();
    }

    public Vector2 GetPosition(int i)
    {
        return new Vector2((firstX + xSpaceBetweenItem * (i % numColumns)), (firstY + (-ySpaceBetweenItem * (i/numColumns))));
    }
}

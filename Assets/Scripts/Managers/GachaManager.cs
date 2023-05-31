using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.IO;

[System.Serializable]
public class PlayerSongInfo
{
    private static int NUM_SONGS = 4;
    public static string songInfoFilePath { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "playerSongInfo.json" ;} }

    public int tickets;
    public bool[] rewardsReceived = new bool[4 * 2 * NUM_SONGS];

    public static PlayerSongInfo GetPlayerSongInfo()
    {
        try {
            string songInfo = System.IO.File.ReadAllText(songInfoFilePath, System.Text.Encoding.UTF8);
            return JsonUtility.FromJson<PlayerSongInfo>(songInfo);
        } catch (FileNotFoundException) {
            return new PlayerSongInfo();
        }
    }

    public static void Write(PlayerSongInfo obj)
    {
        string writeback = JsonUtility.ToJson(obj);
        System.IO.File.WriteAllText(songInfoFilePath, writeback, System.Text.Encoding.UTF8);
    }
}

// Handles rolling and animating the roll
public class GachaManager : MonoBehaviour
{
    public static int CARD_WIDTH = 2048;
    public static int CARD_HEIGHT = 1261;
    private int currentCard;
    private RawImage cardImage;
    private int numRolls = 10;
    // array to hold the texture results of the roll
    private int[] rolls;

    [SerializeField] private ScoreToGachaSO container; 
    [SerializeField] private BeatmapSO beatmapContainer;
    // [SerializeField] private CardDBSO cardDB; 
    [SerializeField] private CardManager cardDB; 
    [SerializeField] private GameObject allResults;

    // roll rates
    // private float threeStarChance=.05f;
    private float twoStarChance = .35f;
    private float oneStarChance = .60f;

    // the game object on which the cards are displayed and animated
    [SerializeField] private GameObject card;
    private Animator cardAnimator;
    private float animationTime;
   
    public static float GetScale()
    {
        // calculates the scale about which all card pngs will scaled by, dependent on screen size
        return Screen.width > CARD_WIDTH ? 
                Mathf.Max(1.0f * Screen.width / CARD_WIDTH, 1.0f * Screen.height / CARD_HEIGHT): 
                Mathf.Min(1.0f * CARD_WIDTH / Screen.width, 1.0f * CARD_HEIGHT / Screen.height);
    }

    void Awake()
    {
        animationTime = 0f;
        cardAnimator = card.GetComponent<Animator>();
    }

    void Start()
    {
        rolls = new int[10];
        currentCard = -1;
        cardImage = card.GetComponent<RawImage>();
    }

    void Update()
    {
        animationTime += Time.deltaTime;
        if(AnimatorIsPlaying())
        {
            cardAnimator.SetFloat("time", animationTime);
        }
    }

    public void Roll(int numberOfRolls, Combo combo)
    {
        numRolls = numberOfRolls;
        SetRates(combo);

        DoRoll();

        card.SetActive(true);
        RunAnimationLoop();
    }

    public void Roll()
    {
        if (container.postGame)
        {
            Roll(10, container.combo);
            GiveTickets();
        }
        else
        {
            Roll(container.numRolls, Combo._0);
        }
    }

    // Either retrieves and animates summon for next roll, or skips the summon animation and displays the current roll
    public void RunAnimationLoop()
    {
        if (AnimatorIsPlaying())
        {
            SkipAnimatorToEnd();
        }
        else
        {
            currentCard += 1;
            if (currentCard >= numRolls)
            {
                card.SetActive(false);
                for(int i = 0; i < 10; i++)
                {
                    allResults.transform.GetChild(0).GetChild(i).gameObject.SetActive(i < numRolls);
                }
                allResults.SetActive(true);
                container.reset();
                return;
            }
            animationTime = 0f;
            cardImage.color = new Color(cardImage.color.r,cardImage.color.g,cardImage.color.b,0);
            cardImage.texture = cardDB.cardDB[rolls[currentCard]].cardArt;
            cardAnimator.Play("FadeToNext");
            cardAnimator.SetFloat("time", animationTime);
        }
    }

    void DoRoll()
    {
        float roll;
        int cardID;

        for(int i = 0; i < numRolls; i++)
        {
            roll = Random.Range(0f, 1f);
            // THE HORSE
            // if (roll >= .999)
            // {
            //     give the horse!!!
            // }
            if (roll > oneStarChance)
            {
                cardID = GetCardOfRarity(Rarity.Three);
            }
            else if (roll > twoStarChance)
            {
                cardID = GetCardOfRarity(Rarity.Four);
            }
            else
            {
                cardID = GetCardOfRarity(Rarity.Five);
            }
            cardDB.addCard(cardID);
            rolls[i] = cardID;
            allResults.transform.GetChild(0).GetChild(i).GetComponent<Image>().sprite = cardDB.cardDB[cardID].cardIcon;
        }
    }

    int GetCardOfRarity(Rarity rarity)
    {
        int infLoopCatch = 0;
        int id = Random.Range(0, cardDB.cardDB.Length);
        while (cardDB.cardDB[id].rarity != rarity)
        {
            infLoopCatch++;
            if (infLoopCatch > 999)
            {
                Debug.Log("infLoop while rolling");
                break;
            }
            id = Random.Range(0, cardDB.cardDB.Length);
        }
        return id;
    }

    // public void OnPointerDown(PointerEventData e)
    // {
    //     // insert some async stuff here about waiting for cards to be received 
    //     if (summonsDone)
    //     {
    //         RunAnimationLoop();
    //     }
    // }

    // Checks if animator is currently playing the fade animation (essentially the animation the plays between each roll)
    private bool AnimatorIsPlaying()
    {
        AnimatorStateInfo currentAnim = cardAnimator.GetCurrentAnimatorStateInfo(0);
        return currentAnim.IsName("FadeToNext") && animationTime < currentAnim.length;
    }

    // Skips the animator to the end of the animation (assumed that the fade animation is playing)
    private void SkipAnimatorToEnd()
    {
        animationTime = cardAnimator.GetCurrentAnimatorStateInfo(0).length;
        cardAnimator.SetFloat("time", animationTime);
    }

    private void SetRates(Combo combo)
    {
        switch (combo)
        {
            case Combo._0:
                // threeStarChance = .05f;
                twoStarChance = .35f;
                oneStarChance = .60f;
                break;
            case Combo._25:
                // threeStarChance = .07f;
                twoStarChance = .34f;
                oneStarChance = .59f;
                break;
            case Combo._50:
                // threeStarChance = .09f;
                twoStarChance = .33f;
                oneStarChance = .58f;
                break;
            case Combo._75:
                // threeStarChance = .11f;
                twoStarChance = .32f;
                oneStarChance = .57f;
                break;
            case Combo._100:
                // threeStarChance = .13f;
                twoStarChance = .31f;
                oneStarChance = .56f;
                break;
        }
    }

    private void GiveTickets()
    {
        PlayerSongInfo playerSongInfo = PlayerSongInfo.GetPlayerSongInfo();
        switch (container.grade)
        {
            case Grade.C:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID])
                {
                    playerSongInfo.tickets += 1;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID] = true;
                }
                break;
            case Grade.B:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID + 1])
                {
                    playerSongInfo.tickets += 3;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID + 1] = true;
                }
                break;
            case Grade.A:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID + 2])
                {
                    playerSongInfo.tickets += 5;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID + 2] = true;
                }
                break;
            case Grade.S:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID + 3])
                {
                    playerSongInfo.tickets += 10;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID + 3] = true;
                }
                break;
        }
        PlayerSongInfo.Write(playerSongInfo);
    }
}


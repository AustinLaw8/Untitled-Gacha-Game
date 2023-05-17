using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

// Handles rolling and animating the roll
public class GachaManager : MonoBehaviour, IPointerDownHandler
{
    public static int CARD_WIDTH = 2048;
    public static int CARD_HEIGHT = 1261;

    private int currentCard;
    private RawImage cardImage;
    // array to hold the texture results of the roll
    private Texture2D[] textures;
    private bool summonsDone;
    private int numRolls = 10;

    [SerializeField] private ScoreToGacha container; 

    // roll rates
    // private float threeStarChance=.05f;
    private float twoStarChance = .35f;
    private float oneStarChance = .60f;

    // the game object on which the cards are displayed and animated
    [SerializeField] private GameObject card;
    [SerializeField] private Texture2D placeholder;
    private Animator cardAnimator;
    private float animationTime;
   

    void Awake()
    {
        // calculates the scale about which all card pngs will scaled by, dependent on screen size
        float scale = Screen.width > CARD_WIDTH ? 
                Mathf.Max(1.0f * Screen.width / CARD_WIDTH, 1.0f * Screen.height / CARD_HEIGHT): 
                Mathf.Min(1.0f * CARD_WIDTH / Screen.width, 1.0f * CARD_HEIGHT / Screen.height);

        card.transform.localScale = new Vector3(scale, scale, 1f);
        animationTime = 0f;
        cardAnimator = card.GetComponent<Animator>();
    }

    void Start()
    {
        summonsDone = false;
        textures = new Texture2D[numRolls];
        for (int i = 0; i < numRolls; i++) textures[i] = placeholder;
        // for (int i = 0; i < numRolls; i++) textures[i] = new Texture2D(CARD_WIDTH,CARD_HEIGHT);
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

    // Interface for a button to tell the GachaManager to start rolling
    public void Roll(int numberOfRolls, Combo combo)
    {
        // StartCoroutine(DoRoll());
        SetRates(combo);
        numRolls = numberOfRolls;
        summonsDone = true;
        card.SetActive(true);
        RunAnimationLoop();
    }

    // Interface for a button to tell the GachaManager to start rolling
    public void Roll(int numberOfRolls)
    {
        Roll(numberOfRolls, Combo._0);
    }

    // :)
    public void PostGameRoll()
    {
        Roll(numberOfRolls, container.combo);
    }

    // Either retrieves and animates summon for next roll, or skips the summon animation and displays the current roll
    private void RunAnimationLoop()
    {
        if (AnimatorIsPlaying())
        {
            SkipAnimatorToEnd();
        }
        else
        {
            currentCard += 1;
            if (currentCard == numRolls) { Debug.Log("display all acquired cards anim"); card.SetActive(false); }
            animationTime = 0f;
            cardImage.color = new Color(cardImage.color.r,cardImage.color.g,cardImage.color.b,0);
            cardImage.texture = textures[currentCard];
            cardAnimator.Play("FadeToNext");
            cardAnimator.SetFloat("time", animationTime);
        }
    }

    // TODO: Make this async, with loading screen or animation or something to allow the async to run
    IEnumerator DoRoll()
    {
        float roll;
        int id;

        for(int i = 0; i < numRolls; i++)
        {
            roll = Random.Range(0f, 1f);
            if (roll > oneStarChance)
            {
                id = 0;
            }
            else if (roll > twoStarChance)
            {
                id = 1;
            }
            else
            {
                id = 2;
            }
            /* insert code here to add a card to players inventory */ 
            yield return GetCard(id, i);
            // Debug.Log($"acquired data for {i}");
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        // insert some async stuff here about waiting for cards to be received 
        if (summonsDone)
        {
            RunAnimationLoop();
        }
    }

    /* GetCard(int) is acting as sort of interface so that we can change this later down the line if necessary */
    IEnumerator GetCard(int id, int i)
    {
        yield return StartCoroutine(GetRequest("https://austinlaw8.github.io/test_art.png", res => ImageConversion.LoadImage(textures[i], res) ));
    }

    // Conducts a get request to a URL and takes the resulting png and loads it into a texture.
    IEnumerator GetRequest(string url, System.Action<byte[]> lambda)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();


            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    yield break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    lambda(null);
                    yield break;
                case UnityWebRequest.Result.Success:
                    lambda(webRequest.downloadHandler.data);
                    yield break;
            }
        }
    }

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
}


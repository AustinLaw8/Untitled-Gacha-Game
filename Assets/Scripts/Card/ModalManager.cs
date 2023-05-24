using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModalManager : MonoBehaviour
{
    public static ModalManager instance;

    [SerializeField] CardInventory cardInventory;
    [SerializeField] CardManager cardManager;
    
    [Header("Modal Sections")]
    [SerializeField] GameObject modalWindow;
    [SerializeField] TextMeshProUGUI cardTitle;
    [SerializeField] TextMeshProUGUI cardZodiac;
    [SerializeField] RawImage cardImg;
    [SerializeField] TextMeshProUGUI cardArtist;
    [SerializeField] RawImage enlargeImage;

    private CardSO card;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (cardInventory == null) cardInventory = GameObject.Find("CardData").GetComponent<CardInventory>();
        if (cardManager == null) cardManager = GameObject.Find("CardData").GetComponent<CardManager>();
    }

    public void ShowModal(int cardID)
    {
        card = cardManager.cardDB[cardID];
        cardTitle.text = card.title;
        cardZodiac.text = card.zodiac.ToString();
        cardImg.texture = card.cardArt;
        cardArtist.text = "Art by " + card.artist;
        
        modalWindow.SetActive(true);
    }

    public void HideModal()
    {
        modalWindow.SetActive(false);
    }

    public void ShowImage()
    {
        float scale = GachaManager.GetScale();
        enlargeImage.texture = card.cardArt;
        enlargeImage.transform.localScale = new Vector3(scale, scale, 1f);
        enlargeImage.gameObject.SetActive(true);
    }
}

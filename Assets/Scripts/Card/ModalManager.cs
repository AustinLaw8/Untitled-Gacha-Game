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

    public bool Active()
    {
        return modalWindow.activeSelf;
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
        enlargeImage.texture = card.cardArt;
        enlargeImage.gameObject.SetActive(true);
    }
}

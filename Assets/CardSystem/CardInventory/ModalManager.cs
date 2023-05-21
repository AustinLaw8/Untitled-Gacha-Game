using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModalManager : MonoBehaviour
{
    public GameObject modalWindow;
    //public TextMeshProUGUI header;
 
    public GameObject imageWindow;
    [SerializeField] CardManager cardManager;
    [SerializeField] RawImage cardImg;
    [SerializeField] RawImage enlargeImage;
    public TextMeshProUGUI cardHeader;
    public TextMeshProUGUI cardDesc;
    private CardSO card;

    //public GameObject body;

    public static ModalManager instance;

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
    }

    public void ShowModal(int cardID)
    {
        card = cardManager.cardDB[cardID];
        cardImg.texture = card.cardArt;
        cardHeader.text = string.Concat(card.zodiac.ToString(), ", ", card.title);
        cardDesc.text = string.Concat("Title: ", card.title, "\n", "Artist: ", card.artist, "\n", "Zodiac: ", card.zodiac.ToString(), "\n", "Rarity: ", card.rarity.ToString(), "\n");

        modalWindow.SetActive(true);
    }

    public void HideModal()
    {
        modalWindow.SetActive(false);
    }

    public void ShowImage(int cardID)
    {
        enlargeImage.texture = card.cardArt;

        imageWindow.SetActive(true);
    }

    public void HideImage()
    {
        imageWindow.SetActive(false);
    }
}

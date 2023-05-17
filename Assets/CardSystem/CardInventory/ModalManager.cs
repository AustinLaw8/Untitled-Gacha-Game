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

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

    [SerializeField] public GameObject cardSlotPrefab;
    [SerializeField] public GameObject cardScreen;

    public List<CardSO> ownedCards = new List<CardSO>();

    [SerializeField] CardManager cardManager;

    // Start is called before the first frame update
    void Start()
    {
        
        Vector3[] v = new Vector3[4];
        cardScreen.GetComponent<RectTransform>().GetLocalCorners(v);

        xSpaceBetweenItem = - ((v[1].x - v[2].x) / 22) * 3;
        ySpaceBetweenItem = xSpaceBetweenItem;

        firstX = v[1].x + (xSpaceBetweenItem * 2 / 3);
        firstY = (v[1].y) - (xSpaceBetweenItem * 2/ 3);

        //firstY = cardScreen.GetComponent<RectTransform>().offsetMin.y;
        for(int i = 0; i < cardManager.cardDB.Length; i++)
        {
            if (cardManager.cardDB[i].numCopies > 0)
            {
                ownedCards.Add(cardManager.cardDB[i]);
            }
        }
        DisplayCards();
    }

    public void DisplayCards()
    {
        
        for (int i = 0; i < ownedCards.Count; i++)
        {
            var obj = Instantiate(cardSlotPrefab, Vector2.zero, Quaternion.identity, cardScreen.transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponent<Image>().sprite = ownedCards[i].cardIcon;
            
        }
    }   

    public Vector2 GetPosition(int i)
    {
        return new Vector2((firstX + xSpaceBetweenItem * (i % numColumns)), (firstY + (-ySpaceBetweenItem * (i/numColumns))));
    }
}

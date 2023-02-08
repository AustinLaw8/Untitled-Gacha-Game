using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Rarity
{
    One, Two, Three
}

public class CardSO : ScriptableObject
{
    public static int CARD_WIDTH = 2048;
    public static int CARD_HEIGHT = 1261;

    public Image cardArt;
    public string artist;
    public Rarity rarity;
}

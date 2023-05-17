using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public enum Rarity
{
    Three, Four, Five, Six
}

public enum Zodiac
{
    Rabbit, Dragon, Tiger, Horse
}

/* It is not set in stone if we are going to have assets be stored here like this, or if they will be online*/
/* In either case, this can act as a place holder so we know what we need */
[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardSO : ScriptableObject
{
    public uint ID;
    public Texture2D cardArt;
    public Sprite cardIcon;
    public string artist;
    public Rarity rarity;
    public Zodiac zodiac;
    public int power;
    public int numCopies;
    public string title;
}

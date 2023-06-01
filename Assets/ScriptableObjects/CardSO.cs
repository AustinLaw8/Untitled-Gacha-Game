using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    Three, Four, Five, Six
}

public enum Zodiac
{
    Rabbit, Dragon, Tiger, Horse, Snake
}

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

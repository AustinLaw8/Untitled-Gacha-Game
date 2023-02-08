using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public enum Rarity
{
    One, Two, Three
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardSO : ScriptableObject
{

    public uint ID;
    public Texture2D cardArt;
    public string artist;
    public Rarity rarity;
}

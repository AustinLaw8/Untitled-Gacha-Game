using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public enum Rarity
{
    One, Two, Three
}

/* It is not set in stone if we are going to have assets be stored here like this, or if they will be online*/
/* In either case, this can act as a place holder so we know what we need */
[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardSO : ScriptableObject
{
    public uint ID;
    public Texture2D cardArt;
    public string artist;
    public Rarity rarity;

    // A card's "pivot" is the location about which its icon and character slice will be framed
    public Vector2 pivot;
}

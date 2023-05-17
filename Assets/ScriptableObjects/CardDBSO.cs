using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDBSO", menuName = "ScriptableObjects/CardDB", order = 1)]
public class CardDBSO : ScriptableObject
{
    public CardSO[] cardDB;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreToGacha", menuName = "ScriptableObjects/ScoreToGacha", order = 1)]
public class ScoreToGacha : ScriptableObject
{
    //Allow Gacha Manager to access the combo percent and total score
    public Combo combo;
    public float score;
}

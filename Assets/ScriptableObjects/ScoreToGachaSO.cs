using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreToGacha", menuName = "ScriptableObjects/ScoreToGacha", order = 1)]
public class ScoreToGachaSO : ScriptableObject
{
    //Allow Gacha Manager to access the combo percent and total score
    public Combo combo;
    public float score;
    public Grade grade;
    public bool postGame;
    public int numRolls;

    public void reset()
    {
        combo = Combo._0;
        score = 0;
        grade = Grade.C;
        postGame = false;
        numRolls = 0;
    }
}

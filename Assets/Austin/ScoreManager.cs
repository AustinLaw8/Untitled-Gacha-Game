using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set;  }
    public float Score { get; private set; }
    public int Combo { get; private set; }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public float GetScore()
    {
        return Score;
    }

    public void IncScore(int deltaScore)
    {
        Score += deltaScore;
    }

    public void ResetScore()
    {
        Score = 0;
    }

    public int GetCombo()
    {
        return Combo;
    }

    public void IncCombo()
    {
        Combo++;
    }

    public void ResetCombo()
    {
        Combo = 0;
    }

    public float GetComboMultiplier(int curCombo)
    {
        // TODO add combo multipliers
        switch (curCombo)
        { 
            case 10:
                //break;
            case 20:
                //break;
            default:
                break;
        }
        // placeholder number
        return 1;
    }
}

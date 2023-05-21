using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Accuracy
{
    Perfect, Great, Good, Bad, Miss
}
public enum Grade
{
    S, A, B, C
}
public enum Combo
{
    _0, _25, _50, _75, _100
}
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager scoreManager { get; private set;  }

    private static float BREAKPOINT_C=10f;
    private static float BREAKPOINT_B=200f;
    private static float BREAKPOINT_A=400f;
    private static float BREAKPOINT_S=1000f;

    private float score;
    private int combo;
    private int mapBaseScore = 1;

    /* [SerializeField] private ScoreToGachaSO container; */

    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image letter;

    [SerializeField] private Sprite S;
    [SerializeField] private Sprite A;
    [SerializeField] private Sprite B;
    [SerializeField] private Sprite C;
    [SerializeField] private Sprite D;

    void Awake()
    {
        if (scoreManager != null && scoreManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            scoreManager = this;
        }
    }

    void Start()
    {
        score = 0;
        combo = 0;
    }

    public void IncreaseScore(Accuracy accuracy)
    {
        float accuracyMultiplier = GetAccuracyMultiplierAndUpdateCombo(accuracy);

        int baseScore = 
            // Some value based on team
            + mapBaseScore;

        float comboMultiplier = GetComboMultiplier(combo);

        // TODO: conduct full score calcs
        int deltaScore = (int)Mathf.Ceil(baseScore * comboMultiplier * accuracyMultiplier);

        UpdateScore(deltaScore);
    }

    public void GiveHoldPoints()
    {
        float deltaScore = 0;
        UpdateScore(deltaScore);
    }

    private void UpdateScore(float deltaScore)
    {
        score += deltaScore;
        if (score <= BREAKPOINT_C)
        {
            letter.sprite = D;
        }
        else if (score <= BREAKPOINT_B)
        {
            letter.sprite = C;
        }
        else if (score <= BREAKPOINT_A)
        {
            letter.sprite = B;
        }
        else if (score <= BREAKPOINT_S)
        {
            letter.sprite = A;
        }
        else
        {
            letter.sprite = S;
        }
        slider.value = score/BREAKPOINT_S;
        fill.color = gradient.Evaluate(1-slider.value);
    }
    // Updates combo based on accuracy
    // Returns the score multiplier for a given accuracy
    private float GetAccuracyMultiplierAndUpdateCombo(Accuracy accuracy)
    {
        // TODO: Set all these values to their actual values, and determine what reset combos.
        switch (accuracy)
        { 
            case Accuracy.Perfect:
                combo += 1;
                return 5;
            case Accuracy.Great:
                combo += 1;
                return 4;
            case Accuracy.Good:
                combo = 0;
                return 3;
            case Accuracy.Bad:
                combo = 0;
                return 2;
            case Accuracy.Miss:
                combo = 0;
                return 0;
            default:
                return 0;
        }
    }

    // Returns the score muliplier for a given accuracy
    // TODO: determine actual values and formula
    private float GetComboMultiplier(int curCombo)
    {
        switch (curCombo)
        { 
            case 10:
                //break;
            case 20:
                //break;
            default:
                break;
        }
        return 1.0f + GetComboSkillMultiplier(0, 0, 0);
    }

    private float GetComboSkillMultiplier(int numOneStarTigers, int numTwoStarTigers, int numThreeStarTigers)
    {
        return numOneStarTigers * 0.03f + numTwoStarTigers * 0.05f + numThreeStarTigers * 0.10f;
    }

    /* Various getters/setters */
    public float GetScore() { return score; }
    public float GetCombo() { return combo; }
    public void ResetScore() { score = 0; }
    public void ResetCombo() { combo = 0; }
}

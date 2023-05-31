using System;
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
    S, A, B, C, D
}
public enum Combo
{
    _0, _25, _50, _75, _100
}
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager scoreManager { get; private set; }

    private static float BREAKPOINT_C=500_000f;
    private static float BREAKPOINT_B=750_000f;
    private static float BREAKPOINT_A=900_000f;
    private static float BREAKPOINT_S=1_000_000f;

    private float baseScore;

    private float score;
    private Grade grade;
    private int combo;
    private int maxCombo;

    [SerializeField] private ScoreToGachaSO container;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI plusScoreText;
    [SerializeField] private Animator scoreAnimator;

    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image letter;

    [SerializeField] private Sprite S;
    [SerializeField] private Sprite A;
    [SerializeField] private Sprite B;
    [SerializeField] private Sprite C;
    [SerializeField] private Sprite D;

    [SerializeField] private GameObject postGameCanvas;
    [SerializeField] private TextMeshProUGUI postGameHeader;
    [SerializeField] private Transform postGameRewards;
    [SerializeField] private TextMeshProUGUI postGameScoreText;
    [SerializeField] private Image postGameScoreLetter;

    [SerializeField] private Color blue;
    [SerializeField] private Color purple;

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
        int[] teamIDs = TeamManager.GetTeam();

        int power = 0;
        foreach(int i in teamIDs)
        {
            power += CardManager.cardManager.cardDB[i].power;
        }
        float scoreFromPower = power / 1000f;
        int mapDifficultyBonus = BeatManager.beatManager.container.ID % 2 == 0 ? 1 : 0;
        baseScore = scoreFromPower + BREAKPOINT_C / BeatManager.beatManager.NumNotes*1f + mapDifficultyBonus;
        
        score = 0;
        combo = 0;
        maxCombo = 0;
        scoreText.text = "0";
        grade = Grade.D;
    }

    public void IncreaseScore(Accuracy accuracy)
    {
        float accuracyMultiplier = GetAccuracyMultiplierAndUpdateCombo(accuracy);
        float comboMultiplier = GetComboMultiplier(combo);
        float deltaScore = baseScore * comboMultiplier * accuracyMultiplier;
        UpdateScore(deltaScore);
    }

    public void GiveHoldPoints()
    {
        float deltaScore = baseScore / 10f;
        combo+=1;
        UpdateScore(deltaScore);
    }

    // Increases score and score bar
    private void UpdateScore(float deltaScore)
    {
        score += deltaScore;
        if (score <= BREAKPOINT_C)
        {
            letter.sprite = D;
        }
        else if (score <= BREAKPOINT_B)
        {
            grade = Grade.C;
            letter.sprite = C;
        }
        else if (score <= BREAKPOINT_A)
        {
            grade = Grade.B;
            letter.sprite = B;
        }
        else if (score <= BREAKPOINT_S)
        {
            grade = Grade.A;
            letter.sprite = A;
        }
        else
        {
            grade = Grade.S;
            letter.sprite = S;
        }
        if (!Mathf.Approximately(deltaScore, 0f))
        {
            plusScoreText.text = $"+{Mathf.Round(deltaScore)}";
            scoreAnimator.Play("Fade");
        }
        scoreText.text = $"{Mathf.Round(score)}";
        slider.value = 1f* score/BREAKPOINT_S;
        fill.color = gradient.Evaluate(Mathf.Min(1, 1-slider.value));
    }
    
    /**
     * Updates combo based on accuracy, and returns the score multiplier for a given accuracy
     * Perfect: 100% 
     * Great: 90%
     * Good: 75%
     * Bad: 50%
     * Miss: 0%
     */
    private float GetAccuracyMultiplierAndUpdateCombo(Accuracy accuracy)
    {
        switch (accuracy)
        { 
            case Accuracy.Perfect:
            case Accuracy.Great:
            case Accuracy.Good:
                combo += 1;
                if(combo > 0 && combo % 25 == 0)
                {
                    UpdateScore(SkillManager.skillManager.flatScoreBonus);
                    SkillManager.skillManager.AnimateSkill(Zodiac.Dragon);
                }
                break;
            case Accuracy.Bad:
            case Accuracy.Miss:
            default:
                maxCombo = Math.Max(maxCombo,  combo);
                combo = 0;
                break;
        }

        switch (accuracy)
        { 
            case Accuracy.Perfect:
                return 1f;
            case Accuracy.Great:
                return .9f;
            case Accuracy.Good:
                return .75f;
            case Accuracy.Bad:
                return .5f;
            case Accuracy.Miss:
            default:
                return 0f;
        }
    }
    
    // Returns the score muliplier for a given accuracy
    // Returns an extra 5% per 50 combo
    private float GetComboMultiplier(int curCombo)
    {
        return Mathf.Floor(curCombo / 50f) * .05f * SkillManager.skillManager.comboMultiplierBonus + 1f;
    }

    // Sets the ScoreToGacha Container
    // Needed to know rates, number of rolls, tickets, and set PlayerSongInfo
    public void OnEndGame()
    {
        PlayerSongInfo playerSongInfo = PlayerSongInfo.GetPlayerSongInfo();
        container.score = score;
        container.grade = grade;
        container.postGame = true;
        if (maxCombo < (BeatManager.beatManager.NumNotes * 0.25f))
        {
            container.combo = Combo._0;
        }
        else if (maxCombo < (BeatManager.beatManager.NumNotes * 0.5f))
        {
            container.combo = Combo._25;
        }
        else if (maxCombo < (BeatManager.beatManager.NumNotes * 0.75f))
        {
            container.combo = Combo._50;
        }
        else if (maxCombo < (BeatManager.beatManager.NumNotes * 1.00f))
        {
            container.combo = Combo._75;
        }
        else
        {
            container.combo = Combo._100;
        }
        BeatmapSO beatmap = BeatManager.beatManager.container;
        string difficulty = beatmap.ID % 8 < 4 ? "easy" : "hard";
        float percentCombo = Mathf.Round(100f * maxCombo / BeatManager.beatManager.NumNotes);
        GiveTickets(playerSongInfo, beatmap);
        postGameHeader.text = $"Passed - {beatmap.songName} ({difficulty})";
        for (int i = 0; i < 4; i++)
        {
            if (playerSongInfo.rewardsReceived[beatmap.ID + i])
            {
                postGameRewards.GetChild(i).GetComponent<Image>().color = blue;
            }
            else if (maxCombo >= Mathf.FloorToInt(BeatManager.beatManager.NumNotes * .25f * (i + 1)))
            {
                postGameRewards.GetChild(i).GetComponent<Animator>().Play("TurnBlue");
                playerSongInfo.rewardsReceived[beatmap.ID + i] = true;
            }
            else
            {
                postGameRewards.GetChild(i).GetComponent<Image>().color = purple;
            }
        }
        if (Mathf.Approximately(percentCombo, 100f)) postGameScoreText.text = $"Final Score: {Mathf.Round(score)}\nHighest Combo: {maxCombo} (Full Combo!)";
        else postGameScoreText.text = $"Final Score: {Mathf.Round(score)}\nHighest Combo: {maxCombo} ({percentCombo.ToString()}%)";
        postGameScoreLetter.sprite = letter.sprite;
        postGameCanvas.SetActive(true);
        PlayerSongInfo.Write(playerSongInfo);
    }

    private void GiveTickets(PlayerSongInfo playerSongInfo, BeatmapSO beatmapContainer)
    {
        switch (container.grade)
        {
            case Grade.S:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID + 3])
                {
                    playerSongInfo.tickets += 10;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID + 3] = true;
                }
                goto case Grade.A;
            case Grade.A:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID + 2])
                {
                    playerSongInfo.tickets += 5;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID + 2] = true;
                }
                goto case Grade.B;
            case Grade.B:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID + 1])
                {
                    playerSongInfo.tickets += 3;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID + 1] = true;
                }
                goto case Grade.C;
            case Grade.C:
                if (!playerSongInfo.rewardsReceived[beatmapContainer.ID])
                {
                    playerSongInfo.tickets += 1;
                    playerSongInfo.rewardsReceived[beatmapContainer.ID] = true;
                }
                break;
        }
    }

    // Various getters/setters 
    public float GetScore() { return score; }
    public float GetCombo() { return combo; }
    public void ResetScore() { score = 0; }
    public void ResetCombo() { combo = 0; }
    public void ResetMaxCombo() { maxCombo = 0; }
}

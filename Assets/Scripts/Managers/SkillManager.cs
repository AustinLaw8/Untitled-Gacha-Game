using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillManager : MonoBehaviour
{
    public static SkillManager skillManager { get; private set; }
    public static float baseScoreBonus=500f;
    public static float baseComboBonus=1.0f;
    public static int baseHealAmount=1;

    private float flatScoreBonus_;
    public float flatScoreBonus { get { return flatScoreBonus_; } }

    private float comboMultiplierBonus_;
    public float comboMultiplierBonus { get { return comboMultiplierBonus_; } }

    private int healAmount_;
    public int healAmount { get { return healAmount_; } }

    private int[] teamIDs;
    private int ind;

    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private Animator skillAnimator;

    void Awake()
    {
        if (skillManager != null && skillManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            skillManager = this;
        }
    }

    public void AnimateSkill(Zodiac zodiac)
    {
        string str;
        switch (zodiac)
        {
            case Zodiac.Tiger:
                str = "Tiger";
                break;
            case Zodiac.Rabbit:
                str = "Rabbit";
                break;
            case Zodiac.Dragon:
            default:
                str = "Dragon";
                break;
        }
        skillText.text = $"{str} Skill Activated";
        skillAnimator.Play("Fade");
    }

    void Start()
    {
        teamIDs = TeamManager.GetTeam();

        flatScoreBonus_ = baseScoreBonus * GetNumOfZodiac(teamIDs, Zodiac.Dragon);
        comboMultiplierBonus_ = baseComboBonus + GetNumOfZodiac(teamIDs, Zodiac.Tiger)/100f;
        healAmount_ = baseHealAmount * GetNumOfZodiac(teamIDs, Zodiac.Rabbit);
        if (comboMultiplierBonus_ > 0)
        {
            AnimateSkill(Zodiac.Tiger);
        }
    }

    int GetNumOfZodiac(int[] teamIDs, Zodiac zodiac)
    {
        int count = 0;
        foreach (int id in teamIDs)
        {
            CardSO card = CardManager.cardManager.cardDB[id];
            if(zodiac == card.zodiac)
            {
                switch(card.rarity)
                {
                    case Rarity.Three:
                        count++;
                        break;
                    case Rarity.Four:
                        count += 2;
                        break;
                    case Rarity.Five:
                    case Rarity.Six:
                        count += 4;
                        break;
                }
            }
      }
      return count;
    }
}

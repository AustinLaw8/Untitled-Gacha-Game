using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public static int BAD_NOTE_AMOUNT = 3;
    public static int MISS_NOTE_AMOUNT = 5;
    public static int HOLD_MISS_NOTE_AMOUNT = 1;

    public static HealthManager healthManager;

    [SerializeField] private Slider healthBar;
    [SerializeField] private int lowHealth = 20;
    [SerializeField] private int medHealth = 40;

    [SerializeField] private Gradient gradient;
    [SerializeField] private Image hpHeart;
    [SerializeField] private Sprite highHealthHeart;
    [SerializeField] private Sprite medHealthHeart;
    [SerializeField] private Sprite lowHealthHeart;
    [SerializeField] private Color ded;

    [SerializeField] private Image hpBarBackground;
    [SerializeField] private TextMeshProUGUI health;

    [SerializeField] private int startingHP = 100;

    private int currentHealth;

    void Awake()
    {
        if (healthManager != null && healthManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            healthManager = this;
        }
    }

    void Start()
    {
        if (healthBar == null) healthBar = GetComponent<Slider>();
        healthBar.maxValue = startingHP;
        currentHealth = startingHP;
        SetHealthSlider();
        ColorCheck();
    }

    public void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        ColorCheck();
        SetHealthSlider();
    }

    public void DecreaseHealth(int amount)
    {
        currentHealth -= amount;
        ColorCheck();
        if (currentHealth <= 0)
        {
            BeatManager.beatManager.EndGame();
            currentHealth = 0;
        }
        SetHealthSlider();        
    }

    private void SetHealthSlider()
    {
        if (currentHealth >= startingHP)
        {
            healthBar.value = startingHP;
        }
        else
        {
            healthBar.value = currentHealth;
        }
        health.text = currentHealth.ToString();
    }

    private void ColorCheck()
    {
        if(currentHealth <= 0)
        {
            hpBarBackground.color = ded;
            health.color = gradient.Evaluate(0);
        }
        else
        {
            hpBarBackground.color = gradient.Evaluate((float)currentHealth/startingHP);
            health.color = gradient.Evaluate(currentHealth/startingHP);
        }
        
        if(currentHealth <= 0)
        {
            hpHeart.sprite = lowHealthHeart;
        }
        else if(currentHealth <= lowHealth)
        {
            hpHeart.sprite = lowHealthHeart;
        }
        else if (currentHealth <= medHealth)
        {
            hpHeart.sprite = medHealthHeart;
        }
        else
        {
            hpHeart.sprite = highHealthHeart;
        }
    }
}

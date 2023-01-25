using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{
    private static int BAD_NOTE_AMOUNT = 3;
    private static int MISS_NOTE_AMOUNT = 5;

    [SerializeField] private Slider healthBar;
    [SerializeField] private int lowHealth = 20;
    [SerializeField] private int medHealth = 40;

    [SerializeField] private Color lowHealthColor;
    [SerializeField] private Color medHealthColor;
    [SerializeField] private Color highHealthColor;
    [SerializeField] private Color ded;

    [SerializeField] private Image hpBarBackground;


    [SerializeField] private int startingHP = 100;

    [SerializeField] private TextMeshProUGUI health;
    private int currentHealth;

    void Start()
    {
        //healthBar = GameObject.GetComponent<Slider>();
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
            EndGame();
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
            health.color = lowHealthColor;
        }
        else if(currentHealth <= lowHealth)
        {
            hpBarBackground.color = lowHealthColor;
            health.color = lowHealthColor;
        }
        else if (currentHealth <= medHealth)
        {
            hpBarBackground.color = medHealthColor;
            health.color = medHealthColor;
        }
        else
        {
            hpBarBackground.color = highHealthColor;
            health.color = highHealthColor;
        }
    }

    private void EndGame()
    {
        EventManager.Broadcast(Events.DeathEvent);
    }
}

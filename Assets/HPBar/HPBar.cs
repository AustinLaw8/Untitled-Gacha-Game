using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{

    [SerializeField] private Slider healthBar;
    [SerializeField] private int lowHealth = 20;
    [SerializeField] private int medHealth = 40;

    [SerializeField] private Color lowHealthColor;
    [SerializeField] private Color medHealthColor;
    [SerializeField] private Color highHealthColor;
    [SerializeField] private Color ded;

    [SerializeField] private Image hpBarBackground;


    [SerializeField] private int healthRecoveryAmount = 5;
    [SerializeField] private int badNoteAmount = 3;
    [SerializeField] private int missNoteAmount = 5;

    [SerializeField] private int startingHP = 100;

    [SerializeField] private TextMeshProUGUI health;
    private int currentHealth;

    void Start()
    {
        //healthBar = GameObject.GetComponent<Slider>();
        healthBar.maxValue = startingHP;
        currentHealth = startingHP;
        setHealthSlider();
        colorCheck();
    }


    public void addHealth()
    {
        currentHealth += healthRecoveryAmount;
        colorCheck();
        setHealthSlider();
    }

    public void badNote()
    {
        currentHealth -= badNoteAmount;
        colorCheck();
        if (currentHealth <= 0)
        {
            EndGame();
            currentHealth = 0;
        }
        setHealthSlider();        
    }

    public void missNote()
    {
        currentHealth -= missNoteAmount;
        colorCheck();
        if (currentHealth <= 0)
        {
            EndGame();
            currentHealth = 0;
        }
        setHealthSlider();
    }


    private void setHealthSlider()
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


    private void colorCheck()
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
        Debug.Log("Oh rip, better luck next time lmao");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TicketHandler : MonoBehaviour
{
    [SerializeField] private ScoreToGachaSO scoreToGacha;
    [SerializeField] private TextMeshProUGUI ticketsText;

    private int numTickets;
    private PlayerSongInfo playerSongInfo;

    void Start()
    {
        playerSongInfo = PlayerSongInfo.GetPlayerSongInfo();
        numTickets = playerSongInfo.tickets;
        ticketsText.text = $"Gacha (x{numTickets})";
    }

    public void UseTickets()
    {
        int numRolls = Math.Min(numTickets, 10);

        scoreToGacha.postGame = false;
        scoreToGacha.numRolls = numRolls;
        numTickets -= numRolls;
        PlayerSongInfo.Write(playerSongInfo);
        SceneManager.LoadScene("GachaScreen", LoadSceneMode.Single);
    }
}

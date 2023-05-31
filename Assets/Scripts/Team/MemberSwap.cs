using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberSwap : MonoBehaviour
{
    [SerializeField] TeamManager teamManager;
    private static int teamID=3900;
    private static int swapInd=-1;
    private static int teamPos;
    [SerializeField] private CardInventory cardInventory;
    [SerializeField] private TeamInventorySelected scriptInv;
    [SerializeField] private TeamInventory teamInventory;
    [SerializeField] private selectedCard scriptTeam;

    public void SetSwap(int ind)
    {
        swapInd = ind;
    }

    public void SetTeamId(int id)
    {
        teamID = id;
    }

    public void SetTeamPos(int pos)
    {
        teamPos = pos;
    }

    public void Confirmed()
    {
        if (swapInd == -1)
        {
            teamManager.teamIDs[teamPos] = teamID;
            teamID = 3900;
        }
        else
        {
            teamManager.teamIDs[swapInd] = teamManager.teamIDs[teamPos];
            teamManager.teamIDs[teamPos] = teamID;
            swapInd = -1;
        }
        teamInventory.UpdateCards();
        scriptInv.ClearImage();
        scriptTeam.ClearImage();
        cardInventory.UpdateDisplay();
        teamManager.SaveTeam();
        teamID = 3900;

    }
}

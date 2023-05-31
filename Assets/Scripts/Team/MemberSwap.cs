using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberSwap : MonoBehaviour
{
    [SerializeField] TeamManager teamManager;
    private static int teamID=3900;
    private static int teamPos;
    [SerializeField] private CardInventory cardInventory;
    [SerializeField] private TeamInventorySelected scriptInv;
    [SerializeField] private TeamInventory teamInventory;
    [SerializeField] private selectedCard scriptTeam;

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
        teamManager.teamIDs[teamPos] = teamID;
        teamInventory.UpdateCards();
        scriptInv.ClearImage();
        scriptTeam.ClearImage();
        cardInventory.UpdateDisplay();
        teamManager.SaveTeam();
        teamID = 3900;
    }
}

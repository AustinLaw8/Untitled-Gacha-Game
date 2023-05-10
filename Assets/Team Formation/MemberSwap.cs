using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberSwap : MonoBehaviour
{
    // TODO - swap selected member with current team member
    [SerializeField] TeamManager teamManager;
    //[SerializeField] CardManager cardManager;
    private static int invID;
    private static int teamID;
    private static int teamPos;
    [SerializeField] CardInventory script;
    [SerializeField] TeamInventorySelected scriptInv;
    [SerializeField] selectedCard scriptTeam;

    void Awake()
    {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(delegate () { Confirmed(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInvID(int id)
    {
        invID = id;
        Debug.Log("team inventory " + invID);
    }

    public void SetTeamID(int id)
    {
        teamID = id;
        Debug.Log("team " + teamID);
    }

    public void SetTeamPos(int pos)
    {
        teamPos = pos;
    }


    private void Confirmed()
    {
        Debug.Log("clicked");
        Debug.Log("team " + teamID + " inv "+ invID);
        // take the two IDs and then swap their cards in the respective arrays

        for (int i = 0; i < teamManager.teamInvIDs.Count; i++)
        {
            for (int j = 0; j < teamManager.teamIDs.Length; j++)
            {
               //print(teamManager.cardDB[j].ID + " " + invManager.cardDB[i].ID);
                if (teamManager.teamIDs[j] == teamID && teamManager.teamInvIDs[i] == invID)
                {
                    teamManager.teamInvIDs[i] = teamID;
                    Debug.Log("should be 3900 "+ teamManager.teamInvIDs[i] + " " + teamID);
                    teamManager.teamIDs[teamPos] = invID;
                    Debug.Log("attempted swap");
                    break;
                }
            }
        }
        //script = FindObjectOfType<CardInventory>();
        script.UpdateCards();
        scriptInv.ClearImage();
        scriptTeam.ClearImage();
    }

}

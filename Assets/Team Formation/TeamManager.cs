using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TeamManager : MonoBehaviour
{
    [SerializeField] CardManager cardManager;
    public int[] teamIDs;
    public List<int> teamInvIDs = new List<int>();
    [SerializeField] CardInventory script; 

    public void Awake()
    {
        //LoadCards();
        //Load cards in here
    }

    public static int[] FromJson(string json)
    {
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
        return wrapper.Items;
    }

    public static string ToJson(int[] array)
    {
        Wrapper wrapper = new Wrapper();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper
    {
        public int[] Items;
    }


    string filepath2 { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "teamCards.json"; } }

    // 3900 means empty slot
    public void SaveTeam(int cardID1 = 3900, int cardID2 = 3900, int cardID3 = 3900, int cardID4 = 3900, int cardID5 = 3900)
    {
        int[] teamIDs = new int[5];
        teamIDs[0] = cardID1;
        teamIDs[1] = cardID2;
        teamIDs[2] = cardID3;
        teamIDs[3] = cardID4;
        teamIDs[4] = cardID5;
        var teamData = JsonUtility.ToJson(teamIDs);
        //Debug.Log(cardData);
        System.IO.File.WriteAllText(filepath2, teamData, System.Text.Encoding.UTF8);
        //Debug.Log(cardData);
    }

    public void LoadTeam()
    {
        teamIDs = new int[5];
        teamInvIDs.Clear();
        var loadedTeam = System.IO.File.ReadAllText(filepath2, System.Text.Encoding.UTF8);
        teamIDs = FromJson(loadedTeam);

        // add the IDs of the non-team cards to the team inventory
        int index = 0;
        bool onTeam = false;
        for (int i = 0; i<cardManager.cardDB.Length; i++)
        {
            for (int j = 0; j<5; j++)
            {
                if (cardManager.cardDB[i].ID == teamIDs[j])
                    onTeam = true;
            }
            if (!onTeam)
            {
                teamInvIDs.Add((int)cardManager.cardDB[i].ID);
                index++;
            }

        }
    }

    public void ClearTeam()
    {
        for (int i = 0; i<teamIDs.Length; i++)
        {
            teamIDs[i] = 3900;
        }

        teamInvIDs.Clear();

        // add the IDs of the non-team cards to the team inventory
        int index = 0;
        bool onTeam = false;
        for (int i = 0; i < cardManager.cardDB.Length; i++)
        {
            teamInvIDs.Add((int)cardManager.cardDB[i].ID);
        }

        for (int i = 0; i<teamInvIDs.Count; i++)
        {
            Debug.Log("in inv: " + teamInvIDs[i] + " " + i);
        }

        script.UpdateCards();
    }

}

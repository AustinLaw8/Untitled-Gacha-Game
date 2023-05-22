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
        LoadTeam();
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
    public void SaveTeam()
    {
        Wrapper temp = new Wrapper();
        temp.Items = teamIDs;
        var teamData = JsonUtility.ToJson(temp);
        System.IO.File.WriteAllText(filepath2, teamData, System.Text.Encoding.UTF8);
    }

    public void LoadTeam()
    {
        teamInvIDs.Clear();
        try {
            var loadedTeam = System.IO.File.ReadAllText(filepath2, System.Text.Encoding.UTF8);
            teamIDs = FromJson(loadedTeam);
        } catch (Exception) {
            teamIDs = new int[5]{3900, 3900, 3900, 3900, 3900};
        }

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
        for (int i = 0; i < cardManager.cardDB.Length; i++)
        {
            teamInvIDs.Add((int)cardManager.cardDB[i].ID);
        }

        script.UpdateCards();
        SaveTeam();
    }

}

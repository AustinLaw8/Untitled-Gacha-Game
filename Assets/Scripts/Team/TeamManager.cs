using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TeamManager : MonoBehaviour
{
    public static int TEAM_SIZE=5;
    static string teamFilepath { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "teamCards.json"; } }
    
    [SerializeField] CardManager cardManager;
    [SerializeField] CardInventory cardInventory; 
    [SerializeField] TeamInventory teamInventory; 

    public int[] teamIDs;

    public static int[] GetTeam()
    {
        var loadedTeam = System.IO.File.ReadAllText(teamFilepath, System.Text.Encoding.UTF8);
        return FromJson(loadedTeam);
    }

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

    // 3900 means empty slot
    public void SaveTeam()
    {
        Wrapper temp = new Wrapper();
        temp.Items = teamIDs;
        var teamData = JsonUtility.ToJson(temp);
        System.IO.File.WriteAllText(teamFilepath, teamData, System.Text.Encoding.UTF8);
    }

    void LoadTeam()
    {
        try {
            var loadedTeam = System.IO.File.ReadAllText(teamFilepath, System.Text.Encoding.UTF8);
            teamIDs = FromJson(loadedTeam);
        } catch (Exception) {
            teamIDs = new int[5]{3900, 3900, 3900, 3900, 3900};
            SaveTeam();
        }

        cardInventory.UpdateDisplay();
    }

    public void ClearTeam()
    {
        for (int i = 0; i < TEAM_SIZE; i++)
        {
            teamIDs[i] = 3900;
        }

        SaveTeam();
        teamInventory.UpdateCards();
        cardInventory.UpdateDisplay();
    }

    public bool InTeam(int cardID)
    {
        return Array.Exists(teamIDs, element => element == cardID);
    }
}

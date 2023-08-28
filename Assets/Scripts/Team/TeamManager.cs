using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamManager : MonoBehaviour
{
    public static int TEAM_SIZE=5;
    static string teamFilepath { get { return Application.persistentDataPath + Path.DirectorySeparatorChar + "teamCards.json"; } }
    
    [SerializeField] CardManager cardManager;
    [SerializeField] CardInventory cardInventory; 
    [SerializeField] TeamInventory teamInventory; 
    [SerializeField] Animator textAnimator;

    public int[] teamIDs;

    public static int[] GetTeam()
    {
        try {
            var loadedTeam = System.IO.File.ReadAllText(teamFilepath, System.Text.Encoding.UTF8);
            return FromJson(loadedTeam);
        } catch (FileNotFoundException) {
            SaveTeam(new int[5]{0,1,2,3,62});
            var loadedTeam = System.IO.File.ReadAllText(teamFilepath, System.Text.Encoding.UTF8);
            return FromJson(loadedTeam);
        }
    }

    public static void SaveTeam(int[] teamIDs)
    {
        Wrapper temp = new Wrapper();
        temp.Items = teamIDs;
        var teamData = JsonUtility.ToJson(temp);
        System.IO.File.WriteAllText(teamFilepath, teamData, System.Text.Encoding.UTF8);
    }

    public static void ResetData()
    {
        SaveTeam(new int[5]{0,1,2,3,62});
    }

    public void Awake()
    {
        LoadTeam();
        cardInventory.forTeamFormation = true;
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
            teamIDs = new int[5]{0,1,2,3,62};
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

        teamInventory.UpdateCards();
        cardInventory.UpdateDisplay();
    }

    public int InTeam(int cardID)
    {
        Debug.Log($"checking for card {cardID}");
        return Array.FindIndex(teamIDs, element => element == cardID);
    }

    public void TryChangeScene()
    {
        if(Array.Exists(teamIDs, id => id != 3900))
        {
            CardManager.cardManager.PlayButtonSFX();
            SceneManager.LoadScene("CharacterScreen", LoadSceneMode.Single);
        }
        else
        {
            textAnimator.Play("Fade");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetSong : MonoBehaviour
{

    [Header("Sprites for copying")]
    [SerializeField] private Sprite selectedSong;
    [SerializeField] private Sprite unselectedSong;
    [SerializeField] private Sprite selectedEasy;
    [SerializeField] private Sprite unselectedEasy;
    [SerializeField] private Sprite selectedHard;
    [SerializeField] private Sprite unselectedHard;

    [Header("Dynamically edited GameObjects")]
    [SerializeField] private Image easyButton;
    [SerializeField] private Image hardButton;
    [SerializeField] private TextMeshProUGUI songInfo;

    [Header("Beatmap Information")]
    [SerializeField] private BeatmapSO container;
    [SerializeField] private BeatmapSO[] beatmaps;
    
    // For when user selects one of the song buttons
    public void SetSelectedSong(int id)
    {
        // Set the sprite of the song buttons
        UnselectAll();
        this.transform.GetChild((int)id).GetComponent<Image>().sprite = selectedSong;

        uint uid = (uint)id;
        // Set the stored beatmap to either the easy or hard version of the song
        // Depending on what is currently selected (keeps difficulty)
        if (container.ID % 2 ==　0)
        {
            Set(uid * 2);
        }
        else
        {
            Set(uid * 2 + 1);
        }
    }

    // For when user has a song selected, and presses easy or hard
    public void SetDifficulty(bool easy)
    {
        // If difficutly is already set right, noop
        if (easy && (container.ID % 2 == 0) ||
            !easy && (container.ID % 2 == 1)) return;

        // Change from Hard -> Easy
        if (easy)
        {
            Set(container.ID - 1);
        }
        else
        {
            Set(container.ID + 1);
        }
    }

    private void Set(uint songId)
    {
        // Even songId is easy
        if (songId % 2 == 0)
        {
            easyButton.sprite = selectedEasy;
            hardButton.sprite = unselectedHard;
        }
        else
        {
            easyButton.sprite = unselectedEasy;
            hardButton.sprite = selectedHard;

        }

        // Set text to match beatmap
        songInfo.text = $"Composer: {beatmaps[songId].artist}\nLevel Designer: {beatmaps[songId].levelDesigner}";

        // Sets the container beatmap to whatever was selected
        container.copy(beatmaps[songId]);
    }

    // Changes all song button sprites to the unselected
    private void UnselectAll()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<Image>().sprite = unselectedSong;
        }
    }
}
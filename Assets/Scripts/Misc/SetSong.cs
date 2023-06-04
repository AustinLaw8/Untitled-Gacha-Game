using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetSong : MonoBehaviour

{
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite unselected;
    [SerializeField] private Sprite selectedButton;
    [SerializeField] private Sprite unSelectedButton;

    [SerializeField] private Image[] allSongs;
    [SerializeField] private SetBeatmap easyButton;
    [SerializeField] private SetBeatmap hardButton;
    [SerializeField] private BeatmapSO container;
    [SerializeField] private BeatmapSO easy;
    [SerializeField] private BeatmapSO hard;

    [SerializeField] private TextMeshProUGUI songInfo;

    private void UnselectAll()
    {
        foreach(var image in allSongs)
        {
            image.sprite = unselected;
        }
    }
    
    public void SetContainer()
    {
        UnselectAll();
        this.gameObject.GetComponent<Image>().sprite = selected;
        easyButton.gameObject.GetComponent<Image>().sprite = selectedButton;
        easyButton.beatmap = easy;
        hardButton.beatmap = hard;
        hardButton.gameObject.GetComponent<Image>().sprite = unSelectedButton;
        songInfo.text = $"Composer: {easy.artist}\nLevel Designer: {easy.levelDesigner}";
        container.copy(easy);
    }
}
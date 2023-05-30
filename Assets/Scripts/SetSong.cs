using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSong : MonoBehaviour

{
    [SerializeField] private SetBeatmap easyButton;
    [SerializeField] private SetBeatmap hardButton;
    [SerializeField] private BeatmapSO container;
    [SerializeField] private BeatmapSO easy;
    [SerializeField] private BeatmapSO hard;

    public void SetContainer()
    {
        easyButton.beatmap = easy;
        hardButton.beatmap = hard;
        container.copy(easy);
    }
}
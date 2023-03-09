using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBeatmap : MonoBehaviour
{
    public BeatmapSO temp;
    [SerializeField] private BeatmapSO container;

    public void SetContainer()
    {
        container.songName = temp.songName;
        container.mapData = temp.mapData;
        container.clip = temp.clip;
        container.artist = temp.artist;
    }
}

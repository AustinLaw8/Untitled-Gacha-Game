using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Beatmap", menuName = "ScriptableObjects/Beatmap", order = 1)]
public class BeatmapSO : ScriptableObject
{
    public uint ID;
    public string songName;
    public TextAsset mapData;
    public AudioClip clip;
    public string artist;
    public string levelDesigner;

    public void copy (BeatmapSO other)
    {
        this.ID = other.ID;
        this.songName = other.songName;
        this.mapData = other.mapData;
        this.clip = other.clip;
        this.artist = other.artist;
        this.levelDesigner = other.levelDesigner;
    }
}

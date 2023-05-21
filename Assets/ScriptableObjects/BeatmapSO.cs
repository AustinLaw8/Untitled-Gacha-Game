using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Beatmap", menuName = "ScriptableObjects/Beatmap", order = 1)]
public class BeatmapSO : ScriptableObject
{
    public uint ID;
    public string songName;
    public TextAsset mapData;
    // public float duration;
    public AudioClip clip;
    public string artist;
}

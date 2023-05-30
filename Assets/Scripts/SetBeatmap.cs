using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBeatmap : MonoBehaviour
{
    public BeatmapSO beatmap;
    [SerializeField] private BeatmapSO container;

    public void SetContainer()
        { container.copy(beatmap); } // on click
}

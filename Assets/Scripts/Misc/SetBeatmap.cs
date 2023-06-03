using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetBeatmap : MonoBehaviour
{
    public BeatmapSO beatmap;
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite otherUnselected;
    [SerializeField] private GameObject other;
    [SerializeField] private BeatmapSO container;

    [SerializeField] private TextMeshProUGUI songInfo;

    public void SetContainer()
    {
        this.gameObject.GetComponent<Image>().sprite = selected;
        other.gameObject.GetComponent<Image>().sprite = otherUnselected;
        songInfo.text = $"Composer: {beatmap.artist}\nLevel Designer: {beatmap.levelDesigner}";
        container.copy(beatmap);
    }
}

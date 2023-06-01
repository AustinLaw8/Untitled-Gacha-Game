using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
    public void Reset()
    {
        PlayerSongInfo.ResetData();
        TeamManager.ResetData();
        CardManager.ResetData();
    }
}

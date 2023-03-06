using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/** TODO
[System.Serializable]
public class NotePressedEvent : UnityEvent
{
}
*/

public class BeatManager : MonoBehaviour
{
    /* Predetermined, based on the song and the created beatmap */
    [Header("Song Information")]

    [Tooltip("The offset, in beats, until song starts")]
    [SerializeField] private float songStartOffset;
    
    [Tooltip("Container BeatmapSO so BeatManager knows what song to play and load")]
    [SerializeField] private BeatmapSO container;

    [Header("Environment Objects")]
    [SerializeField] private GameObject note;
    [SerializeField] private GameObject flickNote;
    [SerializeField] private GameObject holdNote;

    [SerializeField] private Transform spawnLine;
    [SerializeField] private Transform playLine;

    /* Calculated once when game Controller loads */
    [Header("Debug Fields, Do Not Manually Edit")]
    [Tooltip("The song BPM")]
    [SerializeField] private float bpm;
    [Tooltip("Time to when the song will start, including offset")]
    [SerializeField] private float startTime;
    [Tooltip("Time to end of song, in seconds, not including any offset")]
    [SerializeField] private float endTime;
    [Tooltip("BPM / 60")]
    [SerializeField] private float beatsPerSecond;
    [Tooltip("1 / BPS (or equivalently, 60 / BPM")]
    [SerializeField] private float secondsPerBeat;
    [Tooltip("Offset in seconds until song starts (simply converted from songStartOffset")]
    [SerializeField] private float songStartOffsetInSeconds;

    /* Calculated every Update() */
    [Tooltip("Song position in seconds")]
    [SerializeField] private float songPosition;
    [Tooltip("Song position in beats")]
    [SerializeField] private float songPositionInBeats;

    // A beatmap is a queue of times notes are meant to exist (i.e. there should be a note to press at 2.2 seconds)
    // static (float, int)[] exampleNotes = {(1f,0), (2f,0), (3f,0), (4f,0), (5f,0), (6f,0), (7f,0), (8f,0), (9f,0), (10f,0)};

    private Queue<(float, int)> beatmap = new Queue<(float, int)>();
    private Queue<List<(float, int)>> holdNotes = new Queue<List<(float, int)>>();

    private float spawnDiff;
    void Awake()
    {
        /** TODO:
         *
         * Create a BeatmapController that loads beatmap from memory and loads it into controller
         * It will probably set the bpm too 
         */


        bpm = container.bpm;
        if (bpm == 0)
        {
            Debug.LogError("Give BPM a value or else you will infinite loop");
            Destroy(this.gameObject);
        }

        beatsPerSecond = bpm / 60;
        secondsPerBeat = 1 / beatsPerSecond;
        songStartOffsetInSeconds = songStartOffset * secondsPerBeat;

        startTime = (float)AudioSettings.dspTime + songStartOffsetInSeconds;
        endTime += songStartOffset * secondsPerBeat;

        spawnDiff = (spawnLine.position.y - playLine.position.y) / Note.fallSpeed;
        songPosition = 0;
        LoadSong();
    }

    void Start()
    {
        StartCoroutine(PlayMusicWithOffset());
        foreach( var x in beatmap) {
            Debug.Log($"time {x.Item1}, lane {x.Item2}");
        }
    }

    void FixedUpdate()
    {
        songPosition = ((float)AudioSettings.dspTime - startTime);
        songPositionInBeats = beatsPerSecond * songPosition;

        // songPosition += Time.fixedDeltaTime;
        float pos;
        int lane;
        GameObject noteSpawned;
        while (beatmap.Count > 0)
        {
            (pos, lane) = beatmap.Peek();
            
            if (Mathf.Abs(pos) > songPosition + spawnDiff)
            {
                break;
            }

            if (pos > 0)
            {
                noteSpawned = GameObject.Instantiate(note);
            }
            else
            {
                noteSpawned = GameObject.Instantiate(flickNote);
            }
            beatmap.Dequeue();
            // calculate the difference in time when the note was supposed to spawn and the time now
            float diffTime = (songPosition + spawnDiff) - Mathf.Abs(pos);
            float diffDist = (diffTime*Note.fallSpeed);
            // push down the note a small about based on that difference in time
            noteSpawned.transform.position -= new Vector3(0f, diffDist, 0f);
            // noteSpawned.transform.position -= new Vector3(0f, <the amount to push>, 0f);
        }

        if (beatmap.Count == 0)
        {
            // Debug.Log("end game");
            // SceneManager.LoadScene("GachaScene", LoadSceneMode.Single);
        }
    }

    // Adds the offset to the song (to wait for the beat map to start)
    IEnumerator PlayMusicWithOffset()
    {
        yield return new WaitForSeconds(songStartOffsetInSeconds);
        // start playing song
    }
    
    void LoadSong()
    {
        List<(float, int)> temp = new List<(float, int)>();
        string[] lines = container.mapData.text.Split('\n');
        for (int i = 0; i < 7; i++)
        {
            string[] times = lines[i].Split(',');
            foreach (string time in times)
            {
                Debug.Log(time);
                if (time != "")
                {
                    float songTime = float.Parse(time);
                    temp.Add( (songTime, i) );
                }
            }
        }
        temp.Sort( delegate ( (float, int) x, (float, int) y )
        {
            if (x.Item1 < y.Item1) return -1;
            else return 1;
        });
        foreach (var x in temp)
        {
            beatmap.Enqueue(x);
        }
        for (int i = 7; i < lines.Length; i++)
        {
            temp = new List<(float, int)>();
            string[] times = lines[i].Split(',');
            int lane=-1;
            float songTime=0f;
            for(int j = 0; j < times.Length; j++)
            {
                if ( j % 2 == 0 )
                {
                    if (songTime != 0f)
                    {
                        temp.Add( (songTime, lane) );
                    }
                    lane = int.Parse(times[j]);
                }
                else
                {
                    songTime = float.Parse(times[j]);
                }
            }
            holdNotes.Enqueue(temp);
        }
    }
}
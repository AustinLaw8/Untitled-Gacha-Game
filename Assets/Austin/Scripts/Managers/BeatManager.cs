using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Tooltip("The song BPM")]
    [SerializeField] private float bpm;

    [Tooltip("The offset, in beats, until song starts")]
    [SerializeField] private float songStartOffset;

    [Tooltip("Time to end of song, in seconds, not including any offset")]
    [SerializeField] private float endTime;

    /* Calculated once when game Controller loads */
    [Header("Debug Fields")]
    [Tooltip("Time to when the song will start, including offset")]
    [SerializeField] private float startTime;
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

    /* Various external components */
    [Header("External Objects")]

    /* Fields to set up notes */

    // A beatmap is a queue of times notes are meant to exist (i.e. there should be a note to press at 2.2 seconds)
    static (float, int)[] exampleNotes = {(1f,0), (2f,0), (3f,0), (4f,0), (5f,0), (6f,0), (7f,0), (8f,0), (9f,0), (10f,0)};

    private Queue<(float, int)> beatmap = new Queue<(float, int)>(exampleNotes);
    [SerializeField] private GameObject note;
    [SerializeField] private GameObject flickNote;
    [SerializeField] private GameObject holdNote;

    [SerializeField] private Transform spawnLine;
    [SerializeField] private Transform playLine;

    private float spawnDiff;
    void Awake()
    {
        /** TODO:
         *
         * Create a BeatmapController that loads beatmap from memory and loads it into controller
         * It will probably set the bpm too 
         */

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
    }

    void Start()
    {
        StartCoroutine(PlayMusicWithOffset());
    }

    void FixedUpdate()
    {
        // songPosition = ((float)AudioSettings.dspTime - startTime);
        songPositionInBeats = beatsPerSecond * songPosition;

        songPosition += Time.fixedDeltaTime;
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
            Debug.Log(diffDist);
            // push down the note a small about based on that difference in time
            noteSpawned.transform.position -= new Vector3(0f, diffDist, 0f);
            // noteSpawned.transform.position -= new Vector3(0f, <the amount to push>, 0f);
        }
    }

    // Adds the offset to the song (to wait for the beat map to start)
    IEnumerator PlayMusicWithOffset()
    {
        yield return new WaitForSeconds(songStartOffsetInSeconds);
        // start playing song
    }
    
}
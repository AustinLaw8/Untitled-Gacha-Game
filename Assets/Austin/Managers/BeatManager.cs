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
    [SerializeField] private GameObject tapNotePrefab;

    /* Fields to set up notes */

    // A beatmap is a queue of times notes are meant to exist (i.e. there should be a note to press at 2.2 seconds)
    private Queue<float> beatmap = new Queue<float>();
    
    // private static UnityEvent NotePressedEvent;

    void Awake()
    {
        /** TODO:
         *
         * Create a BeatmapController that loads beatmap from memory and loads it into controller
         * It will probably set the bpm too 
         */


        // NotePressedEvent.AddListener(HandleNotePress);

        beatsPerSecond = bpm / 60;
        secondsPerBeat = 1 / beatsPerSecond;
        songStartOffsetInSeconds = songStartOffset * secondsPerBeat;

        startTime = (float)AudioSettings.dspTime + songStartOffsetInSeconds;
        endTime += songStartOffset * secondsPerBeat;
    }

    void Start()
    {
        StartCoroutine(PlayMusicWithOffset());
    }

    void Update()
    {
        songPosition = ((float)AudioSettings.dspTime - startTime);
        songPositionInBeats = beatsPerSecond * songPosition;
        // float center;
        // Note note;

        
        // if (beatmap.Count > 0) 
        {
            // center = beatmap.Peek();

            // if(songPosition >= middle - Note.fallTime)
            {
                // note = GameObject.Instantiate(notePrefab).GetComponent<Note>();
            }
        }

        /**TODO*
         * 
         * Implement a ScoreManager that handles score
         * Ideally have it be event based?
         */
    }


    /*** Helpers ***/

    /*** TODO
    private void SetBeatmap()
    {
        beatmap.Clear(); 
        for (float i = 3; i < 31; i ++)
        {
            beatmap.Enqueue(((i - Note.DEFAULT_LEEWAY) * secondsPerBeat, (i + Note.DEFAULT_LEEWAY) * secondsPerBeat));
        }
    }
    */

    // Adds the offset to the song (to wait for the beat map to start)
    IEnumerator PlayMusicWithOffset()
    {
        yield return new WaitForSeconds(songStartOffsetInSeconds);
        // start playing song
    }
    
}
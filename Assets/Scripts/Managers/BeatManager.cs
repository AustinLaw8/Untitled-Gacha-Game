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
    public readonly static float SPAWN_POINT = 5f;
    public readonly static float PLAY_POINT = -3.4f;

    private static float WAIT_TIME = 5f;
    
    [Header("Game Information")]
    [Tooltip("Container BeatmapSO so BeatManager knows what song to play and load")]
    [SerializeField] private BeatmapSO container;
    [Tooltip("Container SettingsSO")]
    [SerializeField] private SettingsSO settings;

    [Header("Environment Objects")]
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private GameObject note;
    [SerializeField] private GameObject flickNote;
    [SerializeField] private GameObject holdNote;

    [SerializeField] private Transform spawnLine;
    [SerializeField] private Transform playLine;
    [SerializeField] private SpriteRenderer background;

    /* Calculated every Update() */
    [Tooltip("Song position in seconds")]
    [SerializeField] private float songPosition;

    private float startTime;

    // A beatmap is represented a queue of (time, lane) notes are meant to exist
    // i.e. (2.2, 3) means that there should be a note to press at 2.2 seconds in lane 3
    private Queue<(float, int)> beatmap = new Queue<(float, int)>();

    // Hold notes are represented in a similar way, as a queue of List<(time, lane)> notes 
    // The List reflects the pivot points of the hold note
    // i.e. (2.2, 3), (3.3, 5) means that there should be a note to press at 2.2 seconds and held until 3.3 seconds in lane 5
    private Queue<List<(float, int)>> holdNotes = new Queue<List<(float, int)>>();

    private float spawnDiff;
    void Awake()
    {
        LoadSong();

        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        
        Note.fallSpeed = settings.noteSpeed;
        settings.SetVolume();

        spawnDiff = (spawnLine.position.y - playLine.position.y) / Note.fallSpeed;

    }

    void Start()
    {
        startTime = (float)AudioSettings.dspTime + WAIT_TIME;

        musicSource.clip = container.clip;
        StartCoroutine(PlayMusicWithOffset());
        background.color = new Color(255,255,255,255 * settings.transparency);
    }

    void Update()
    {
        songPosition = ((float)AudioSettings.dspTime - startTime);

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
            noteSpawned.transform.position = spawnLine.transform.position;

            // calculate the difference in time when the note was supposed to spawn and the time now
            // move the note a small amount based on that difference in time
            float diffTime = (songPosition + spawnDiff) - Mathf.Abs(pos);
            Note n = noteSpawned.GetComponent<Note>();
            n.SetLane(lane);
            n.Bump(diffTime);

            beatmap.Dequeue();
        }

        if (beatmap.Count == 0 && !musicSource.isPlaying)
        {
            StartCoroutine(EndGameWithOffset());
        }
    }

    // Adds the offset to the song (to wait for the beat map to start)
    IEnumerator PlayMusicWithOffset()
    {
        yield return new WaitForSeconds(WAIT_TIME);
        musicSource.Play();
    }

    IEnumerator EndGameWithOffset()
    {
        yield return new WaitForSeconds(WAIT_TIME);
        SceneManager.LoadScene("HomeScreen", LoadSceneMode.Single);
    }

    
    // Helper to load whatever song is in the BeatmapSO container
    void LoadSong()
    {
        // Parses the first seven lines of the beatmap, which contains information for the lanes
        List<(float, int)> temp = new List<(float, int)>();
        string[] lines = container.mapData.text.Split('\n');
        for (int i = 0; i < 7; i++)
        {
            string[] times = lines[i].Split(',');
            foreach (string time in times)
            {
                if (time != "")
                {
                    float songTime = float.Parse(time);
                    temp.Add( (songTime, i) );
                }
            }
        }
        // Sorts the beatmap by so that the Queue assumption is valid
        temp.Sort( delegate ( (float, int) x, (float, int) y )
        {
            if (Mathf.Abs(x.Item1) < Mathf.Abs(y.Item1)) return -1;
            else return 1;
        });
        // Sets beatmap
        foreach (var x in temp)
        {
            beatmap.Enqueue(x);
        }

        // Parses the rest of the beatmap to retrieve holdNotes
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
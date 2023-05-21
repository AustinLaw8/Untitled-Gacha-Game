using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugClocker : MonoBehaviour
{
    private float ONE_SECOND = 1f;

    [SerializeField] private bool SPAWN_HOLD=true;
    [SerializeField] private GameObject note;
    [SerializeField] private GameObject flickNote;
    [SerializeField] private GameObject holdNote;
    [SerializeField] private Transform spawnLine;

    private float timer;
    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        counter = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        timer += Time.fixedDeltaTime;

        if (timer > ONE_SECOND)
        {
            timer = 0f;
            GameObject clone;

            if (counter == 0)
            {
                TrySpawnHold();
                timer = -6f;
            }
            else if (counter < 8)
            {
                if (counter % 8 == 7)
                {
                    for(int i = 0; i < 7; i+=3)
                    {
                        clone = GameObject.Instantiate(flickNote);
                        clone.transform.position = spawnLine.transform.position;
                        clone.GetComponent<Note>().SetLane(i);
                    }
                }
                else
                {
                    clone = GameObject.Instantiate(note);
                    clone.transform.position = spawnLine.transform.position;
                    clone.GetComponent<Note>().SetLane(counter);
                }
            }
            counter = (counter + 1) % 8;
        }
    }

    void TrySpawnHold()
    {
        GameObject clone = GameObject.Instantiate(holdNote);
        HoldNote hn = clone.GetComponent<HoldNote>();
        hn.SetPoints(0, new List<(float, int)>(){
            (0, 5),
            (2, 6),
            (4, 4),
        });

        clone = GameObject.Instantiate(holdNote);
        hn = clone.GetComponent<HoldNote>();
        hn.SetPoints(0, new List<(float, int)>(){
            (0, 1),
            (1, 0),
            (2, 2),
            (3, 1),
        });
    }
}

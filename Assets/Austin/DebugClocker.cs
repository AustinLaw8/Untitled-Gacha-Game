using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugClocker : MonoBehaviour
{
    private float ONE_SECOND = 1f;

    [SerializeField] private GameObject note;
    [SerializeField] private GameObject flickNote;
    private float timer;
    private uint counter;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > ONE_SECOND)
        {
            timer = 0f;
            counter += 1;
            if (counter % 4 == 0)
            {
                GameObject.Instantiate(flickNote);
            }
            else
            {
                GameObject.Instantiate(note);
            }
        }
    }
}

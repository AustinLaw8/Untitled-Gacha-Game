using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugClocker : MonoBehaviour
{
    private float ONE_SECOND = 1f;

    [SerializeField] private GameObject note;
    [SerializeField] private GameObject flickNote;
    [SerializeField] private GameObject holdNote;

    private float timer;
    private uint counter;

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
            if (counter < 4)
            {
                if (counter % 4 == 0)
                {
                    GameObject.Instantiate(flickNote);
                }
                else
                {
                    GameObject.Instantiate(note);
                    GameObject note2 =  GameObject.Instantiate(note);
                    note2.transform.position -= new Vector3(4f, 0, 0);
                }
            }
            else if (counter == 5)
            {
                GameObject.Instantiate(holdNote);
            }
            counter = (counter + 1) % 10;
        }
    }
}

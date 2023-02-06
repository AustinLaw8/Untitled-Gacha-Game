using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Note : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 10f;    

    // Note fall!
    void FixedUpdate()
    {
        this.transform.position = new Vector3(
                this.transform.position.x,
                this.transform.position.y - fallSpeed * Time.fixedDeltaTime, 
                this.transform.position.z
        );
    }

    // TODO: placeholder to return value of the note
    // note from austin ~ this will probably just be a function that calls a score manager or something like that, since it will be probably be something external that will do score calcs; however, since tap/flick notes will probably have different score values than hold notes, this is here so that Lane can call Note.GetScoreValue() to get the score number
    public int GetScoreValue()
    {
        return 1;
    }

}

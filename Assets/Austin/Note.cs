using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Accuracy
{
    Perfect, Great, Good, Bad, Miss
}

public class Note : MonoBehaviour
{
    private float fallSpeed;    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // fall down   
    }   

    // TODO should this be in note or in fall line
    void OnNotePressed()
    {
        // Calculate Accuracy
        // Tell ScoreManager something if necessary
        // Tell HPBar something if necessary

    }
}

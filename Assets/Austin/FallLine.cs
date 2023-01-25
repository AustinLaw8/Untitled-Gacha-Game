using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallLine : MonoBehaviour
{
    [Tooltip("The distance at which a player's \"tap\" is still registered")]
    [SerializeField] private float range;

    private Collider col;
    private float yLow;
    private float yHigh;

    void Start()
    {
        float x = this.transform.position.x;
        float y = this.transform.position.y;
        yLow = y - range;
        yHigh = y + range;

        Debug.DrawLine(
            new Vector3(x - 10f, yLow, 0),
            new Vector3(x + 10f, yLow, 0),
            Color.green
        );
        Debug.DrawLine(
            new Vector3(x - 10f, yHigh, 0),
            new Vector3(x + 10f, yHigh, 0),
            Color.green
        );
    }

    void Update()
    {
        Vector3 pos;

        foreach(Touch touch in Input.touches)
        {
            /* Insert code here */
            /* Should be able to copy paste everything below here, with minor modifications */
        }

        if (Input.GetMouseButtonDown(0))
        {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(pos);
            if (yLow <= pos.y && pos.y <= yHigh)
            {
                TapHandler(MapXPosToLane(pos.x));
            }
        }
    }

    void TapHandler(int lane)
    {
        Debug.Log("In range");
        // Check if note in range
        // either call Note.OnNotePressed or do it all itself here
    }

    // Helper to map the x location of the user input to the proper lane
    int MapXPosToLane(float x)
    {
        // TODO
        return 0;
    }
}

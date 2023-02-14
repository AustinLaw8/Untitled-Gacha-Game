using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Note : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f;    

    private bool interactable;
    public bool IsInteractable { get { return interactable; } } 

    protected Collider2D lane;
    protected Collider2D col;

    void Start()
    {
        lane = GameObject.Find("Lane").GetComponent<Collider2D>();
        col = GetComponent<Collider2D>();
        interactable = false;
    }
    
    // Drops note, and determines if it is interactable yet
    protected virtual void FixedUpdate()
    {
        this.transform.position -= new Vector3(0, fallSpeed * Time.fixedDeltaTime, 0);
        interactable = col.IsTouching(lane);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == lane)
        {
            Destroy(this.gameObject);
            // TODO: Deal damage to player
        }
    }

    /* Helpers */

    /**
     * Although detection for each note type has to be unique, once we know a note has been pressed,
     * the score, health, and combo calcs should be the same irrespective of the note type
     * ... although that isn't necessarily set in stone.
     */
    // Gets accuracy, tells ScoreManager to increase the score, and destroys the pressed Note.
    protected void OnNotePressed()
    {
        if (interactable)
        {
            Accuracy accuracy = GetAccuracy();
            ScoreManager.scoreManager.IncreaseScore(accuracy);
            Destroy(this.gameObject);
        }
    }

    // TODO: Calculate accuracy based on distance from fall line/time difference between press and intended
    protected Accuracy GetAccuracy()
    {
        return Accuracy.Great;
    }
}

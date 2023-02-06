using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickInput : MonoBehaviour
{
    [SerializeField] Vector2 MinMoveVal = new Vector2(50f, 50f);

    private Vector2 StartPos;
    private Vector2 EndPos;

    public enum FlickDirection
    {
        NONE,
        TAP,
        UP,
        DOWN, 
        RIGHT,
        LEFT,
    }

    private FlickDirection NowFlick = FlickDirection.NONE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        JudgeInput();
    }

    public FlickDirection GetNowFlick()
    {
        return NowFlick;
    }

    private void ResetNowFlick()
    {
        NowFlick = FlickDirection.NONE;
    }

    private void HandleFlick()
    {
        Vector2 distance = new Vector2(
            (new Vector3(EndPos.x, 0, 0) - new Vector3(StartPos.x, 0, 0)).magnitude,
            (new Vector3(0, EndPos.y, 0) - new Vector3 (0, StartPos.y, 0)).magnitude
            );
        if (distance.x <=MinMoveVal.x && distance.y <= MinMoveVal.y)
        {
            ResetNowFlick();
        }
        else if (distance.x > distance.y)
        {
            float x = Mathf.Sign(EndPos.x - StartPos.x);
            if (x > 0)
                NowFlick = FlickDirection.RIGHT;
            else if (x < 0)
                NowFlick = FlickDirection.LEFT;
        }
        else
        {
            float y = Mathf.Sign(EndPos.y - StartPos.y);
            if (y > 0)
                NowFlick = FlickDirection.UP;
            else if (y < 0)
                NowFlick = FlickDirection.DOWN;
        }
    }

    private void JudgeInput()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
                StartPos = Input.mousePosition;
            else if (Input.GetMouseButtonUp(0))
            {
                EndPos = Input.mousePosition;
                HandleFlick();
            }
            else if (NowFlick != FlickDirection.NONE)
            {
                ResetNowFlick();
            }    
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    StartPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    EndPos = touch.position;
                    HandleFlick();
                }
            }
            else if (NowFlick != FlickDirection.NONE)
            {
                ResetNowFlick();
            }
        }
    }

}

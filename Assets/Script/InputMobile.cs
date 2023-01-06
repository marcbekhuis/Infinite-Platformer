using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMobile : InputBase
{
    private Dictionary<int, JumpTouch> jumpTouchStartPosition = new Dictionary<int, JumpTouch>();
    private float jumpTriggerDistance = 1;

    private List<bool> positionUI = new List<bool>();

    public InputMobile()
    {
        jumpTriggerDistance = Screen.height / 4f;
    }

    private class JumpTouch 
    {
        public Vector3 position;
        public float lastJump;

        public JumpTouch(Vector3 position)
        {
            this.position = position;
            lastJump = Time.time - 1;
        }
    }

    public void UiCheck()
    {
        positionUI.Clear();
        foreach (var touch in Input.touches)
        {
            positionUI.Add(UiDetection.instance.CheckOnUI(touch.position));
        }
    }

    public override bool Jump()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];
            if (positionUI[i])
                continue;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!jumpTouchStartPosition.ContainsKey(touch.fingerId))
                        jumpTouchStartPosition.Add(touch.fingerId, new JumpTouch(touch.position));
                    break;
                case TouchPhase.Moved:
                    if (jumpTouchStartPosition.ContainsKey(touch.fingerId))
                    {
                        if (touch.position.y - jumpTouchStartPosition[touch.fingerId].position.y >= jumpTriggerDistance)
                        {
                            if (Time.time >= jumpTouchStartPosition[touch.fingerId].lastJump + 0.1f)
                            {
                                jumpTouchStartPosition[touch.fingerId].position = touch.position;
                                jumpTouchStartPosition[touch.fingerId].lastJump = Time.time;
                                return true;
                            }
                        }
                        else if(touch.position.y < jumpTouchStartPosition[touch.fingerId].position.y)
                        {
                            jumpTouchStartPosition[touch.fingerId].position = touch.position;
                        }
                    }
                    break;
                case TouchPhase.Canceled:
                    jumpTouchStartPosition.Remove(touch.fingerId);
                    break;
                case TouchPhase.Ended:
                    jumpTouchStartPosition.Remove(touch.fingerId);
                    break;
                default:
                    break;
            }
        }
        return false;
    }

    public override int MoveLeft()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];
            if (positionUI[i])
                continue;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (touch.position.x < Screen.width / 2)
                        return -1;
                    break;
                case TouchPhase.Moved:
                    if (touch.position.x < Screen.width / 2)
                        return -1;
                    break;
                case TouchPhase.Stationary:
                    if (touch.position.x < Screen.width / 2)
                        return -1;
                    break;
                default:
                    break;
            }
        }
        return 0;
    }

    public override int MoveRight()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.touches[i];
            if (positionUI[i])
                continue;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (touch.position.x >= Screen.width / 2)
                        return 1;
                    break;
                case TouchPhase.Moved:
                    if (touch.position.x >= Screen.width / 2)
                        return 1;
                    break;
                case TouchPhase.Stationary:
                    if (touch.position.x >= Screen.width / 2)
                        return 1;
                    break;
                default:
                    break;
            }
        }
        return 0;
    }

    public override string ToString()
    {
        return "Mobile Input";
    }
}

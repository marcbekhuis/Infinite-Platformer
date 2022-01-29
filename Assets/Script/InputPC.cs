using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPC : InputBase
{
    public override bool Jump()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
    }

    public override int MoveLeft()
    {
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            return -1;
        return 0;
    }

    public override int MoveRight()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            return 1;
        return 0;
    }

    public override string ToString()
    {
        return "PC Input";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private InputPC inputPC;
    private InputMobile inputMobile;

    private void Start()
    {
        inputPC = new InputPC();
        inputMobile = new InputMobile();
    }

    public void Uicheck()
    {
        inputMobile.UiCheck();
    }

    public bool Jump()
    {
        if(inputPC.Jump())
            return true;
        return inputMobile.Jump();
    }

    public int MoveLeft()
    {
        int pc = inputPC.MoveLeft();
        if (pc != 0)
            return pc;
        return inputMobile.MoveLeft();
    }

    public int MoveRight()
    {
        int pc = inputPC.MoveRight();
        if (pc != 0)
            return pc;
        return inputMobile.MoveRight();
    }
}

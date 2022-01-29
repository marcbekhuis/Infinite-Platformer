using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : Singleton<Gamemanager>
{
    [HideInInspector] public InputBase input;
    [HideInInspector] public bool gamePaused = false;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            input = new InputMobile();
        else
            input = new InputPC();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isRemoteConnected && input.GetType() != typeof(InputMobile))
        {
            input = new InputMobile();
            return;
        }
#endif
    }

    public bool PlayerMovementBlocked { get => gamePaused; }
}

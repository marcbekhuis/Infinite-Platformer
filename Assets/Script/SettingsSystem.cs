using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSystem : Singleton<SettingsSystem>
{
    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        //Application.targetFrameRate = Screen.currentResolution.refreshRate + 10;
    }
}

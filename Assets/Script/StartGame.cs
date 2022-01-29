using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;
    [SerializeField] private UIPanels uiPanels;

    private void Start()
    {
        Gamemanager.instance.gamePaused = true;
    }

    public void Play()
    {
        uiPanels.SetActivePanel(gameUI);
        Gamemanager.instance.gamePaused = false;
    }
}

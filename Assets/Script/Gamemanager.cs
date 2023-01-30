using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Player player;

    [HideInInspector] public bool gamePaused = false;
    [HideInInspector] public bool gameEnded = false;

    public float playerDistance = 0;
    public bool PlayerMovementBlocked { get => gamePaused || gameEnded; }

    private void Start()
    {
        MainMenu();
    }

    private void Update()
    {
        InputManager.instance.Uicheck();
        player.UpdateLogic();
    }

    public void MainMenu()
    {
        gameEnded = false;
        gamePaused = true;
        UiManager.instance.ShowMainMenu();
    }

    public void Play()
    {
        gamePaused = false;
        gameEnded = false;
        UiManager.instance.ShowGameUI();
    }

    public void TogglePause()
    {
        gamePaused = !gamePaused;
        if (gamePaused)
            UiManager.instance.ShowPauseMenu();
        else 
            UiManager.instance.ShowGameUI();
    }

    public void GameOver()
    {
        gameEnded = true;
        UiManager.instance.ShowGameOverMenu();
    }

    public void ContinueGame()
    {
        gamePaused = false;
        gameEnded = false;
        UiManager.instance.ShowGameUI();
    }

    public void ResetGame()
    {
        playerDistance = 0;
        MapGenerator.instance.ResetMap();
    }
}

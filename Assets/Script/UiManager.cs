using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    [Header("Game UI")]
    [SerializeField] private GameObject gameUI;
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenu;
    [Header("Shop Menu")]
    [SerializeField] private GameObject shopMenu;
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [Header("Game Over Menu")]
    [SerializeField] private GameObject gameOverMenu;

    public void ShowGameUI()
    {
        gameUI.SetActive(true);
        mainMenu.SetActive(false);
        shopMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        gameUI.SetActive(false);
        shopMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    public void ShowShopMenu()
    {
        shopMenu.SetActive(true);
        mainMenu.SetActive(false);
        gameUI.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        gameUI.SetActive(false);
        mainMenu.SetActive(false);
        shopMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    public void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameUI.SetActive(false);
        mainMenu.SetActive(false);
        shopMenu.SetActive(false);
    }
}

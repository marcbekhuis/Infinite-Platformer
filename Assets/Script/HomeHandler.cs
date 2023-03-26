using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeHandler : MonoBehaviour
{
    [SerializeField] private Player player;

    public void ClickHome()
    {
        if (AdsSystem.instance.showAd)
        {
            new AdsHandler(onAdEnded: Home);
        }
        else
            Home();
    }

    private void Home()
    {
        GameManager.instance.ResetGame();
        GameManager.instance.MainMenu();
        player.Respawn(player.spawnPosition);
    }
}

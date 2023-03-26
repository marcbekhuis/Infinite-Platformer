using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryHandler : MonoBehaviour
{
    [SerializeField] private Player player;

    public void ClickRetry()
    {
        if (AdsSystem.instance.showAd)
        {
            new AdsHandler(onAdEnded: Retry);
        }
        else
            Retry();
    }

    private void Retry()
    {
        GameManager.instance.ResetGame();
        GameManager.instance.Play();
        player.Respawn(player.spawnPosition);
    }
}

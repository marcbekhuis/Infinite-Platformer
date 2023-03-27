using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueHandler : MonoBehaviour
{
    public void ClickRespawn()
    {
        if (AdsSystem.instance.showAd)
        {
            new AdsHandler(onAdEnded: Respawn, reward: true);
        }
        else
            Respawn();
    }

    private void Respawn()
    {
        GameManager.instance.ContinueGame();
    }
}

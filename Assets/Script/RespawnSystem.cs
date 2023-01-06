using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    public void RespawnPressed()
    {
        AdsSystem.instance.onRewardComplited += Respawn;
        AdsSystem.instance.LoadAd(AdsSystem.instance._rewardAdUnitId);
    }

    private void Respawn()
    {
        AdsSystem.instance.onRewardComplited -= Respawn;
        GameManager.instance.ContinueGame();
    }
}

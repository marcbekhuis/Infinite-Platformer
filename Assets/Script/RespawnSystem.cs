using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    public void RespawnPressed()
    {
        AdsSystem.instance.onRewardCompleted += Respawn;
        AdsSystem.instance.LoadAd(AdsSystem.instance._rewardAdUnitId);
    }

    private void Respawn()
    {
        AdsSystem.instance.onRewardCompleted -= Respawn;
        GameManager.instance.ContinueGame();
    }
}

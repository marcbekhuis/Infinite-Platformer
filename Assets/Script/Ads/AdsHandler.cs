using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsHandler : IUnityAdsShowListener
{
    public delegate void AdComplited();
    private event AdComplited onAdFinish = delegate { };
    private event AdComplited onAdEnded = delegate { };
    private event AdComplited onAdSkipped = delegate { };
    private bool reward;

    public AdsHandler(AdComplited onAdFinish = null, AdComplited onAdEnded = null, AdComplited onAdSkipped = null, bool reward = false)
    {
        if(onAdEnded != null)
            this.onAdEnded += onAdEnded;
        if (onAdFinish != null)
            this.onAdFinish += onAdFinish;
        if (onAdSkipped != null)
            this.onAdSkipped += onAdSkipped;

        this.reward = reward;
        AdsSystem.instance.LoadAd(reward ? AdsSystem.instance._adUnitId : AdsSystem.instance._rewardAdUnitId, this);
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        AdsSystem.instance.adScore -= reward ? AdsSystem.instance.rewardScoreDecrease : AdsSystem.instance.periodicScoreDecrease;
        onAdEnded();
        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.SKIPPED:
                onAdSkipped();
                return;
            case UnityAdsShowCompletionState.COMPLETED:
                onAdFinish();
                return;
            case UnityAdsShowCompletionState.UNKNOWN:
                break;
            default:
                break;
        }
    }
}

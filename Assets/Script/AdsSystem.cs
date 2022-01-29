using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsSystem : Singleton<AdsSystem>, IUnityAdsShowListener
{
    [SerializeField] private int timeBetweenAds = 300;
    [SerializeField] private string androidGameID = "4459541";
    [SerializeField] private string iosGameID = "4459540";
    private float timeOfLastAd = 0;
    private string platform = "";

    public event RewardAdFinished onRewardAdFinished = delegate { };
    public delegate void RewardAdFinished();

    private void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (!Advertisement.isInitialized)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Advertisement.Initialize(androidGameID);
                platform = "_Android";
            }
            else
            {
                Advertisement.Initialize(iosGameID);
                platform = "_iOS";
            }
        }
    }

    public void ShowTimedAd()
    {
        if (Advertisement.isShowing)
            return;
        if (Time.time < timeOfLastAd + timeBetweenAds)
            return;
        if (!Advertisement.IsReady("Interstitial" + platform))
            return;
        Advertisement.Show("Interstitial" + platform);
        timeOfLastAd = Time.time;
    }

    public void RewardAd()
    {
        if (Advertisement.isShowing)
            return;
        if (!Advertisement.IsReady("Rewarded" + platform))
            return;
        Advertisement.Show("Rewarded" + platform);
        timeOfLastAd = Time.time;
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.SKIPPED:
                break;
            case UnityAdsShowCompletionState.COMPLETED:
                onRewardAdFinished();
                onRewardAdFinished = delegate { };
                break;
            case UnityAdsShowCompletionState.UNKNOWN:
                break;
            default:
                break;
        }
    }
}

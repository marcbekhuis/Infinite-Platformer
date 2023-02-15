using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsSystem : Singleton<AdsSystem>, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameID = "4459541";
    [SerializeField] private string iosGameID = "4459540";
    [SerializeField] private bool testMode = false;
    private string _gameId;

    [SerializeField] private string androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string iosAdUnitId = "Interstitial_iOS";
    public string _adUnitId { get; private set; }
    [SerializeField] private string rewardAndroidAdUnitId = "Rewarded_Android";
    [SerializeField] private string rewardIosAdUnitId = "Rewarded_iOS";
    public string _rewardAdUnitId { get;  private set; }

    public delegate void AdComplited();
    public event AdComplited onRewardCompleted = delegate { };
    public event AdComplited onAdShowCompleted = delegate { };

    protected override void Awake()
    {
        base.Awake();
        if (instance == this)
        {
            DontDestroyOnLoad(this);
            InitializeAds();
        }
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? iosGameID
            : androidGameID;
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? iosAdUnitId
            : androidAdUnitId;
        _rewardAdUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? rewardIosAdUnitId
            : rewardAndroidAdUnitId;

        if (!Advertisement.isInitialized)
            Advertisement.Initialize(_gameId, testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.LogError("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // Load content to the Ad Unit:
    public void LoadAd(string adUnitId)
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.LogError("Loading Ad: " + adUnitId);
        Advertisement.Load(adUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
    private void ShowAd(string adUnitId)
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.LogError("Showing Ad: " + adUnitId);
        Advertisement.Show(adUnitId, this);
    }

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
        ShowAd(adUnitId);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) 
    {
        onAdShowCompleted();
        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.SKIPPED:
                break;
            case UnityAdsShowCompletionState.COMPLETED:
                onRewardCompleted();
                break;
            case UnityAdsShowCompletionState.UNKNOWN:
                break;
            default:
                break;
        }
    }
}
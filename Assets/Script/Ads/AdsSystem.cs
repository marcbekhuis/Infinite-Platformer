using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsSystem : Singleton<AdsSystem>, IUnityAdsLoadListener, IUnityAdsInitializationListener
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

    public float periodicAdScore = 50;
    public float periodicScoreDecrease = 50;
    public float rewardScoreDecrease = 150;

    private float adScoreVar = 0;

    private Dictionary<string, IUnityAdsShowListener> listeners = new Dictionary<string, IUnityAdsShowListener>();

    public  float adScore
    {
        get => adScoreVar;
        set
        {
            adScoreVar = Mathf.Max(value, 0);
            PlayerPrefs.SetFloat("adScore", adScoreVar);
        }
    }

    public bool showAd { get => adScore > periodicAdScore; }

    private void Start()
    {
        if (PlayerPrefs.HasKey("adScore"))
            adScore = PlayerPrefs.GetFloat("adScore");
    }

    protected override void Awake()
    {
        base.Awake();
        if (instance == this)
        {
            DontDestroyOnLoad(this);
            InitializeAds();
        }
    }

    private void InitializeAds()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _gameId = iosGameID;
            _adUnitId = iosAdUnitId;
            _rewardAdUnitId = rewardIosAdUnitId;
        }
        else
        {
            _gameId = androidGameID;
            _adUnitId = androidAdUnitId;
            _rewardAdUnitId = rewardAndroidAdUnitId;
        }

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
    public void LoadAd(string adUnitId, IUnityAdsShowListener listener)
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.LogError("Loading Ad: " + adUnitId);

        listeners.Add(adUnitId, listener);
        Advertisement.Load(adUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
    private void ShowAd(string adUnitId)
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.LogError("Showing Ad: " + adUnitId);
        Advertisement.Show(adUnitId, listeners[adUnitId]);
        listeners.Remove(adUnitId);
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
}
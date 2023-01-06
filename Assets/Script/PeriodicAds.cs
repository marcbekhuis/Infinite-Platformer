using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicAds : MonoBehaviour
{
    [SerializeField] private float showAdScore = 2000;
    [SerializeField] private Player player;

    private float adScore = 0;

    public void ResetAd()
    {
        adScore = 0;
    }

    public void RetryPressed()
    {
        AddScore(player.transform.position.x, true);
    }

    public void HomePressed()
    {
        AddScore(player.transform.position.x);
    }

    public void AddScore(float additiveScore, bool instantPlay = false)
    {
        adScore += additiveScore;
        if(adScore > showAdScore)
        {
            if(instantPlay)
                AdsSystem.instance.onAdComplited += OnAdRetryComplete;
            else
                AdsSystem.instance.onAdComplited += OnAdHomeComplete;

            AdsSystem.instance.LoadAd(AdsSystem.instance._adUnitId);
        }
        else
        {
            if (instantPlay)
                Retry();
            else
                Home();
        }
    }

    private void OnAdRetryComplete()
    {
        Retry();
        adScore -= showAdScore;
        AdsSystem.instance.onAdComplited -= OnAdRetryComplete;
    }
    private void OnAdHomeComplete()
    {
        Home();
        adScore -= showAdScore;
        AdsSystem.instance.onAdComplited -= OnAdHomeComplete;
    }

    private void Retry()
    {
        GameManager.instance.Play();
        MapGenerator.instance.ResetMap();
        player.Respawn();
    }


    private void Home()
    {
        GameManager.instance.MainMenu();
        MapGenerator.instance.ResetMap();
        player.Respawn();
    }
}

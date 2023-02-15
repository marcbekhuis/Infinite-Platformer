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
        AddScore(GameManager.instance.playerDistance, true);
    }

    public void HomePressed()
    {
        AddScore(GameManager.instance.playerDistance);
    }

    public void AddScore(float additiveScore, bool instantPlay = false)
    {
        adScore += additiveScore;
        if(adScore > showAdScore)
        {
            if(instantPlay)
                AdsSystem.instance.onAdShowCompleted += OnAdRetryComplete;
            else
                AdsSystem.instance.onAdShowCompleted += OnAdHomeComplete;

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
        AdsSystem.instance.onAdShowCompleted -= OnAdRetryComplete;
    }
    private void OnAdHomeComplete()
    {
        Home();
        adScore -= showAdScore;
        AdsSystem.instance.onAdShowCompleted -= OnAdHomeComplete;
    }

    private void Retry()
    {
        GameManager.instance.ResetGame();
        GameManager.instance.Play();
        player.Respawn();
    }


    private void Home()
    {
        GameManager.instance.ResetGame();
        GameManager.instance.MainMenu();
        player.Respawn();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointManager : Singleton<PointManager>
{
    [SerializeField] private TMP_Text pointText;

    private int pointsVar;

    public int points
    {
        get { return pointsVar; }
        set { 
            pointsVar = value;
            PlayerPrefs.SetInt("playerPoints", points);
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        pointText.text = points.ToString();
    }

    private void Start()
    {
        pointsVar = PlayerPrefs.GetInt("playerPoints");
        UpdateDisplay();
    }
}

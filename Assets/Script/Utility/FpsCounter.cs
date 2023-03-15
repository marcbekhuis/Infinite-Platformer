using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text counter;

    // Update is called once per frame
    void Update()
    {
        counter.text = (1 / Time.deltaTime).ToString("F0");
    }
}

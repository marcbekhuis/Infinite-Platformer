using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanels : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;

    public void SetActivePanel(GameObject panel)
    {
        foreach (var item in panels)
        {
            item.SetActive(panel == item);
        }
    }
}

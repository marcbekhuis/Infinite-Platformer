using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    protected void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = FindObjectOfType<T>();
    }
}

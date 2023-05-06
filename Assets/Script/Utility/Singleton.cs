using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;

    protected virtual void Awake()
    {
        if (instance != null) WhenSingletonInstanceAlreadyExists();
        else instance = FindObjectOfType<T>();
    }

    protected virtual void WhenSingletonInstanceAlreadyExists() => Destroy(this);
}

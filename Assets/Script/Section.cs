using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Section", menuName = "ScriptableObject/Section")]
public class Section : ScriptableObject
{
    public GameObject prefab;
    public float startHeight;
    public float endHeight;
    public float length;
}

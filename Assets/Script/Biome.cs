using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "ScriptableObject/Biome")]
public class Biome : ScriptableObject
{
    public int generationDistance = 0;
    public Section[] sections;

    public Section GetSection()
    {
        return sections[Random.Range(0, sections.Length)];
    }
}

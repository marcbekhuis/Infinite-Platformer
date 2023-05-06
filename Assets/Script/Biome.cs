using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "ScriptableObject/Biome")]
public class Biome : ScriptableObject
{
    public float minGenerationDistance = 0;
    public float maxGenerationDistance = 0;
    public int minSections = 3;
    public int maxSections = 10;
    public Section[] sections;
    public GameObject endMap;

    public Section GetSection(float playerDistance, float biomeDistance)
    {
        float distance = playerDistance - biomeDistance;
        List<Section> validSections = new List<Section>();
        foreach (var section in sections)
        {
            if (playerDistance < section.minGenerationDistance || playerDistance > section.maxGenerationDistance)
                continue;
            if (distance < section.minBiomeSpawnDistance || distance > section.maxBiomeSpawnDistance)
                continue;

            validSections.Add(section);
        }

        Debug.LogError("Valid Sections: " + validSections.Count);
        return validSections[Random.Range(0, validSections.Count)];
    }
}

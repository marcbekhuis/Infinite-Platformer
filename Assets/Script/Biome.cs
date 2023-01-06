using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "ScriptableObject/Biome")]
public class Biome : ScriptableObject
{
    public int minGenerationDistance = 0;
    public int maxGenerationDistance = 0;
    public int minSections = 3;
    public int maxSections = 10;
    public Section[] sections;

    public Section GetSection(float playerDistance, float biomeDistance)
    {
        float distance = playerDistance - biomeDistance;
        List<Section> validSections = new List<Section>();
        foreach (var section in sections)
        {
            if (distance >= section.minBiomeSpawnDistance && distance <= section.maxBiomeSpawnDistance)
            {
                validSections.Add(section);
                continue;
            }
        }

        return sections[Random.Range(0, validSections.Count)];
    }
}

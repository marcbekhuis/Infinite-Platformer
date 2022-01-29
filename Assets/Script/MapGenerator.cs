using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    [SerializeField] private Transform target;
    [SerializeField] private float distanceFromEndToSpawn = 10;
    [SerializeField] private Biome[] biomes;

    private float distance = -3;
    private Section lastSection;
    private float lastGameObjectHeight;

    private void Start()
    {
        GenerateSection();
    }

    private void Update()
    {
        if (distance - target.position.x <= distanceFromEndToSpawn)
            GenerateSection();
    }

    private Biome GetValidBiome()
    {
        Biome validBiome = null;
        foreach (var biome in biomes)
        {
            if(validBiome == null)
            {
                validBiome = biome;
                continue;
            }
            if(Mathf.Abs(distance - biome.generationDistance) < Mathf.Abs(distance - validBiome.generationDistance))
            {
                validBiome = biome;
            }
        }
        return validBiome;
    }

    public void GenerateSection()
    {
        Section section = GetValidBiome().GetSection();
        if (lastSection)
        {
            GameObject gameObject = Instantiate(section.prefab, new Vector3(distance, lastSection.endHeight - section.startHeight + lastGameObjectHeight, 0), new Quaternion(0, 0, 0, 0));
            lastGameObjectHeight = gameObject.transform.position.y;
        }
        else
        {
            GameObject gameObject = Instantiate(section.prefab, new Vector3(distance, 0, 0), new Quaternion(0, 0, 0, 0));
            lastGameObjectHeight = gameObject.transform.position.y;
        }

        lastSection = section;
        distance += section.length;
    }
}

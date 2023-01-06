using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    [SerializeField] private Transform target;
    [SerializeField] private float distanceFromEndToSpawn = 10;
    [SerializeField] private Biome[] biomes;

    private float distance = -3;
    private int biomeSections = 0;
    private Section lastSection;
    private Biome lastBiome;
    private float biomeDistance;
    private float lastGameObjectHeight;
    private List<GameObject> sections = new List<GameObject>();

    private void Start()
    {
        GenerateSection();
    }

    private void Update()
    {
        if (distance - target.position.x <= distanceFromEndToSpawn)
            GenerateSection();
    }

    public void ResetMap()
    {
        distance = -3;
        biomeSections = 0;
        lastSection = null;
        lastBiome = null;
        biomeDistance= 0;
        lastGameObjectHeight= 0;

        foreach (var section in sections)
        {
            Destroy(section);
        }

        sections.Clear();
        GenerateSection();
    }

    private Biome GetValidBiome()
    {
        if(lastBiome)
            if (biomeSections < lastBiome.minSections)
                return lastBiome;

        List<Biome> validBiomes = new List<Biome>();
        foreach (var biome in biomes)
        {
            if (biomeSections >= biome.maxSections && biome == lastBiome)
                continue;

            if(distance >= biome.minGenerationDistance && distance <= biome.maxGenerationDistance)
                validBiomes.Add(biome);
        }
        biomeDistance = distance;
        return validBiomes[Random.Range(0, validBiomes.Count)];
    }

    public void GenerateSection()
    {
        if (sections.Count > 4)
        {
            Destroy(sections[0]);
            sections.RemoveAt(0);
        }

        Section section = GetValidBiome().GetSection(distance, biomeDistance);
        if (lastSection)
        {
            GameObject gameObject = Instantiate(section.gameObject, new Vector3(distance, lastSection.endHeight - section.startHeight + lastGameObjectHeight, 0), new Quaternion(0, 0, 0, 0));
            sections.Add(gameObject);
            lastGameObjectHeight = gameObject.transform.position.y;
        }
        else
        {
            GameObject gameObject = Instantiate(section.gameObject, new Vector3(distance, 0, 0), new Quaternion(0, 0, 0, 0));
            sections.Add(gameObject);
            lastGameObjectHeight = gameObject.transform.position.y;
        }
        biomeSections++;
        lastSection = section;
        distance += section.length;
    }
}

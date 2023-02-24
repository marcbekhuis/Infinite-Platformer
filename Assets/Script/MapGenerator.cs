using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    [SerializeField] private Transform target;
    [SerializeField] private float distanceFromEndToSpawn = 10;
    [SerializeField] private Biome[] biomes;

    private float distance = 0;
    private int biomeSections = 0;
    private Section lastSection;
    private Biome lastBiome;
    private float biomeDistance;
    private float lastGameObjectHeight;
    private List<GameObject> sections = new List<GameObject>();
    private GameObject endMap;

    private void Start()
    {
        GameManager.instance.OnMoveMap += MoveMap;
        GameManager.instance.OnContinueGame += OnContinueGame;
        GenerateSection();
    }

    private void Update()
    {
        if (distance - GameManager.instance.playerDistance <= distanceFromEndToSpawn)
            GenerateSection();
    }

    public void MoveMap(Vector3 offset)
    {
        foreach (var section in sections)
        {
            section.transform.Translate(offset);
        }
        endMap.transform.Translate(offset);
    }

    public void ResetMap()
    {
        distance = 0;
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
        Biome newBiome = validBiomes[Random.Range(0, validBiomes.Count)];
        if (newBiome == lastBiome)
            return lastBiome;

        biomeDistance = distance;
        biomeSections = 0;
        return newBiome;
    }

    public void GenerateSection()
    {
        if (sections.Count > 1)
        {
            Destroy(sections[0]);
            sections.RemoveAt(0);
            endMap.transform.position = sections[0].transform.position;
        }

        Biome biome = GetValidBiome();
        Section section = biome.GetSection(distance, biomeDistance);
        if(biome != lastBiome)
        {
            if(endMap)
                Destroy(endMap);

            endMap = Instantiate(biome.endMap);
        }
        if (lastSection)
        {
            GameObject gameObject = Instantiate(section.gameObject, new Vector3(sections.Last().transform.position.x + lastSection.end.localPosition.x, lastSection.end.localPosition.y - section.start.localPosition.y + lastGameObjectHeight, 0), new Quaternion(0, 0, 0, 0));
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
        lastBiome = biome;
        distance += section.end.localPosition.x;
    }

    private void OnContinueGame()
    {
        Vector3 offset = Vector3.right * (GameManager.instance.lastSavePlayerDistance - GameManager.instance.playerDistance);
        foreach (var section in sections)
        {
            section.transform.Translate(offset);
        }
        endMap.transform.Translate(offset);
    }
}

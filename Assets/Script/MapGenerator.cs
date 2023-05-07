using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    [SerializeField] private Transform target;
    [SerializeField] private float distanceFromEndToSpawn = 10;
    [SerializeField] private float startDistance = -3;
    [SerializeField] private Biome[] biomes;

    private float distance = 0;
    private int biomeSections = 0;
    private Section lastSection;
    private Biome lastBiome;
    private float biomeDistance;
    private float lastGameObjectHeight;
    private List<GameObject> sections = new List<GameObject>();
    private GameObject endMap;
    private Vector3 offset = default;

    private void Start()
    {
        GameManager.instance.OnMoveMap += MoveMap;
        GameManager.instance.OnContinueGame += OnContinueGame;
        SetValues();
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
        MaterialTranslator.instance.SetOffset(-(this.offset += offset));
    }   

    private void SetValues()
    {
        distance = startDistance;
        biomeSections = 0;
        lastSection = null;
        lastBiome = null;
        biomeDistance = 0;
        lastGameObjectHeight = 0;
    }

    public void ResetMap()
    {
        SetValues();

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

        if (biomes.Length == 1)
            return biomes[0];

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
        Section section = biome.GetSection(distance, biomeDistance, lastSection);
        if(biome != lastBiome)
        {
            if(endMap)
                Destroy(endMap);

            Vector3 postion;
            if (lastSection)
                postion = new Vector3(sections.First().transform.position.x, lastSection.start.y, 0);
            else
                postion = new Vector3(distance, 0, 0);

            endMap = Instantiate(biome.endMap, postion, new Quaternion(0, 0, 0, 0));
        }
        {
            Vector3 postion;
            if (lastSection)
                postion = new Vector3(sections.Last().transform.position.x + lastSection.end.x, lastSection.end.y - section.start.y + lastGameObjectHeight, 0);
            else
                postion = new Vector3(distance, 0, 0);

            GameObject gameObject = Instantiate(section.gameObject, postion, new Quaternion(0, 0, 0, 0));
            sections.Add(gameObject);
            lastGameObjectHeight = gameObject.transform.position.y;
        }

        biomeSections++;
        lastSection = section;
        lastBiome = biome;
        distance += section.end.x;
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

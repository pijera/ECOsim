using UnityEngine;
using System.Collections.Generic;

public class ResourceSpawner : MonoBehaviour
{
    public SimulationSettings settings;
    public GameObject foodPrefab;
    public GameObject waterPrefab;
    public Transform[] spawnPoints;

    public Sprite[] pondSprites;
    
    private List<int> usedIndices = new List<int>();

    void Start()
    {
        SpawnResources(foodPrefab, SimulationSettings.Instance.foodSourceCount);
        SpawnResources(waterPrefab, SimulationSettings.Instance.waterSourceCount);
    }

    void SpawnResources(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int index = GetUniqueSpawnIndex();
            if (index == -1)
            {
                Debug.LogWarning("Not enough spawn points.");
                return;
            }

            Instantiate(prefab, spawnPoints[index].position, Quaternion.identity);
            if (prefab==waterPrefab)
            {
                int rand  = Random.Range(0,pondSprites.Length);
                prefab.GetComponent<SpriteRenderer>().sprite = pondSprites[rand];
            }
        }
    }

    int GetUniqueSpawnIndex()
    {
        if (usedIndices.Count >= spawnPoints.Length)
            return -1;

        int index;
        do
        {
            index = Random.Range(0, spawnPoints.Length);
        } while (usedIndices.Contains(index));

        usedIndices.Add(index);
        return index;
    }
}
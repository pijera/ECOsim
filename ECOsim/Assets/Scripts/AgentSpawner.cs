using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public GameObject agentPrefab;

    public Vector2 minSpawnBounds;
    public Vector2 maxSpawnBounds;

    
    void Start()
    {
        
        int agentCount = SimulationSettings.Instance != null ? SimulationSettings.Instance.numberOfAgents : 10;
        
        
        
        
        
        for (int i = 0; i < agentCount; i++)
        {
            BiljojedAI agentScript  = agentPrefab.GetComponent<BiljojedAI>();
        
            agentScript.moveSpeed = Random.Range(SimulationSettings.Instance.minSpeed, SimulationSettings.Instance.maxSpeed);
            agentScript.sightRange = Random.Range(SimulationSettings.Instance.minSightRange, SimulationSettings.Instance.maxSightRange);
            agentScript.readyToReproduceRate = Random.Range(SimulationSettings.Instance.minReadyToReproduceRate, SimulationSettings.Instance.maxReadyToReproduceRate);
            agentScript.readyToReproduceValue = Random.Range(SimulationSettings.Instance.minReadyToReproduceValue, SimulationSettings.Instance.maxReadyToReproduceValue);
            agentScript.lifespan = Random.Range(SimulationSettings.Instance.minLifespan, SimulationSettings.Instance.maxLifespan);
            agentScript.numbOfChildren = Random.Range(1, (int)Mathf.Round(SimulationSettings.Instance.maxNumbOfChildren)+1);
            
            
            Vector2 position = new Vector2(
                Random.Range(minSpawnBounds.x, maxSpawnBounds.x),
                Random.Range(minSpawnBounds.y, maxSpawnBounds.y)
            );

            Instantiate(agentPrefab, position, Quaternion.identity);
        }
        
    }
}

using UnityEngine;

public class SimulationSettings : MonoBehaviour
{
    public static SimulationSettings Instance;

    public int numberOfAgents = 10; // Default
    public float minSpeed = 1.5f;
    public float maxSpeed = 4f;
    public float minSightRange = 2.5f;
    public float maxSightRange = 4f;
    public float minReadyToReproduceRate = 10;
    public float maxReadyToReproduceRate = 15;
    public float minReadyToReproduceValue = 100;
    public float maxReadyToReproduceValue = 200;
    public float minLifespan = 15f;
    public float maxLifespan = 40;
    public float maxNumbOfChildren = 3;
    
    public int foodSourceCount = 5;
    public int waterSourceCount = 3;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }
}

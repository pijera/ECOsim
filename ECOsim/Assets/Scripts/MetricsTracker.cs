using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MetricsTracker : MonoBehaviour
{
    public CSVLogger csvLogger;
    public float logInterval = 10f;
    private float timeElapsed = 0f;

    void Start()
    {
        StartCoroutine(LogMetrics());
    }

    IEnumerator LogMetrics()
    {
        // Loguje sve odma na pocetku
        Dictionary<string, float> initialMetrics = CalculateMetrics();
        csvLogger.LogData(initialMetrics, timeElapsed);
        timeElapsed += logInterval;

        while (true)
        {
            yield return new WaitForSeconds(logInterval);
            Dictionary<string, float> metrics = CalculateMetrics();
            csvLogger.LogData(metrics, timeElapsed);
            timeElapsed += logInterval;
        }
    }

    Dictionary<string, float> CalculateMetrics()
    {
        BiljojedAI[] agents = FindObjectsOfType<BiljojedAI>();
        float totalSpeed = 0f;
        float totalSight = 0f;
        float totalHungerRate = 0f;
        float totalThirstRate = 0f;
        float totalLifespan = 0f;
        float totalReadyToReproduceRate = 0f;
        float totalReadyToReproduceValue = 0f;
        float totalNumbOfChildren = 0f;
        
        foreach (var agent in agents)
        {
            totalSpeed += agent.moveSpeed;
        }

        foreach (var agent in agents)
        {
            totalSight += agent.sightRange;
        }
        foreach (var agent in agents)
        {
            totalHungerRate += agent.hungerRate;
        }
        foreach (var agent in agents)
        {
            totalThirstRate += agent.thirstRate;
        } 
        foreach (var agent in agents)
        {
            totalLifespan += agent.lifespanValue;
        }
        foreach (var agent in agents)
        {
            totalReadyToReproduceRate += agent.readyToReproduceRate;
        }
        foreach (var agent in agents)
        {
            totalReadyToReproduceValue += agent.readyToReproduceValue;
        }
        foreach (var agent in agents)
        {
            totalNumbOfChildren += agent.numbOfChildren;
        }
        
        
        float averageSpeed = agents.Length > 0 ? totalSpeed / agents.Length : 0f;
        float averageSight = agents.Length > 0 ? totalSight / agents.Length : 0f;
        float averageHungerRate = agents.Length > 0 ? totalHungerRate / agents.Length : 0f;
        float averageThirstRate = agents.Length > 0 ? totalThirstRate / agents.Length : 0f;
        float averageLifespan = agents.Length > 0 ? totalLifespan / agents.Length : 0f;
        float averageReadyToReproduceRate = agents.Length > 0 ? totalReadyToReproduceRate / agents.Length : 0f;
        float averageReadyToReproduceValue = agents.Length > 0 ? totalReadyToReproduceValue / agents.Length : 0f;
        float averageNumbOfChildren = agents.Length > 0 ? totalNumbOfChildren / agents.Length : 0f;
        
        return new Dictionary<string, float>
        {
            { "Population", agents.Length },
            { "AvgSpeed", averageSpeed },
            {"AvgSight", averageSight },
            {"AvgHungerRate", averageHungerRate },
            {"AvgThirstRate", averageThirstRate },
            {"AvgLifespan", averageLifespan },
            {"AvgReadyToReprduceRate", averageReadyToReproduceRate },
            {"AvgReadyToReprduceValue", averageReadyToReproduceValue },
            {"AvgNumberOfChildren", averageNumbOfChildren },
        };
    }
}
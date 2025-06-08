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
        // Log immediately at start
        Dictionary<string, float> initialMetrics = CalculateMetrics();
        csvLogger.LogData(initialMetrics, timeElapsed);
        timeElapsed += logInterval;

        while (true)
        {
            yield return new WaitForSecondsRealtime(logInterval);
            Dictionary<string, float> metrics = CalculateMetrics();
            csvLogger.LogData(metrics, timeElapsed);
            timeElapsed += logInterval;
        }
    }

    Dictionary<string, float> CalculateMetrics()
    {
        BiljojedAI[] agents = FindObjectsOfType<BiljojedAI>();
        float totalSpeed = 0f;

        foreach (var agent in agents)
        {
            totalSpeed += agent.moveSpeed;
        }

        float averageSpeed = agents.Length > 0 ? totalSpeed / agents.Length : 0f;

        return new Dictionary<string, float>
        {
            { "AvgSpeed", averageSpeed },
            { "Population", agents.Length }
        };
    }
}
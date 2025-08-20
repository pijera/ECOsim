using UnityEngine;

public enum WeatherType{Clear,Rain}
public class WeatherSystem : MonoBehaviour
{
    public static WeatherSystem instance;
    public WeatherType currentWeather;

    public GameObject rainEffects;
    [SerializeField]
    private ParticleSystem[] rainSystems;
    
    public float growthMultiplier;
    public float sightMultiplier;
    public float speedMultiplier;
    
    void Awake()
    {
        instance = this;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleWeather(WeatherType.Rain);
        }
    }

    void ToggleWeather(WeatherType type)
    {
        if (currentWeather==type) setWeather(WeatherType.Clear);
        else setWeather(type);
    }

    void setWeather(WeatherType type)
    {
        stopParticleSystems(rainSystems,rainEffects);
        currentWeather = type;
        
        switch (type)
        {
            case WeatherType.Clear:
            {
                foreach (var plant in FindObjectsOfType<FoodGrowth>())
                {
                    plant.timeToGrow /= growthMultiplier;
                }
                foreach (var animal in FindObjectsOfType<BiljojedAI>())
                {
                    animal.moveSpeed /= speedMultiplier;
                    animal.sightRange /= sightMultiplier;
                }
                break;
            }
            case WeatherType.Rain:
            {
                rainEffects.SetActive(true);
                startParticleSystems(rainSystems);
                foreach (var plant in FindObjectsOfType<FoodGrowth>())
                {
                    plant.timeToGrow *= growthMultiplier;
                }
                foreach (var animal in FindObjectsOfType<BiljojedAI>())
                {
                    animal.moveSpeed *= speedMultiplier;
                    animal.sightRange *= sightMultiplier;
                }
                break;
            }
        }
    }

    void stopParticleSystems(ParticleSystem[] particleSystems,GameObject effectObject)
    {
        foreach (var ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    void startParticleSystems(ParticleSystem[] particleSystems)
    {
        foreach (var ps in particleSystems)
        {
            ps.Play();
        }
    }
}

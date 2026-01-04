using System;
using UnityEngine;

public enum WeatherType{Clear,Rain,Snow,Dry}
public class WeatherSystem : MonoBehaviour
{
    public static WeatherSystem instance;
    public WeatherType currentWeather;
    public WeatherType StartWeatherType;
    
    public GameObject rainEffects;
    [SerializeField]
    private ParticleSystem[] rainSystems;
    public float rainGrowthMultiplier;
    public float rainSightMultiplier;
    public float rainSpeedMultiplier;
    
    public GameObject snowEffects;
    [SerializeField]
    private ParticleSystem[] snowSystems;
    public float snowGrowthMultiplier;
    public float snowFoodRateMultiplier;
    public float snowReprodceRateMultiplier;
    
    public GameObject droughtEffects;
    [SerializeField]
    private ParticleSystem[] droughtSystems;
    public float droughtGrowthMultiplier;
    public float droughtThirstMultiplier;
    
    
    void Awake()
    {
        instance = this;
        setWeather(StartWeatherType);
        currentWeather = StartWeatherType;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleWeather(WeatherType.Rain);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleWeather(WeatherType.Snow);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ToggleWeather(WeatherType.Dry);
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
        stopParticleSystems(snowSystems,snowEffects);
        currentWeather = type;
        
        switch (type)
        {
            case WeatherType.Clear:
            {
                foreach (var plant in FindObjectsOfType<FoodGrowth>())
                {
                    plant.timeToGrow = plant.baseTimeToGrow;
                }
                foreach (var animal in FindObjectsOfType<BiljojedAI>())
                {
                    animal.moveSpeed = animal.baseMoveSpeed;
                    animal.sightRange = animal.baseSightRange;
                }
                break;
            }
            case WeatherType.Rain:
            {
                rainEffects.SetActive(true);
                startParticleSystems(rainSystems);
                foreach (var plant in FindObjectsOfType<FoodGrowth>())
                {
                    plant.timeToGrow *= rainGrowthMultiplier;
                }
                foreach (var animal in FindObjectsOfType<BiljojedAI>())
                {
                    animal.moveSpeed *= rainSpeedMultiplier;
                    animal.sightRange *= rainSightMultiplier;
                }
                break;
            }
            case WeatherType.Snow:
            {
                snowEffects.SetActive(true);
                startParticleSystems(snowSystems);
                foreach (var plant in FindObjectsOfType<FoodGrowth>())
                {
                    plant.timeToGrow *= snowGrowthMultiplier;
                }

                foreach (var animal in FindObjectsOfType<BiljojedAI>())
                {
                    animal.hungerRate *= snowFoodRateMultiplier;
                    animal.readyToReproduceRate *= snowReprodceRateMultiplier;
                }
                break;
            }
            case WeatherType.Dry:
            {
                droughtEffects.SetActive(true);
                startParticleSystems(droughtSystems);
                foreach (var plant in FindObjectsOfType<FoodGrowth>())
                {
                    plant.timeToGrow *= droughtGrowthMultiplier;
                }

                foreach (var animal in FindObjectsOfType<BiljojedAI>())
                {
                   animal.thirstRate *= droughtThirstMultiplier;
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

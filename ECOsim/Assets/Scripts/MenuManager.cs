using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField agentCountInput;
    
    public TMP_InputField minBrzinaInput;
    public TMP_InputField maxBrzinaInput;

    public TMP_InputField minSightRangeInput;
    public TMP_InputField maxSightRangeInput;
    
    public TMP_InputField minReadyToReproduceRateInput;
    public TMP_InputField maxReadyToReproduceRateInput;

    public TMP_InputField minReadyToReproduceValueInput;
    public TMP_InputField maxReadyToReproduceValueInput;
    
    public TMP_InputField minLifespanInput;
    public TMP_InputField maxLifespanInput;

    public TMP_InputField maxNumbOfChildrenInput;

    public TMP_InputField foodInput;
    public TMP_InputField waterInput;
    
    public TMP_Dropdown dropdown;

    private string selectedScene;

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnMapSelected);
    }

    public void StartSimulation()
    {
        int count = 10; // Default fallback

        if (int.TryParse(agentCountInput.text, out int parsed))
        {
            count = Mathf.Clamp(parsed, 1, 1000); // Prevent extremes
        }
        
        float.TryParse(minBrzinaInput.text, out float minBrzina);
        float.TryParse(maxBrzinaInput.text, out float maxBrzina);
        float.TryParse(minSightRangeInput.text, out float minSightRange);
        float.TryParse(maxSightRangeInput.text, out float maxSightRange);
        float.TryParse(minReadyToReproduceRateInput.text, out float minReadyToReproduceRate);
        float.TryParse(maxReadyToReproduceRateInput.text, out float maxReadyToReproduceRate);
        float.TryParse(minLifespanInput.text, out float minLifespan);
        float.TryParse(maxLifespanInput.text, out float maxLifespan);
        float.TryParse(maxNumbOfChildrenInput.text, out float maxNumbOfChildren);
        float.TryParse(minReadyToReproduceValueInput.text, out float minReadyToReproduceValue);
        float.TryParse(maxReadyToReproduceValueInput.text, out float maxReadyToReproduceValue);
        
        SimulationSettings.Instance.numberOfAgents = count;
        SimulationSettings.Instance.minSpeed = minBrzina;
        SimulationSettings.Instance.maxSpeed = maxBrzina;
        SimulationSettings.Instance.minSightRange = minSightRange;
        SimulationSettings.Instance.maxSightRange = maxSightRange;
        SimulationSettings.Instance.minReadyToReproduceRate = minReadyToReproduceRate;
        SimulationSettings.Instance.maxReadyToReproduceRate = maxReadyToReproduceRate;
        SimulationSettings.Instance.minLifespan = minLifespan;
        SimulationSettings.Instance.maxLifespan = maxLifespan;
        SimulationSettings.Instance.maxNumbOfChildren = maxNumbOfChildren;
        SimulationSettings.Instance.minReadyToReproduceValue = minReadyToReproduceValue;
        SimulationSettings.Instance.maxReadyToReproduceValue = maxReadyToReproduceValue;
        SimulationSettings.Instance.foodSourceCount = int.Parse(foodInput.text);
        SimulationSettings.Instance.waterSourceCount = int.Parse(waterInput.text);

        SceneManager.LoadScene(selectedScene); 
    }

    void OnMapSelected(int index)
    {
        switch (index)
        {
            case 1: selectedScene = "Polje"; break;
            case 2: selectedScene = "Pustinja"; break;
            case 3: selectedScene = "Tundra"; break;
        }
    }
}

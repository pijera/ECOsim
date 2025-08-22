using TMPro;
using UnityEngine;

public class CountPopulation : MonoBehaviour
{
    public TMP_Text populationText;
    
    // Update is called once per frame
    void Update()
    {
        int population = FindObjectsOfType<BiljojedAI>().Length;
        populationText.text = population.ToString();
    }
}

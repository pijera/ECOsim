using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGrowth : MonoBehaviour
{
    public int numbOfFruits;
    public int maxNumbOfFruits = 3;
    public float timeToGrow;

    public bool isGrowing;
    public GameObject Food;
    
    void Start()
    {
        StartCoroutine(Grow());
    }
    
    void Update()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        switch (numbOfFruits)
        {
            case 0:
                Food.GetComponent<Renderer>().material.color = Color.gray;
                break;
            case 1:
                Food.GetComponent<Renderer>().material.color = Color.red;
                break;
            case 2:
                Food.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case 3:
                Food.GetComponent<Renderer>().material.color = Color.green;
                break;
        }
    }

    
    IEnumerator Grow()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(timeToGrow);
            if (numbOfFruits < maxNumbOfFruits)
            {
                numbOfFruits++;
            }
        }
    }
    
    public bool TryConsumeFruit()
    {
        if (numbOfFruits > 0)
        {
            numbOfFruits--;
            return true;
        }
        return false;
    }

}

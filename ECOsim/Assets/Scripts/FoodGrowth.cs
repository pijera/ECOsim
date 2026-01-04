using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGrowth : MonoBehaviour
{
    public int numbOfFruits;
    public int maxNumbOfFruits = 3;
    public float timeToGrow;
    [HideInInspector]
    public float baseTimeToGrow;
    
    public bool isGrowing;
    public GameObject Food;

    public Sprite[] GrassBlades3;
    public Sprite[] GrassBlades2;
    public Sprite[] GrassBlades1;
    public Sprite[] GrassBlades0;

    private int randomGrass;
    
    void Start()
    {
        baseTimeToGrow = timeToGrow;
        StartCoroutine(Grow());
        randomGrass = Random.Range(0,GrassBlades2.Length);
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
                Food.GetComponent<SpriteRenderer>().sprite = GrassBlades0[randomGrass];
                break;
            case 1:
                Food.GetComponent<SpriteRenderer>().sprite = GrassBlades1[randomGrass];
                break;
            case 2:
                Food.GetComponent<SpriteRenderer>().sprite = GrassBlades2[randomGrass];
                break;
            case 3:
                Food.GetComponent<SpriteRenderer>().sprite = GrassBlades3[randomGrass];
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

using UnityEngine;

public class RandomRockPicker : MonoBehaviour
{
    public Sprite[] rocks;
    
    void Start()
    {
        int rand = Random.Range(0, rocks.Length);
        gameObject.GetComponent<SpriteRenderer>().sprite = rocks[rand];
    }
}

using TMPro;
using UnityEngine;

public class GameSpeedChanger : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pauseBars;
    private int currentTimeScale;
    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;
            pauseBars.SetActive(isPaused);   
            Time.timeScale = isPaused ? 0 : currentTimeScale;
        }
    }
    public void times1GameSpeed()
    {
        Time.timeScale = 1;
        currentTimeScale = 1;
    }
    public void times3GameSpeed()
    {
        Time.timeScale = 3;
        currentTimeScale = 3;
    }
    public void times5GameSpeed()
    {
        Time.timeScale = 5;
        currentTimeScale = 5;
    }
    public void times10GameSpeed()
    {
        Time.timeScale = 10;
        currentTimeScale = 10;
    }
}

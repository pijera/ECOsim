using UnityEngine;

public class GameSpeedChanger : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pauseBars;

    private int currentTimeScale = 1;

    void Start()
    {
        Time.timeScale = currentTimeScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pauseBars.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : currentTimeScale;
    }

    public void SetGameSpeed(int speed)
    {
        Debug.Log("radi");
        
        currentTimeScale = speed;
        
        if (!isPaused)
            Time.timeScale = currentTimeScale;
    }

    public void times1GameSpeed()  => SetGameSpeed(1);
    public void times3GameSpeed()  => SetGameSpeed(3);
    public void times5GameSpeed()  => SetGameSpeed(5);
    public void times10GameSpeed() => SetGameSpeed(10);
}
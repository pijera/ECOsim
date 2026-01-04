using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelection : MonoBehaviour
{
    public Dropdown dropdown;

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnMapSelected);
    }

    void OnMapSelected(int index)
    {
        switch (index)
        {
            case 1: SceneManager.LoadScene("Polje"); break;
            case 2: SceneManager.LoadScene("Pustinja"); break;
            case 3: SceneManager.LoadScene("Tundra"); break;
        }
    }
}
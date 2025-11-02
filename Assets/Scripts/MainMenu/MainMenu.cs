using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadARScene()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadVRScene()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}

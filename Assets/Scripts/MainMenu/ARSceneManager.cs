using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene navigation for the AR Scene.
/// Provides functionality to return to the Main Menu.
/// </summary>
public class ARSceneManager : MonoBehaviour
{
    /// <summary>
    /// Returns to the Main Menu scene (build index 0).
    /// Call this method from UI button onClick events.
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Load Main Menu scene (assumed to be at build index 0)
        // Note: If scenes are reordered in Build Settings, update this index
        SceneManager.LoadScene(0);
    }
}

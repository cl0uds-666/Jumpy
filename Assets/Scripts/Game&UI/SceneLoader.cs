using UnityEngine;
using UnityEngine.SceneManagement; // Required for managing scenes

public class SceneLoader : MonoBehaviour
{
    // This function can be called by UI elements like buttons.
    public void ReloadCurrentScene()
    {
        // First, make sure time is back to normal before reloading.
        Time.timeScale = 1f;

        // Get the index of the currently active scene.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the scene using its index.
        SceneManager.LoadScene(currentSceneIndex);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The main menu panel with the Play and Quit buttons.")]
    [SerializeField] private GameObject mainMenuPanel;
    [Tooltip("The shop panel that you want to show.")]
    [SerializeField] private GameObject shopPanel;

    [Header("Scene To Load")]
    [Tooltip("The exact name of your main gameplay scene file.")]
    [SerializeField] private string gameSceneName = "MainGame"; // IMPORTANT: Change this to your scene's name

    void Start()
    {
        // At the start, ensure the main menu is visible and the shop is hidden.
        mainMenuPanel.SetActive(true);
        shopPanel.SetActive(false);
    }

    /// <summary>
    /// Called by the Play button.
    /// </summary>
    public void PlayGame()
    {
        // Loads the main gameplay scene.
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Called by the Shop button.
    /// </summary>
    public void OpenShop()
    {
        // Hides the main menu and shows the shop.
        mainMenuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    /// <summary>
    /// Called by the Quit button.
    /// </summary>
    public void QuitGame()
    {
        // This will only work in a built game, not in the editor.
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    // You will need a "Back" button inside your ShopPanel that calls this function.
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}

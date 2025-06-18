using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The main menu panel with the Play and Quit buttons.")]
    [SerializeField] private GameObject mainMenuPanel;
    [Tooltip("The shop panel.")]
    [SerializeField] private GameObject shopPanel;
    [Tooltip("The settings/options panel.")]
    [SerializeField] private GameObject optionsPanel; // NEW

    [Header("Scene To Load")]
    [SerializeField] private string gameSceneName = "MainGame";

    void Start()
    {
        // At the start, ensure only the main menu is visible.
        mainMenuPanel.SetActive(true);
        shopPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenShop()
    {
        mainMenuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // --- NEW METHODS for Options Panel ---
    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    // --- End of New Methods ---

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}

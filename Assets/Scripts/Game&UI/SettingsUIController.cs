using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script now only manages the Momentum button.
public class SettingsUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The Button for toggling airborne momentum.")]
    [SerializeField] private Button momentumButton;
    [Tooltip("The TextMeshPro text object that is a child of the momentum button.")]
    [SerializeField] private TextMeshProUGUI momentumButtonText;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("FATAL ERROR: Could not find PlayerController. Settings will not work.");
            return;
        }

        // Initialize the GameSettings manager with only the momentum setting.
        GameSettings.Instance.Initialize(playerController.inheritAirborneMomentum);

        // Hook up the button click listener.
        momentumButton.onClick.RemoveAllListeners();
        momentumButton.onClick.AddListener(ToggleMomentumSetting);

        // Update the button text.
        UpdateButtonText();
    }

    private void ToggleMomentumSetting()
    {
        bool newValue = !GameSettings.Instance.InheritAirborneMomentum;
        GameSettings.Instance.SetInheritAirborneMomentum(newValue);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        momentumButtonText.text = "Momentum: " + (GameSettings.Instance.InheritAirborneMomentum ? "ON" : "OFF");
    }
}

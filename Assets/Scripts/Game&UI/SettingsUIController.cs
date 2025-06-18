using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider ambianceVolumeSlider;

    [Header("Gameplay Toggles")]
    [SerializeField] private Toggle hapticsToggle;
    [SerializeField] private Toggle vfxToggle;

    [Header("Reset Functionality")] // NEW SECTION
    [Tooltip("The pop-up panel that asks for confirmation.")]
    [SerializeField] private GameObject confirmationPanel;
    [Tooltip("The button in the main settings that opens the confirmation pop-up.")]
    [SerializeField] private Button resetButton;
    [Tooltip("The button inside the pop-up that confirms the reset.")]
    [SerializeField] private Button confirmResetButton;
    [Tooltip("The button inside the pop-up that cancels the reset.")]
    [SerializeField] private Button cancelResetButton;

    void Start()
    {
        // Hide the confirmation panel at the start.
        confirmationPanel.SetActive(false);

        SetupInitialValues();
        AddListeners();
    }

    private void SetupInitialValues()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1f);
        ambianceVolumeSlider.value = PlayerPrefs.GetFloat("AmbianceVolume", 1f);
        hapticsToggle.isOn = PlayerPrefs.GetInt("HapticsEnabled", 1) == 1;
        vfxToggle.isOn = PlayerPrefs.GetInt("VfxEnabled", 1) == 1;
    }

    private void AddListeners()
    {
        masterVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSfxVolume);
        ambianceVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetAmbianceVolume);
        hapticsToggle.onValueChanged.AddListener(SettingsManager.Instance.SetHapticsEnabled);
        vfxToggle.onValueChanged.AddListener(SettingsManager.Instance.SetVfxEnabled);

        // --- NEW Listeners for Reset Buttons ---
        resetButton.onClick.AddListener(OpenConfirmation);
        cancelResetButton.onClick.AddListener(CloseConfirmation);
        confirmResetButton.onClick.AddListener(SettingsManager.Instance.ResetAllData);
    }

    // --- NEW Methods for Confirmation Panel ---
    private void OpenConfirmation()
    {
        confirmationPanel.SetActive(true);
    }

    private void CloseConfirmation()
    {
        confirmationPanel.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainMixer;

    public bool HapticsEnabled { get; private set; }
    public bool VfxEnabled { get; private set; }

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string AmbianceVolumeKey = "AmbianceVolume";
    private const string HapticsKey = "HapticsEnabled";
    private const string VfxKey = "VfxEnabled";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); LoadSettings(); }
    }

    // --- UPDATED VOLUME METHODS ---
    public void SetMasterVolume(float sliderValue)
    {
        // THE FIX: We clamp the value to prevent Log10(0).
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(MasterVolumeKey, sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(MusicVolumeKey, sliderValue);
    }

    public void SetSfxVolume(float sliderValue)
    {
        mainMixer.SetFloat("SfxVolume", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(SfxVolumeKey, sliderValue);
    }

    public void SetAmbianceVolume(float sliderValue)
    {
        mainMixer.SetFloat("AmbianceVolume", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(AmbianceVolumeKey, sliderValue);
    }
    
    // --- Unchanged Methods Below ---
    #region Unchanged Methods
    public void SetHapticsEnabled(bool isEnabled)
    {
        HapticsEnabled = isEnabled;
        PlayerPrefs.SetInt(HapticsKey, isEnabled ? 1 : 0);
    }

    public void SetVfxEnabled(bool isEnabled)
    {
        VfxEnabled = isEnabled;
        PlayerPrefs.SetInt(VfxKey, isEnabled ? 1 : 0);
    }

    private void LoadSettings()
    {
        float masterVol = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        float ambianceVol = PlayerPrefs.GetFloat(AmbianceVolumeKey, 1f);
        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSfxVolume(sfxVol);
        SetAmbianceVolume(ambianceVol);
        HapticsEnabled = PlayerPrefs.GetInt(HapticsKey, 1) == 1;
        VfxEnabled = PlayerPrefs.GetInt(VfxKey, 1) == 1;
    }

    public void ResetAllData()
    {
        Debug.Log("Resetting all player data...");
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}

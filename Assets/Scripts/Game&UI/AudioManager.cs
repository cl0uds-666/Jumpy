using UnityEngine;
using UnityEngine.Audio; // Required for using Audio Mixer Groups

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer Groups")]
    [Tooltip("The Mixer Group for background music.")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [Tooltip("The Mixer Group for player-related sound effects (jump, land, etc.).")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [Tooltip("The Mixer Group for ambient sounds (rain, wind).")]
    [SerializeField] private AudioMixerGroup ambianceMixerGroup;

    [Header("Audio Clips - Loops")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip fallingWindSound;
    [SerializeField] private AudioClip rainSound;

    // We no longer need the individual SFX clips here, as they are handled by the PlayerSoundManager.

    // --- Private Audio Sources ---
    private AudioSource musicSource;
    private AudioSource windSource;
    private AudioSource rainSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }

        // --- Setup Audio Sources and Route them to the Mixer ---
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = musicMixerGroup; // Route to Music group

        windSource = gameObject.AddComponent<AudioSource>();
        windSource.clip = fallingWindSound;
        windSource.loop = true;
        windSource.outputAudioMixerGroup = ambianceMixerGroup; // Route to Ambiance group

        rainSource = gameObject.AddComponent<AudioSource>();
        rainSource.clip = rainSound;
        rainSource.loop = true;
        rainSource.outputAudioMixerGroup = ambianceMixerGroup; // Route to Ambiance group
    }

    void Start()
    {
        // Play the looping sounds. Their volume is now controlled entirely by the mixer.
        if (musicSource.clip != null) musicSource.Play();
        if (rainSource.clip != null) rainSource.Play();
    }

    // --- Public methods for controlling looping sounds ---
    public void StartFallingWind()
    {
        if (windSource != null && !windSource.isPlaying)
        {
            windSource.Play();
        }
    }

    public void StopFallingWind()
    {
        if (windSource != null && windSource.isPlaying)
        {
            windSource.Stop();
        }
    }

    // We also need to update the PlayerSoundManager to use the SFX group.
}

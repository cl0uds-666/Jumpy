using UnityEngine;
using System.Collections; // Required for using Coroutines

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips - SFX")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip highScoreSound;
    [SerializeField] private AudioClip explosionSound;

    [Header("Audio Clips - Loops")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip fallingWindSound;

    [Header("Volume & Mixing Controls")]
    [Tooltip("The normal volume for the background music (0.0 to 1.0).")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolumeNormal = 0.5f;

    [Tooltip("The reduced volume for the music when wind is playing (0.0 to 1.0).")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolumeDucked = 0.1f;

    [Tooltip("How quickly the music volume changes.")]
    [SerializeField] private float duckingSpeed = 3f;

    [Tooltip("Boost the volume of the falling wind sound (1 is normal).")]
    [Range(0f, 5f)]
    [SerializeField] private float fallingWindVolume = 2.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource loopingSfxSource;
    private Coroutine volumeCoroutine; // Holds our currently running volume-change coroutine

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        loopingSfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;

        loopingSfxSource.clip = fallingWindSound;
        loopingSfxSource.loop = true;

        // Set the initial music volume
        musicSource.volume = musicVolumeNormal;
    }

    void Start()
    {
        if (musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    public void PlayJumpSound() { if (jumpSound != null) sfxSource.PlayOneShot(jumpSound); }
    public void PlayLandingSound() { if (landingSound != null) sfxSource.PlayOneShot(landingSound); }
    public void PlayHighScoreSound() { if (highScoreSound != null) sfxSource.PlayOneShot(highScoreSound); }
    public void PlayExplosionSound() { if (explosionSound != null) sfxSource.PlayOneShot(explosionSound); }

    public void StartFallingWind()
    {
        if (fallingWindSound != null && !loopingSfxSource.isPlaying)
        {
            Debug.Log("AUDIO DEBUG: Trying to PLAY falling wind sound.");
            loopingSfxSource.volume = fallingWindVolume;
            loopingSfxSource.Play();

            // --- DUCK THE MUSIC ---
            FadeMusic(musicVolumeDucked);
        }
    }

    public void StopFallingWind()
    {
        if (loopingSfxSource.isPlaying)
        {
            Debug.Log("AUDIO DEBUG: Trying to STOP falling wind sound.");
            loopingSfxSource.Stop();
            loopingSfxSource.volume = 1f;

            // --- RESTORE THE MUSIC ---
            FadeMusic(musicVolumeNormal);
        }
    }

    /// <summary>
    /// Starts a coroutine to smoothly fade the music volume to a target level.
    /// </summary>
    private void FadeMusic(float targetVolume)
    {
        // If a volume coroutine is already running, stop it first.
        if (volumeCoroutine != null)
        {
            StopCoroutine(volumeCoroutine);
        }
        // Start the new fade coroutine.
        volumeCoroutine = StartCoroutine(FadeVolumeCoroutine(targetVolume));
    }

    /// <summary>
    /// A coroutine that smoothly changes the music volume over time.
    /// </summary>
    private IEnumerator FadeVolumeCoroutine(float targetVolume)
    {
        float startVolume = musicSource.volume;
        float time = 0;

        while (time < 1f)
        {
            // Increase time based on our ducking speed.
            time += Time.deltaTime * duckingSpeed;
            // Lerp (linearly interpolate) from the start volume to the target volume.
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, time);
            yield return null; // Wait for the next frame
        }

        // Ensure the volume is exactly at the target at the end.
        musicSource.volume = targetVolume;
    }
}

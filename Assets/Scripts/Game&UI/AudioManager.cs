using UnityEngine;
using System.Collections;

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
    [SerializeField] private AudioClip rainSound;

    [Header("Volume & Mixing Controls")]
    [Range(0f, 1f)][SerializeField] private float musicVolumeNormal = 0.5f;
    [Range(0f, 1f)][SerializeField] private float musicVolumeDucked = 0.1f;
    [SerializeField] private float duckingSpeed = 3f;
    [Range(0f, 5f)][SerializeField] private float fallingWindVolume = 2.5f;
    [Tooltip("Sets the volume for the looping rain sound.")]
    [Range(0f, 1f)][SerializeField] private float rainVolume = 0.3f; // NEW

    private AudioSource sfxSource, musicSource, loopingSfxSource, rainSource;
    private Coroutine volumeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        loopingSfxSource = gameObject.AddComponent<AudioSource>();
        rainSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;

        loopingSfxSource.clip = fallingWindSound;
        loopingSfxSource.loop = true;

        rainSource.clip = rainSound;
        rainSource.loop = true;

        musicSource.volume = musicVolumeNormal;
        // --- NEW: Set the rain volume ---
        rainSource.volume = rainVolume;
    }

    void Start()
    {
        if (musicSource.clip != null) musicSource.Play();
        if (rainSource.clip != null) rainSource.Play();
    }

    public void PlayJumpSound() { if (jumpSound != null) sfxSource.PlayOneShot(jumpSound); }
    public void PlayLandingSound() { if (landingSound != null) sfxSource.PlayOneShot(landingSound); }
    public void PlayHighScoreSound() { if (highScoreSound != null) sfxSource.PlayOneShot(highScoreSound); }
    public void PlayExplosionSound() { if (explosionSound != null) sfxSource.PlayOneShot(explosionSound); }

    public void StartFallingWind()
    {
        if (fallingWindSound != null && !loopingSfxSource.isPlaying)
        {
            loopingSfxSource.volume = fallingWindVolume;
            loopingSfxSource.Play();
            FadeMusic(musicVolumeDucked);
        }
    }

    public void StopFallingWind()
    {
        if (loopingSfxSource.isPlaying)
        {
            loopingSfxSource.Stop();
            loopingSfxSource.volume = 1f;
            FadeMusic(musicVolumeNormal);
        }
    }

    private void FadeMusic(float targetVolume)
    {
        if (volumeCoroutine != null) StopCoroutine(volumeCoroutine);
        volumeCoroutine = StartCoroutine(FadeVolumeCoroutine(targetVolume));
    }

    private IEnumerator FadeVolumeCoroutine(float targetVolume)
    {
        float startVolume = musicSource.volume;
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime * duckingSpeed;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, time);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }
}

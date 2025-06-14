using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips - SFX")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip highScoreSound;
    [SerializeField] private AudioClip explosionSound;

    [Header("Audio Clips - Music")]
    [SerializeField] private AudioClip backgroundMusic;

    private AudioSource sfxSource;      // For playing one-shot sound effects
    private AudioSource musicSource;    // For playing looping background music

    void Awake()
    {
        // Set up the singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }

        // Set up the audio sources
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        // Configure the music source
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
    }

    void Start()
    {
        // Play the background music if it's assigned
        if (musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    // --- Public methods for playing sound effects ---
    public void PlayJumpSound() { if (jumpSound != null) sfxSource.PlayOneShot(jumpSound); }
    public void PlayLandingSound() { if (landingSound != null) sfxSource.PlayOneShot(landingSound); }
    public void PlayHighScoreSound() { if (highScoreSound != null) sfxSource.PlayOneShot(highScoreSound); }
    public void PlayExplosionSound() { if (explosionSound != null) sfxSource.PlayOneShot(explosionSound); }
}

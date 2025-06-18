using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundManager : MonoBehaviour
{
    [Header("Audio Mixer Group")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private AudioClip currentJumpSound;
    private AudioClip currentLandingSound;
    private AudioClip currentExplosionSound; // NEW

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    public void Initialize(ShopItem equippedItem, ShopItem defaultSounds)
    {
        currentJumpSound = equippedItem.jumpSound != null ? equippedItem.jumpSound : defaultSounds.jumpSound;
        currentLandingSound = equippedItem.landingSound != null ? equippedItem.landingSound : defaultSounds.landingSound;
        currentExplosionSound = equippedItem.explosionSound != null ? equippedItem.explosionSound : defaultSounds.explosionSound; // NEW
    }

    public void PlayJumpSound()
    {
        if (currentJumpSound != null) audioSource.PlayOneShot(currentJumpSound);
    }

    public void PlayLandingSound()
    {
        if (currentLandingSound != null) audioSource.PlayOneShot(currentLandingSound);
    }

    // --- NEW METHOD ---
    public void PlayExplosionSound()
    {
        if (currentExplosionSound != null) audioSource.PlayOneShot(currentExplosionSound);
    }
}

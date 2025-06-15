using UnityEngine;

public class Coin : MonoBehaviour
{
    [Tooltip("The sound effect to play when the coin is collected.")]
    [SerializeField] private AudioClip collectSound;
    [Tooltip("The particle effect to spawn when the coin is collected.")]
    [SerializeField] private GameObject collectEffectPrefab;

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the "Player" tag.
        if (other.CompareTag("Player"))
        {
            // Tell the UIManager to add a coin.
            UIManager.Instance.AddCoin();

            // Play the collection sound at the coin's position.
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Spawn the collection particle effect.
            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }

            // Deactivate the coin so it can be reused by the spawner.
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffectPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Just tell the InventoryManager to add a coin.
            // The manager will handle saving and telling the UI to update.
            InventoryManager.Instance.AddCoins(1);

            if (collectSound != null) AudioSource.PlayClipAtPoint(collectSound, transform.position);
            if (collectEffectPrefab != null) Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);

            gameObject.SetActive(false);
        }
    }
}

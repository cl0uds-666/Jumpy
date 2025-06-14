using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    // This function is called by Unity when another collider enters this trigger
    private void OnTriggerEnter(Collider other)
    {
        // We check if the object that entered has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // --- Defensive Check ---
            // First, we make sure the UIManager.Instance actually exists before we try to use it.
            if (UIManager.Instance != null)
            {
                // If it exists, tell it to add a point.
                UIManager.Instance.AddPoint();
            }
            else
            {
                // If it doesn't exist, we print a clear error message to the console.
                Debug.LogError("ScoreTrigger could not find the UIManager.Instance! Make sure a UIManager object is in your scene and is active.");
            }

            // To prevent scoring multiple times on the same platform,
            // we disable this trigger zone immediately after it's used.
            gameObject.SetActive(false);
        }
    }
}

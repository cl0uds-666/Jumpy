using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    // This function is called by Unity when another collider enters this trigger
    private void OnTriggerEnter(Collider other)
    {
        // We check if the object that entered has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // If it's the player, we tell the ScoreManager to add a point
            ScoreManager.Instance.AddPoint();

            // To prevent scoring multiple times on the same platform,
            // we disable this trigger zone immediately after it's used.
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using System.Collections; // Required for Coroutines

public class TimeManager : MonoBehaviour
{
    // Singleton pattern to make it easily accessible
    public static TimeManager Instance { get; private set; }

    [SerializeField] private float slowdownFactor = 0.2f; // How much to slow down time (e.g., 0.2f is 20% speed)
    [SerializeField] private float slowdownDuration = 1f;  // How long the slow-motion effect should last in real time

    void Awake()
    {
        // Set up the singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // This is the public function other scripts will call
    public void DoSlowmotion()
    {
        // Stop any previous slowmotion coroutine before starting a new one
        StopAllCoroutines();
        StartCoroutine(SlowdownCoroutine());
    }

    private IEnumerator SlowdownCoroutine()
    {
        // --- Slow down time ---
        Time.timeScale = slowdownFactor;

        // --- Wait for the duration ---
        // We need to wait for slowdownDuration in real time, not game time
        // so we use WaitForSecondsRealtime.
        yield return new WaitForSecondsRealtime(slowdownDuration);

        // --- Speed time back up to normal ---
        // We smoothly interpolate back to 1 over a short period.
        float transitionDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            Time.timeScale = Mathf.Lerp(slowdownFactor, 1f, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time for the transition
            yield return null;
        }

        Time.timeScale = 1f; // Ensure it's exactly 1 at the end
    }
}

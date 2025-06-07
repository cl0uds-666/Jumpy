using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private float slowdownFactor = 0.2f;
    [SerializeField] private float slowdownDuration = 1f;

    // We need to store the original physics timestep value
    private float originalFixedDeltaTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // --- THE FIX: PART 1 ---
        // At the start, we save the default physics timestep.
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void DoSlowmotion()
    {
        StopAllCoroutines();
        StartCoroutine(SlowdownCoroutine());
    }

    private IEnumerator SlowdownCoroutine()
    {
        // --- THE FIX: PART 2 ---
        // Slow down both the game time AND the physics timestep.
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

        // Wait for the duration in real time
        yield return new WaitForSecondsRealtime(slowdownDuration);

        // --- THE FIX: PART 3 ---
        // Smoothly transition back to normal speed.
        float transitionDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            // We interpolate both values back to their original state.
            Time.timeScale = Mathf.Lerp(slowdownFactor, 1f, elapsedTime / transitionDuration);
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // --- THE FIX: PART 4 ---
        // Ensure both values are exactly back to their defaults at the end.
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }
}

using UnityEngine;
using System.Collections.Generic;

// This script spawns rings that get faster and more unpredictable as the score increases.
public class SimpleRandomSpawner : MonoBehaviour
{
    [Header("Required Objects")]
    [SerializeField] private Transform playerTransform;

    [Header("Ring Prefabs")]
    [SerializeField] private GameObject[] ringPrefabs;

    [Header("Spawning Logic")]
    [SerializeField] private float verticalSpacing = 5f;
    [SerializeField] private int initialRings = 5;

    [Header("Rotation Speed Settings (Start)")]
    [Tooltip("The minimum rotation speed at the start of the game.")]
    [SerializeField] private float minSpeed_Start = 30f;
    [Tooltip("The maximum rotation speed at the start of the game.")]
    [SerializeField] private float maxSpeed_Start = 70f;

    [Header("Difficulty Scaling")]
    [Tooltip("The score at which the game reaches maximum difficulty.")]
    [SerializeField] private int scoreToReachMaxDifficulty = 100;
    [Tooltip("The maximum possible rotation speed when at max difficulty.")]
    [SerializeField] private float maxSpeed_End = 120f;
    [Tooltip("At max difficulty, the chance (0.0 to 1.0) that a platform will ignore the alternating pattern and spin in a random direction.")]
    [Range(0, 1)]
    [SerializeField] private float randomDirectionChance_End = 0.25f; // 25% chance at max difficulty

    [Header("Optimization")]
    [SerializeField] private int poolSize = 15;
    [SerializeField] private float cleanupDistance = 25f;

    private Queue<GameObject> activeRings = new Queue<GameObject>();
    private List<GameObject> ringPool = new List<GameObject>();
    private float nextSpawnY;
    private bool nextRingShouldSpinRight = true;

    void Start()
    {
        InitializePool();
        for (int i = 0; i < initialRings; i++)
        {
            SpawnRing();
        }
    }

    void Update()
    {
        if (playerTransform.position.y > nextSpawnY - (verticalSpacing * (initialRings - 1)))
        {
            SpawnRing();
        }
        CleanupRings();
    }

    void SpawnRing()
    {
        GameObject ring = GetPooledRing();
        if (ring == null) return;

        ring.transform.position = new Vector3(0, nextSpawnY, 0);
        ring.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        RingRotator rotator = ring.GetComponent<RingRotator>();
        if (rotator != null)
        {
            // --- NEW: Difficulty Calculation ---
            // 1. Get the current score from our UIManager.
            int currentScore = UIManager.Instance.GetCurrentScore();
            // 2. Calculate a difficulty value from 0.0 to 1.0.
            float difficultyPercent = Mathf.Clamp01((float)currentScore / scoreToReachMaxDifficulty);

            // 3. Use the difficulty value to interpolate the speed.
            float currentMinSpeed = Mathf.Lerp(minSpeed_Start, maxSpeed_End * 0.8f, difficultyPercent);
            float currentMaxSpeed = Mathf.Lerp(maxSpeed_Start, maxSpeed_End, difficultyPercent);
            float speed = Random.Range(currentMinSpeed, currentMaxSpeed);

            // 4. Determine the direction.
            bool spinRight = nextRingShouldSpinRight;
            float directionChangeChance = Mathf.Lerp(0, randomDirectionChance_End, difficultyPercent);
            if (Random.value < directionChangeChance)
            {
                // Ignore the pattern and pick a random direction.
                spinRight = (Random.value < 0.5f);
            }

            if (!spinRight)
            {
                speed *= -1;
            }

            rotator.rotationSpeed = speed;

            // 5. Flip the boolean for the *next* spawn's pattern.
            nextRingShouldSpinRight = !nextRingShouldSpinRight;
        }

        ActivateRing(ring);
        nextSpawnY += verticalSpacing;
    }

    #region Standard Pooling and Cleanup
    private void InitializePool()
    {
        if (ringPrefabs.Length == 0) { Debug.LogError("No Ring Prefabs assigned!"); return; }
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = ringPrefabs[Random.Range(0, ringPrefabs.Length)];
            GameObject ring = Instantiate(prefab, transform);
            ring.SetActive(false);
            ringPool.Add(ring);
        }
    }

    void ActivateRing(GameObject ring)
    {
        ring.SetActive(true);
        foreach (var trigger in ring.GetComponentsInChildren<ScoreTrigger>(true)) { trigger.gameObject.SetActive(true); }
        activeRings.Enqueue(ring);
    }

    void CleanupRings()
    {
        while (activeRings.Count > 0 && activeRings.Peek().transform.position.y < playerTransform.position.y - cleanupDistance)
        {
            GameObject ringToCleanup = activeRings.Dequeue();
            foreach (PlatformSegmentController segment in ringToCleanup.GetComponentsInChildren<PlatformSegmentController>()) { segment.ResetSegment(); }
            ringToCleanup.SetActive(false);
        }
    }

    GameObject GetPooledRing()
    {
        for (int i = 0; i < ringPool.Count; i++) { int randomIndex = Random.Range(i, ringPool.Count); GameObject temp = ringPool[i]; ringPool[i] = ringPool[randomIndex]; ringPool[randomIndex] = temp; }
        for (int i = 0; i < ringPool.Count; i++) { if (!ringPool[i].activeInHierarchy) { return ringPool[i]; } }
        Debug.LogWarning("Pool is empty! Consider increasing pool size.");
        return null;
    }
    #endregion
}

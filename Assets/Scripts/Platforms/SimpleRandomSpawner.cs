using UnityEngine;
using System.Collections.Generic;

// This script now correctly handles the start platform and randomizes the alternating pattern.
public class SimpleRandomSpawner : MonoBehaviour
{
    [Header("Required Objects")]
    [SerializeField] private Transform playerTransform;

    [Header("Ring Prefabs")]
    [Tooltip("The prefab for the very first, non-moving ring.")]
    [SerializeField] private GameObject startRingPrefab;
    [Tooltip("The list of normal, gapped rings to spawn.")]
    [SerializeField] private GameObject[] ringPrefabs;

    [Header("Spawning Logic")]
    [SerializeField] private float verticalSpacing = 5f;
    [SerializeField] private int initialRings = 5;

    [Header("Rotation Speed Settings (Start)")]
    [SerializeField] private float minSpeed_Start = 30f;
    [SerializeField] private float maxSpeed_Start = 70f;

    [Header("Difficulty Scaling")]
    [SerializeField] private int scoreToReachMaxDifficulty = 100;
    [SerializeField] private float maxSpeed_End = 120f;

    [Header("Optimization")]
    [SerializeField] private int poolSize = 15;
    [SerializeField] private float cleanupDistance = 25f;

    private Queue<GameObject> activeRings = new Queue<GameObject>();
    private List<GameObject> ringPool = new List<GameObject>();
    private float nextSpawnY;
    private bool nextRingShouldSpinRight = true;

    void Start()
    {
        // --- NEW: Randomize the starting direction of the pattern ---
        nextRingShouldSpinRight = (Random.value > 0.5f);

        InitializePool();
        SpawnFirstRing();
        for (int i = 1; i < initialRings; i++)
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

    void SpawnFirstRing()
    {
        if (startRingPrefab == null)
        {
            Debug.LogError("Start Ring Prefab is not assigned in the Spawner!");
            return;
        }
        GameObject ring = Instantiate(startRingPrefab, transform.position, Quaternion.identity);

        // --- NEW: Ensure the start ring is always stationary ---
        RingRotator rotator = ring.GetComponent<RingRotator>();
        if (rotator != null)
        {
            rotator.rotationSpeed = 0f; // Force the start ring to not move.
        }

        ActivateRing(ring);
        nextSpawnY += verticalSpacing;
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
            int currentScore = UIManager.Instance.GetCurrentScore();
            float difficultyPercent = Mathf.Clamp01((float)currentScore / scoreToReachMaxDifficulty);

            float currentMinSpeed = Mathf.Lerp(minSpeed_Start, maxSpeed_End * 0.8f, difficultyPercent);
            float currentMaxSpeed = Mathf.Lerp(maxSpeed_Start, maxSpeed_End, difficultyPercent);
            float speed = Random.Range(currentMinSpeed, currentMaxSpeed);

            if (!nextRingShouldSpinRight)
            {
                speed *= -1;
            }

            rotator.rotationSpeed = speed;
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
            if (ringToCleanup.GetComponent<RingRotator>() != null)
            {
                foreach (PlatformSegmentController segment in ringToCleanup.GetComponentsInChildren<PlatformSegmentController>()) { segment.ResetSegment(); }
                ringToCleanup.SetActive(false);
            }
            else
            {
                Destroy(ringToCleanup);
            }
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

using UnityEngine;
using System.Collections.Generic;

public class SimpleRandomSpawner : MonoBehaviour
{
    [Header("Required Objects")]
    [SerializeField] private Transform playerTransform;

    [Header("Ring Prefabs")]
    [SerializeField] private GameObject startRingPrefab;
    [SerializeField] private GameObject[] ringPrefabs;

    [Header("Collectible Prefabs")]
    [SerializeField] private GameObject coinPrefab;
    [Range(0, 1)][SerializeField] private float coinSpawnChance = 0.5f;

    [Header("Spawning Logic")]
    [SerializeField] private float verticalSpacing = 5f;
    [Tooltip("The radius of your platforms from the center. This is used to position the coins correctly.")]
    [SerializeField] private float platformRadius = 5f; // NEW
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
    private List<GameObject> coinPool = new List<GameObject>();
    private float nextSpawnY;
    private bool nextRingShouldSpinRight = true;

    void Start()
    {
        nextRingShouldSpinRight = (Random.value > 0.5f);
        InitializePools();
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

    void SpawnRing()
    {
        GameObject ring = GetPooledRing();
        if (ring == null) return;

        // --- NEW: We need the ring's rotation to calculate the coin position ---
        Quaternion ringRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        ring.transform.position = new Vector3(0, nextSpawnY, 0);
        ring.transform.rotation = ringRotation;

        RingRotator rotator = ring.GetComponent<RingRotator>();
        if (rotator != null)
        {
            int currentScore = UIManager.Instance.GetCurrentScore();
            float difficultyPercent = Mathf.Clamp01((float)currentScore / scoreToReachMaxDifficulty);
            float currentMinSpeed = Mathf.Lerp(minSpeed_Start, maxSpeed_End * 0.8f, difficultyPercent);
            float currentMaxSpeed = Mathf.Lerp(maxSpeed_Start, maxSpeed_End, difficultyPercent);
            float speed = Random.Range(currentMinSpeed, currentMaxSpeed);
            if (!nextRingShouldSpinRight) { speed *= -1; }
            rotator.rotationSpeed = speed;
            nextRingShouldSpinRight = !nextRingShouldSpinRight;
        }

        ActivateRing(ring);

        if (Random.value < coinSpawnChance)
        {
            // Pass the new ring's rotation to the coin spawner
            SpawnCoin(ringRotation);
        }

        nextSpawnY += verticalSpacing;
    }

    // --- UPDATED METHOD ---
    void SpawnCoin(Quaternion platformRotation)
    {
        GameObject coin = GetPooledCoin();
        if (coin == null) return;

        // 1. Position the coin vertically, halfway between platforms.
        float coinY = nextSpawnY - (verticalSpacing / 2f);

        // 2. Calculate the position of the gap. We assume the gap is at the "front" (Vector3.forward) of the prefab.
        // We rotate this direction by the platform's rotation and scale it by the radius.
        Vector3 gapPosition = platformRotation * (Vector3.forward * platformRadius);

        // 3. Set the coin's final position.
        coin.transform.position = new Vector3(gapPosition.x, coinY, gapPosition.z);
        coin.SetActive(true);
    }

    void SpawnFirstRing()
    {
        if (startRingPrefab == null) { Debug.LogError("Start Ring Prefab not assigned!"); return; }
        GameObject ring = Instantiate(startRingPrefab, transform.position, Quaternion.identity);
        RingRotator rotator = ring.GetComponent<RingRotator>();
        if (rotator != null) { rotator.rotationSpeed = 0f; }
        ActivateRing(ring);
        nextSpawnY += verticalSpacing;
    }

    #region Pooling and Cleanup
    private void InitializePools()
    {
        if (ringPrefabs.Length == 0) { Debug.LogError("No Ring Prefabs assigned!"); return; }
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = ringPrefabs[Random.Range(0, ringPrefabs.Length)];
            GameObject ring = Instantiate(prefab, transform);
            ring.SetActive(false);
            ringPool.Add(ring);
        }
        if (coinPrefab == null) { Debug.LogError("No Coin Prefab assigned!"); return; }
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform);
            coin.SetActive(false);
            coinPool.Add(coin);
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
        return null;
    }

    GameObject GetPooledCoin()
    {
        for (int i = 0; i < coinPool.Count; i++)
        {
            if (!coinPool[i].activeInHierarchy) { return coinPool[i]; }
        }
        return null;
    }
    #endregion
}

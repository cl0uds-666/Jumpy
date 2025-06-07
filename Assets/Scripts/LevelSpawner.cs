using UnityEngine;
using System.Collections.Generic;

public class LevelSpawner : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerTransform;

    [Header("Ring Prefabs")]
    [SerializeField] private GameObject[] ringPrefabs;

    [Header("Spawning Logic")]
    [SerializeField] private float verticalSpacing = 5f;
    [SerializeField] private int initialRings = 5;

    [Header("Optimization")]
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float cleanupDistance = 20f;

    private Queue<GameObject> activeRings = new Queue<GameObject>();
    private List<GameObject> ringPool = new List<GameObject>();
    private float nextSpawnY;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject randomRingPrefab = ringPrefabs[Random.Range(0, ringPrefabs.Length)];
            GameObject ring = Instantiate(randomRingPrefab, transform);
            ring.SetActive(false);
            ringPool.Add(ring);
        }

        for (int i = 0; i < initialRings; i++)
        {
            SpawnRing();
        }
    }

    void Update()
    {
        if (playerTransform.position.y > nextSpawnY - (initialRings * verticalSpacing))
        {
            SpawnRing();
            CleanupRing();
        }
    }

    void SpawnRing()
    {
        GameObject ringToSpawn = GetPooledRing();

        if (ringToSpawn != null)
        {
            Vector3 spawnPosition = new Vector3(0, nextSpawnY, 0);
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            ringToSpawn.transform.position = spawnPosition;
            ringToSpawn.transform.rotation = randomRotation;

            ringToSpawn.SetActive(true);

            // --- THIS IS THE FIX ---
            // Instead of finding by name, we find the ScoreTrigger component itself.
            // The 'true' argument makes it find the component even if the GameObject is inactive.
            ScoreTrigger scoreTrigger = ringToSpawn.GetComponentInChildren<ScoreTrigger>(true);
            if (scoreTrigger != null)
            {
                // We then enable the GameObject that the script is attached to.
                scoreTrigger.gameObject.SetActive(true);
            }

            activeRings.Enqueue(ringToSpawn);
            nextSpawnY += verticalSpacing;
        }
    }

    void CleanupRing()
    {
        if (activeRings.Count == 0) return;

        if (activeRings.Peek().transform.position.y < playerTransform.position.y - cleanupDistance)
        {
            GameObject ringToCleanup = activeRings.Dequeue();

            foreach (PlatformSegmentController segment in ringToCleanup.GetComponentsInChildren<PlatformSegmentController>())
            {
                segment.ResetSegment();
            }

            ringToCleanup.SetActive(false);
        }
    }

    GameObject GetPooledRing()
    {
        for (int i = 0; i < ringPool.Count; i++)
        {
            if (!ringPool[i].activeInHierarchy)
            {
                return ringPool[i];
            }
        }
        return null;
    }
}

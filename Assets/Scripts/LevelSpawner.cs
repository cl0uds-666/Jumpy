using UnityEngine;
using System.Collections.Generic; // Required for using Lists and Queues

public class LevelSpawner : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerTransform; // Assign your Player

    [Header("Ring Prefabs")]
    [SerializeField] private GameObject[] ringPrefabs;

    [Header("Spawning Logic")]
    [SerializeField] private float verticalSpacing = 5f; // Vertical distance between rings
    [SerializeField] private int initialRings = 5;      // How many rings to have at the start

    [Header("Optimization")]
    [SerializeField] private int poolSize = 10;          // How many rings to keep in the object pool. Should be more than what's on screen.
    [SerializeField] private float cleanupDistance = 20f;  // How far below the player a ring should be before we hide it.

    // This queue will keep track of the rings that are currently active in the scene
    private Queue<GameObject> activeRings = new Queue<GameObject>();
    // This list will be our object pool, holding all the rings we can use
    private List<GameObject> ringPool = new List<GameObject>();

    private float nextSpawnY; // The Y position for the next ring to be spawned

    void Start()
    {
        // --- 1. Create the Object Pool ---
        // We instantiate all the rings we'll need and add them to the pool, disabled.
        for (int i = 0; i < poolSize; i++)
        {
            GameObject randomRingPrefab = ringPrefabs[Random.Range(0, ringPrefabs.Length)];
            GameObject ring = Instantiate(randomRingPrefab, transform); // Instantiate as a child of the spawner
            ring.SetActive(false); // Start with the ring turned off
            ringPool.Add(ring);
        }

        // --- 2. Spawn the Initial Rings ---
        // We place the first few rings to give the player a starting path
        for (int i = 0; i < initialRings; i++)
        {
            SpawnRing();
        }
    }

    void Update()
    {
        // --- 3. Infinite Spawning Logic ---
        // If the player gets close enough to the top, spawn the next ring
        if (playerTransform.position.y > nextSpawnY - (initialRings * verticalSpacing))
        {
            SpawnRing();
            CleanupRing();
        }
    }

    void SpawnRing()
    {
        // Get a disabled ring from our pool
        GameObject ringToSpawn = GetPooledRing();

        if (ringToSpawn != null)
        {
            // Set its position and rotation
            Vector3 spawnPosition = new Vector3(0, nextSpawnY, 0);
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            ringToSpawn.transform.position = spawnPosition;
            ringToSpawn.transform.rotation = randomRotation;

            // Activate the ring and its score trigger
            ringToSpawn.SetActive(true);
            // We need to re-enable the score trigger child object every time
            Transform scoreTrigger = ringToSpawn.transform.Find("ScoreTriggerZone");
            if (scoreTrigger != null)
            {
                scoreTrigger.gameObject.SetActive(true);
            }

            // Add it to our list of active rings
            activeRings.Enqueue(ringToSpawn);

            // Update the Y position for the *next* ring
            nextSpawnY += verticalSpacing;
        }
    }

    void CleanupRing()
    {
        // Look at the oldest ring we've spawned (the one at the bottom)
        if (activeRings.Peek().transform.position.y < playerTransform.position.y - cleanupDistance)
        {
            // Dequeue the ring from the active list
            GameObject ringToCleanup = activeRings.Dequeue();
            // Deactivate it, returning it to the pool for reuse
            ringToCleanup.SetActive(false);
        }
    }

    GameObject GetPooledRing()
    {
        // Search through our pool for a ring that is currently turned off
        for (int i = 0; i < ringPool.Count; i++)
        {
            if (!ringPool[i].activeInHierarchy)
            {
                return ringPool[i];
            }
        }
        // If we can't find one, we return null (this shouldn't happen if the pool is big enough)
        return null;
    }
}

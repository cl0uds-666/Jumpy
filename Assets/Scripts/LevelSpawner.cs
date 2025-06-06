using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [Header("Ring Prefabs")]
    // An array to hold all your different ring prefabs
    [SerializeField] private GameObject[] ringPrefabs;

    [Header("Spawning Settings")]
    [SerializeField] private int numberOfRings = 10; // How many rings to spawn in total
    [SerializeField] private float verticalSpacing = 5f; // The vertical distance between each ring's center
    [SerializeField] private float initialYPosition = 0f; // The starting height for the first ring

    void Start()
    {
        // Check if there are any prefabs assigned to prevent errors
        if (ringPrefabs.Length == 0)
        {
            Debug.LogError("No ring prefabs assigned to the Level Spawner!");
            return;
        }

        SpawnLevels();
    }

    void SpawnLevels()
    {
        // Loop to create the specified number of rings
        for (int i = 0; i < numberOfRings; i++)
        {
            // --- Step 1: Choose a random ring prefab from the array ---
            int randomPrefabIndex = Random.Range(0, ringPrefabs.Length);
            GameObject prefabToSpawn = ringPrefabs[randomPrefabIndex];

            // --- Step 2: Determine the position for the new ring ---
            // Each ring is placed higher than the last one
            Vector3 spawnPosition = new Vector3(0, initialYPosition + (i * verticalSpacing), 0);

            // --- Step 3: Create a random rotation ---
            // We only rotate around the Y axis to change where the gap faces
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            // --- Step 4: Instantiate the chosen prefab at the position with the random rotation ---
            Instantiate(prefabToSpawn, spawnPosition, randomRotation);
        }
    }
}

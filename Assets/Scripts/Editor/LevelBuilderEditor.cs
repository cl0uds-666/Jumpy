using UnityEngine;
using UnityEditor; // Required for making editor tools

// This tells Unity that this script is a custom editor for our LevelBuilder component.
[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelBuilder levelBuilder = (LevelBuilder)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Level"))
        {
            GenerateLevel(levelBuilder);
        }
    }

    private void GenerateLevel(LevelBuilder builder)
    {
        // --- Error Checking ---
        if (builder.ringPrefabs == null || builder.ringPrefabs.Length == 0)
        {
            Debug.LogError("Please assign at least one Ring Prefab in the Level Builder.");
            return;
        }

        if (builder.ringParent == null)
        {
            Debug.LogError("Please assign a Parent Transform to keep the hierarchy clean.");
            return;
        }

        // --- Generation Logic ---

        // Clear out any previously generated rings.
        while (builder.ringParent.childCount > 0)
        {
            DestroyImmediate(builder.ringParent.GetChild(0).gameObject);
        }

        // Loop and spawn the rings.
        for (int i = 0; i < builder.numberOfRings; i++)
        {
            // 1. Pick a random prefab from the array.
            GameObject prefabToSpawn = builder.ringPrefabs[Random.Range(0, builder.ringPrefabs.Length)];

            // 2. Calculate the position for the new ring.
            Vector3 position = new Vector3(0, i * builder.verticalSpacing, 0);

            // 3. Determine the rotation.
            Quaternion rotation = Quaternion.identity; // Default to no rotation.
            if (builder.randomizeRotation)
            {
                rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            }

            // 4. Instantiate the chosen prefab with the correct position and rotation.
            GameObject newRing = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn, builder.ringParent);
            newRing.transform.position = position;
            newRing.transform.rotation = rotation;
        }

        Debug.Log("Randomized level generated successfully!");
    }
}

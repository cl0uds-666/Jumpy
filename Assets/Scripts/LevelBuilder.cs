using UnityEngine;

// This script holds the data for our editor tool.
public class LevelBuilder : MonoBehaviour
{
    [Header("Generation Settings")]
    [Tooltip("A list of all the different ring prefabs you want to use.")]
    public GameObject[] ringPrefabs; // Changed to an array

    [Tooltip("How many rings to spawn vertically in total.")]
    public int numberOfRings = 20;

    [Tooltip("The vertical distance between each ring.")]
    public float verticalSpacing = 5f;

    [Tooltip("Should each spawned ring have a random Y rotation?")]
    public bool randomizeRotation = true;

    [Header("Scene Organization")]
    [Tooltip("An empty parent object to hold the generated rings.")]
    public Transform ringParent;
}

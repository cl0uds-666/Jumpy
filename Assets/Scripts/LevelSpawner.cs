using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelSpawner : MonoBehaviour
{
    [Header("Required Objects")]
    [Tooltip("The player's Transform component.")]
    [SerializeField] private Transform playerTransform;

    [Header("Ring Prefabs")]
    [Tooltip("A list of your pre-made ring prefabs.")]
    [SerializeField] private GameObject[] ringPrefabs;

    [Header("Spawning Logic")]
    [SerializeField] private float verticalSpacing = 5f;
    [SerializeField] private int initialRings = 5;

    [Header("Generation Settings")]
    [Tooltip("Layer of solid platform segments.")]
    [SerializeField] private LayerMask platformLayer;
    [Tooltip("Layer of gap trigger boxes.")]
    [SerializeField] private LayerMask gapTriggerLayer;
    [Tooltip("Max placement attempts per ring.")]
    [SerializeField] private int maxPlacementAttempts = 200;

    [Header("Optimization")]
    [SerializeField] private int poolSize = 15;
    [SerializeField] private float cleanupDistance = 25f;

    private Queue<GameObject> activeRings = new Queue<GameObject>();
    private List<GameObject> ringPool = new List<GameObject>();
    private float nextSpawnY;
    private GameObject highestRing;

    void Start()
    {
        InitializePool();
        for (int i = 0; i < initialRings; i++)
        {
            SpawnRing(true);
        }
    }

    void Update()
    {
        if (playerTransform.position.y > nextSpawnY - verticalSpacing * (initialRings - 1))
        {
            SpawnRing(false);
        }
        CleanupRings();
    }

    void SpawnRing(bool isInitial)
    {
        GameObject ring = GetPooledRing();
        if (ring == null)
            return;

        GameObject belowRing = highestRing;

        // Set position and random initial rotation.
        ring.transform.position = new Vector3(0, nextSpawnY, 0);
        ring.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        Debug.Log($"[SpawnRing] Spawning '{ring.name}' at Y={nextSpawnY:F2}");

        if (!isInitial && belowRing != null)
        {
            bool placed = false;
            for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
            {
                float angle = Random.Range(0f, 360f);
                ring.transform.rotation = Quaternion.Euler(0, angle, 0);
                Debug.Log($"[SpawnRing] Attempt {attempt}: angle={angle:F1}");

                // Check each gap collider on the new ring
                var gapCols = ring.GetComponentsInChildren<BoxCollider>(true);
                foreach (var col in gapCols)
                {
                    if (!col.isTrigger)
                        continue;
                    int layerBit = 1 << col.gameObject.layer;
                    if ((gapTriggerLayer & layerBit) == 0)
                        continue;

                    Vector3 center = col.transform.TransformPoint(col.center);
                    Vector3 halfExtents = Vector3.Scale(col.size * 0.5f, col.transform.lossyScale);
                    Quaternion rot = col.transform.rotation;

                    // Ensure this gap is on the bottom side
                    Vector3 localPos = ring.transform.InverseTransformPoint(center);
                    if (localPos.y > 0f)
                        continue;

                    // Overlap check against the ring directly below
                    Collider[] hits = Physics.OverlapBox(center, halfExtents, rot, platformLayer);
                    bool hitBelow = false;
                    foreach (var h in hits)
                    {
                        if (h.transform.IsChildOf(belowRing.transform))
                        {
                            hitBelow = true;
                            break;
                        }
                    }
                    Debug.Log($" gap '{col.name}': hits={hits.Length}, hitBelow={hitBelow}");

                    if (hitBelow)
                    {
                        placed = true;
                        Debug.Log(" valid placement with below ring");
                        break;
                    }
                }

                if (placed)
                {
                    break;
                }
                else
                {
                    Debug.Log(" no valid gaps, trying next rotation");
                }
            }

            if (!placed)
            {
                Debug.LogError($"[SpawnRing] Unable to place '{ring.name}' after {maxPlacementAttempts} attempts. Skipping.");
                return;
            }
        }

        ActivateRing(ring);
        highestRing = ring;
        Debug.Log($"[SpawnRing] Placed at Y={nextSpawnY:F2}, Rot={ring.transform.rotation.eulerAngles.y:F1}");
        nextSpawnY += verticalSpacing;
    }

    void InitializePool()
    {
        if (ringPrefabs == null || ringPrefabs.Length == 0)
        {
            Debug.LogError("No ring prefabs assigned in the inspector.");
            return;
        }
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = ringPrefabs[Random.Range(0, ringPrefabs.Length)];
            GameObject inst = Instantiate(prefab, transform);
            inst.SetActive(false);
            ringPool.Add(inst);
        }
    }

    GameObject GetPooledRing()
    {
        foreach (var r in ringPool)
        {
            if (!r.activeInHierarchy)
                return r;
        }
        Debug.LogWarning("Ring pool exhausted. Consider increasing poolSize.");
        return null;
    }

    void ActivateRing(GameObject r)
    {
        r.SetActive(true);
        foreach (var st in r.GetComponentsInChildren<ScoreTrigger>(true))
        {
            st.gameObject.SetActive(true);
            var col = st.GetComponent<Collider>();
            if (col != null)
                col.enabled = true;
        }
        activeRings.Enqueue(r);
    }

    void CleanupRings()
    {
        while (activeRings.Count > 0 &&
               activeRings.Peek().transform.position.y < playerTransform.position.y - cleanupDistance)
        {
            GameObject old = activeRings.Dequeue();
            foreach (var seg in old.GetComponentsInChildren<PlatformSegmentController>(true))
            {
                seg.ResetSegment();
            }
            old.SetActive(false);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (highestRing == null)
            return;

        Gizmos.color = Color.red;
        var cols = highestRing.GetComponentsInChildren<BoxCollider>(true);
        foreach (var col in cols)
        {
            if (!col.isTrigger)
                continue;
            int layerBit = 1 << col.gameObject.layer;
            if ((gapTriggerLayer & layerBit) == 0)
                continue;

            Vector3 center = col.transform.TransformPoint(col.center);
            Vector3 halfExtents = Vector3.Scale(col.size * 0.5f, col.transform.lossyScale);
            Matrix4x4 m = Matrix4x4.TRS(center, col.transform.rotation, Vector3.one);
            Gizmos.matrix = m;
            Gizmos.DrawWireCube(Vector3.zero, col.size);
        }
    }
#endif
}

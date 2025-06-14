using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    // We only need the momentum setting now.
    public bool InheritAirborneMomentum { get; private set; }

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
    }

    /// <summary>
    /// Called once at the start to set the initial state.
    /// </summary>
    public void Initialize(bool initialMomentum)
    {
        InheritAirborneMomentum = initialMomentum;
    }

    /// <summary>
    /// Called by the UI toggle/button.
    /// </summary>
    public void SetInheritAirborneMomentum(bool value)
    {
        InheritAirborneMomentum = value;
        Debug.Log("Momentum Setting changed to: " + InheritAirborneMomentum);
    }
}

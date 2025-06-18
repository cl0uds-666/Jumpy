using UnityEngine;

// We are using your original namespace to ensure compatibility.
namespace VFXTools
{
    [ExecuteAlways]
    public class VFXController : MonoBehaviour
    {
        // --- Your original parameters from the asset store script ---
        [Header("Adjustable Parameters")]
        [SerializeField] private Color particleColor = Color.white;
        [SerializeField, Range(0f, 4f)] private float intensity = 1f;
        [SerializeField] private Vector3 windDirection = Vector3.zero;

        // --- The tracking parameters we added ---
        [Header("Vertical Tracking & Camera Facing")]
        [SerializeField] private Transform cameraToFollow;
        [SerializeField] private float heightOffset = 15f;

        // --- Private Variables ---
        private ParticleSystem[] particleSystems;
        private float[] defaultRateOverTimeValues;
        private bool isVfxCurrentlyEnabled = true;

        void Awake()
        {
            // Find all particle systems attached to this object and its children once.
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            // We also need to initialize the default rates array.
            defaultRateOverTimeValues = new float[particleSystems.Length];
        }

        // This is your original function for live editor updates.
        void OnValidate()
        {
            // We need to make sure we've found the particle systems before trying to apply settings.
            if (particleSystems == null || particleSystems.Length == 0)
            {
                Awake();
            }
            ApplyVisualSettings();
        }

        void LateUpdate()
        {
            // This part handles the visual following and rotation.
            if (cameraToFollow != null)
            {
                transform.position = new Vector3(0, cameraToFollow.position.y + heightOffset, 0);
                transform.rotation = cameraToFollow.rotation;
            }

            // --- VFX TOGGLE LOGIC ---
            // Only run this logic when the game is actually playing.
            if (Application.isPlaying && SettingsManager.Instance != null)
            {
                // Get the desired state from our central Settings Manager.
                bool vfxShouldBeEnabled = SettingsManager.Instance.VfxEnabled;

                // If the setting has changed from our current state...
                if (vfxShouldBeEnabled != isVfxCurrentlyEnabled)
                {
                    // ...update our current state tracker.
                    isVfxCurrentlyEnabled = vfxShouldBeEnabled;

                    // And then loop through all our particle systems.
                    foreach (var ps in particleSystems)
                    {
                        if (ps == null) continue;

                        var emission = ps.emission;
                        // Simply enable or disable the emission module. This is the correct way.
                        emission.enabled = isVfxCurrentlyEnabled;
                    }
                }
            }
        }

        /// <summary>
        /// This is your original ApplySettings function, now renamed to be clearer.
        /// </summary>
        private void ApplyVisualSettings()
        {
            if (particleSystems == null || particleSystems.Length == 0)
            {
                Awake();
            }

            for (int i = 0; i < particleSystems.Length; i++)
            {
                var ps = particleSystems[i];
                if (ps == null) continue;

                var main = ps.main;
                var emission = ps.emission;
                var velocityOverLifetime = ps.velocityOverLifetime;

                main.startColor = particleColor;

                // This check was causing issues in the editor, moved it to a safer place.
                if (defaultRateOverTimeValues[i] == 0f && emission.rateOverTime.constant > 0)
                {
                    defaultRateOverTimeValues[i] = emission.rateOverTime.constant;
                }

                var rate = emission.rateOverTime;
                rate.constant = defaultRateOverTimeValues[i] * intensity;
                emission.rateOverTime = rate;

                if (velocityOverLifetime.enabled)
                {
                    velocityOverLifetime.x = windDirection.x;
                    velocityOverLifetime.y = windDirection.y;
                    velocityOverLifetime.z = windDirection.z;
                }
            }
        }

        #region Public Setters/Getters
        // These are your original public functions.
        public void SetParticleColor(Color newColor)
        {
            particleColor = newColor;
            ApplyVisualSettings();
        }

        public void SetIntensity(float newIntensity)
        {
            intensity = Mathf.Clamp(newIntensity, 0f, 4f);
            ApplyVisualSettings();
        }

        public void SetWindDirection(Vector3 newWindDirection)
        {
            windDirection = newWindDirection;
            ApplyVisualSettings();
        }

        public Color GetParticleColor() { return particleColor; }
        public float GetIntensity() { return intensity; }
        public Vector3 GetWindDirection() { return windDirection; }
        #endregion
    }
}

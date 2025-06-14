using UnityEngine;

namespace VFXTools
{
    // This attribute makes the script run in the editor, so you can see changes live.
    [ExecuteAlways]
    public class VFXController : MonoBehaviour
    {
        [Header("Adjustable Parameters")]
        [SerializeField] private Color particleColor = Color.white; // The colour of the particles.
        [SerializeField, Range(0f, 4f)] private float intensity = 1f; // The intensity of the particle emission.
        [SerializeField] private Vector3 windDirection = Vector3.zero; // The direction and strength of the wind effect.

        [Header("Vertical Tracking (Y-Axis)")]
        [Tooltip("The Camera or Player's Transform to follow.")]
        [SerializeField] private Transform targetToFollow;
        [Tooltip("The height above the target where the effect should appear.")]
        [SerializeField] private float heightOffset = 15f;

        // --- Private Variables ---
        private ParticleSystem[] particleSystems; // A list of all particle systems that are children of this object.
        private float[] defaultRateOverTimeValues; // The default emission rate for each particle system.

        void Awake()
        {
            // Apply the settings when the game starts.
            ApplySettings();
        }

        // This function is called in the editor whenever you change a value in the Inspector.
        void OnValidate()
        {
            ApplySettings();
        }

        // This function runs when the effect needs to follow the target.
        void LateUpdate()
        {
            // If a target has been assigned, make this object follow its Y position.
            if (targetToFollow != null)
            {
                // The effect's X and Z position should always be at the world center (0, 0).
                float x = 0;
                float z = 0;

                // The effect's Y position is always based on the target's current height, plus an offset.
                float y = targetToFollow.position.y + heightOffset;

                // Apply the new position.
                transform.position = new Vector3(x, y, z);
            }
        }

        /// <summary>
        /// Finds all ParticleSystem components in the children of this GameObject.
        /// </summary>
        void FindParticles()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            defaultRateOverTimeValues = new float[particleSystems.Length];
        }

        /// <summary>
        /// Applies all the public parameter settings to the actual particle systems.
        /// </summary>
        private void ApplySettings()
        {
            // If we haven't found the particle systems yet, find them now.
            if (particleSystems == null || particleSystems.Length == 0)
            {
                FindParticles();
            }

            // Loop through each particle system and apply the settings.
            for (int i = 0; i < particleSystems.Length; i++)
            {
                var ps = particleSystems[i];
                var main = ps.main;
                var emission = ps.emission;
                var velocityOverLifetime = ps.velocityOverLifetime;

                // Apply color
                main.startColor = particleColor;

                // Store the default emission rate if we haven't already.
                if (defaultRateOverTimeValues[i] == 0f)
                {
                    defaultRateOverTimeValues[i] = emission.rateOverTime.constant;
                }

                // Apply intensity by modifying the emission rate.
                var rate = emission.rateOverTime;
                rate.constant = defaultRateOverTimeValues[i] * intensity;
                emission.rateOverTime = rate;

                // Apply wind direction
                if (velocityOverLifetime.enabled)
                {
                    velocityOverLifetime.x = windDirection.x;
                    velocityOverLifetime.y = windDirection.y;
                    velocityOverLifetime.z = windDirection.z;
                }
            }
        }

        // --- Public methods for changing settings from other scripts ---
        public void SetParticleColor(Color newColor)
        {
            particleColor = newColor;
            ApplySettings();
        }

        public void SetIntensity(float newIntensity)
        {
            intensity = Mathf.Clamp(newIntensity, 0f, 4f);
            ApplySettings();
        }

        public void SetWindDirection(Vector3 newWindDirection)
        {
            windDirection = newWindDirection;
            ApplySettings();
        }

        public Color GetParticleColor()
        {
            return particleColor;
        }

        public float GetIntensity()
        {
            return intensity;
        }

        public Vector3 GetWindDirection()
        {
            return windDirection;
        }
    }
}

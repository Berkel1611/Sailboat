using Sailboat.Config;
using Sailboat.Simulation;
using UnityEngine;

namespace Sailboat.Components
{
    /// <summary>
    /// Provides the current true wind. Starts from the base values in <see cref="WindConfig"/>
    /// and adds slow, smooth variation of direction and speed using Perlin noise.
    /// </summary>
    public sealed class WindProvider : MonoBehaviour
    {
        // Different sample rows keep direction and speed variation independent of each other.
        private const float DirectionNoiseRow = 0.37f;
        private const float SpeedNoiseRow = 17.91f;

        [SerializeField] private WindConfig _config;

        /// <summary>
        /// Current wind velocity pointing where the wind blows to, in m/s (x = world X, y = world Z).
        /// </summary>
        public Vector2 Velocity { get; private set; }

        /// <summary>
        /// Current direction the wind comes from, in degrees: 0 = from world +Z, 90 = from world +X.
        /// </summary>
        public float FromDirectionDegrees { get; private set; }

        /// <summary>
        /// Current wind speed in m/s.
        /// </summary>
        public float Speed { get; private set; }

        private void Awake()
        {
            if (_config == null)
            {
                Debug.LogError($"{nameof(WindProvider)} needs a reference to {nameof(WindConfig)}.", this);
                enabled = false;
                return;
            }

            UpdateWind(0f);
        }

        private void Update()
        {
            UpdateWind(Time.time);
        }

        private void UpdateWind(float time)
        {
            float noiseX = time * _config.VariationFrequency;

            FromDirectionDegrees = Mathf.Repeat(
                _config.FromDirectionDegrees + _config.DirectionVariationDegrees * SignedNoise(noiseX, DirectionNoiseRow),
                360f);
            Speed = Mathf.Max(
                0f,
                _config.Speed * (1f + _config.SpeedVariation * SignedNoise(noiseX, SpeedNoiseRow)));

            Velocity = WindMath.VelocityFrom(FromDirectionDegrees, Speed);
        }

        /// <summary>
        /// Smooth noise in the range -1..1.
        /// </summary>
        private static float SignedNoise(float x, float row)
        {
            // PerlinNoise returns roughly 0..1 and can slightly exceed it, so the result is clamped.
            return Mathf.Clamp01(Mathf.PerlinNoise(x, row)) * 2f - 1f;
        }
    }
}

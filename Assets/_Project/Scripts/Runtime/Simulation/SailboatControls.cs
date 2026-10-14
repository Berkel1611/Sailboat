using UnityEngine;

namespace Sailboat.Simulation
{
    /// <summary>
    /// Immutable snapshot of the player's commands passed to the simulation.
    /// Values are clamped on construction so the model never receives out-of-range input.
    /// </summary>
    public readonly struct SailboatControls
    {
        public const float MaxSailAngleDegrees = 45f;

        public SailboatControls(float sailOpening, float sailAngleDegrees, float rudder)
        {
            SailOpening = Mathf.Clamp01(sailOpening);
            SailAngleDegrees = Mathf.Clamp(sailAngleDegrees, -MaxSailAngleDegrees, MaxSailAngleDegrees);
            Rudder = Mathf.Clamp(rudder, -1f, 1f);
        }

        /// <summary>
        /// Sail opening normalized to 0..1.
        /// </summary>
        public float SailOpening { get; }

        /// <summary>
        /// Sail angle relative to the hull in degrees, -45..45. 0 means that sail lies along the hull.
        /// Positive values rotate the sail to the right (same as Heading).
        /// </summary>
        public float SailAngleDegrees { get; }

        /// <summary>
        /// Rudder deflection, -1 (full left) .. 1 (full right).
        /// </summary>
        public float Rudder { get; }
    }
}

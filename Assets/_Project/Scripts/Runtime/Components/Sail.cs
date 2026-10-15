using System;
using Sailboat.Simulation;
using UnityEngine;

namespace Sailboat.Components
{
    /// <summary>
    /// Single source of truth for the sail setting. Converts the player's input into
    /// the sail opening (0..100) and angle (-45..45) and notifies listeners when either changes.
    /// </summary>
    public sealed class Sail : MonoBehaviour
    {
        private const float MaxOpeningPercent = 100f;

        [SerializeField] private SailboatInputHandler _input;

        [Tooltip("How fast the sail opens and closes, in percent per second.")]
        [SerializeField, Min(0f)] private float _openingSpeed = 40f;

        [Tooltip("How fast the sail rotates, in degrees per second.")]
        [SerializeField, Min(0f)] private float _rotationSpeed = 30f;

        private float _openingPercent;
        private float _angleDegrees;

        /// <summary>
        /// Raised when the opening or the angle changes. Listeners read the new values from properties.
        /// </summary>
        public event Action Changed;

        /// <summary>
        /// Sail opening in percent, 0..100.
        /// </summary>
        public float OpeningPercent => _openingPercent;

        /// <summary>
        /// Sail opening normalized to 0..1, as expected by the sailing model.
        /// </summary>
        public float Opening01 => _openingPercent / MaxOpeningPercent;

        /// <summary>
        /// Sail angle relative to the hull in degrees, -45..45. Zero means the sail lies along the hull.
        /// </summary>
        public float AngleDegrees => _angleDegrees;

        private void Awake()
        {
            if (_input == null)
            {
                Debug.LogError($"{nameof(Sail)} needs a reference to {nameof(SailboatInputHandler)}.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            float maxAngle = SailboatControls.MaxSailAngleDegrees;

            float opening = Mathf.Clamp(
                _openingPercent + _input.SailOpeningAxis * _openingSpeed * deltaTime,
                0f,
                MaxOpeningPercent);
            float angle = Mathf.Clamp(
                _angleDegrees + _input.SailRotationAxis * _rotationSpeed * deltaTime,
                -maxAngle,
                maxAngle);

            // Exact comparison is intended: with no input, or at a limit, the clamped values stay bit-identical.
            if (opening == _openingPercent && angle == _angleDegrees)
            {
                return;
            }

            _openingPercent = opening;
            _angleDegrees = angle;
            Changed?.Invoke();
        }
    }
}

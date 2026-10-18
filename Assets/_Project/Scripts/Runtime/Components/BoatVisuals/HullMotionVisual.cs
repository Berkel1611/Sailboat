using Sailboat.Components.Boat;
using UnityEngine;

namespace Sailboat.Components.BoatVisuals
{
    /// <summary>
    /// Heels the boat model with the sideways sail force and adds gentle bobbing on the water. 
    /// Purely visual.
    /// </summary>
    public sealed class HullMotionVisual : MonoBehaviour
    {
        private const float NewtonsPerKilonewton = 1000f;

        // Pitch and roll use periods unrelated to the bobbing period, so the motion does not look mechanical.
        private const float PitchFrequencyRatio = 0.73f;
        private const float RollFrequencyRatio = 1.37f;
        private const float PitchPhaseOffset = 1.3f;
        private const float RollPhaseOffset = 2.1f;

        [SerializeField] private SailboatController _controller;

        [Tooltip("Child transform holding the boat visual model.")]
        [SerializeField] private Transform _visual;

        [Header("Heel")]
        [Tooltip("Heel angle in degrees per kilonewton of sideways sail force.")]
        [SerializeField, Min(0f)] private float _heelPerKilonewton = 4f;

        [SerializeField, Range(0f, 45f)] private float _maxHeelDegrees = 20f;

        [Tooltip("Approximate time in seconds the boat needs to settle at a new heel angle.")]
        [SerializeField, Min(0f)] private float _heelSmoothTime = 0.6f;

        [Header("Bobbing")]
        [Tooltip("Vertical bobbing amplitude in metres.")]
        [SerializeField, Min(0f)] private float _bobAmplitude = 0.15f;

        [Tooltip("Duration of one bobbing cycle in seconds.")]
        [SerializeField, Min(0.1f)] private float _bobPeriod = 3.5f;

        [Tooltip("Pitch and roll rocking amplitude in degrees.")]
        [SerializeField, Min(0f)] private float _rockAmplitudeDegrees = 1.5f;

        private Vector3 _basePosition;
        private float _heelDegrees;
        private float _heelVelocity;

        private void Awake()
        {
            if (_controller == null || _visual == null)
            {
                Debug.LogError(
                    $"{nameof(HullMotionVisual)} needs references to {nameof(SailboatController)} and the visual transform.",
                    this);
                enabled = false;
                return;
            }

            _basePosition = _visual.localPosition;
        }

        private void Update()
        {
            UpdateHeel();
            ApplyMotion(Time.time);
        }

        private void UpdateHeel()
        {
            // Only the sideways part of the sail force heels the boat; the forward part drives it.
            float lateralForce = Vector2.Dot(_controller.Forces.SailForce, _controller.State.Right);
            float targetHeel = Mathf.Clamp(
                lateralForce / NewtonsPerKilonewton * _heelPerKilonewton,
                -_maxHeelDegrees,
                _maxHeelDegrees);

            _heelDegrees = Mathf.SmoothDamp(_heelDegrees, targetHeel, ref _heelVelocity, _heelSmoothTime);
        }

        private void ApplyMotion(float time)
        {
            float phase = time * 2f * Mathf.PI / _bobPeriod;
            float bob = _bobAmplitude * Mathf.Sin(phase);
            float pitch = _rockAmplitudeDegrees * Mathf.Sin(phase * PitchFrequencyRatio + PitchPhaseOffset);
            float roll = _rockAmplitudeDegrees * Mathf.Sin(phase * RollFrequencyRatio + RollPhaseOffset);

            _visual.localPosition = _basePosition + Vector3.up * bob;

            // A positive Z rotation tilts the mast to port, so a heel to starboard needs a negative angle.
            _visual.localRotation = Quaternion.Euler(pitch, 0f, roll - _heelDegrees);
        }
    }
}

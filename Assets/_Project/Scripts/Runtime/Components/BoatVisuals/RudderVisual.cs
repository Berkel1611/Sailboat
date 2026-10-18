using Sailboat.Components.Boat;
using UnityEngine;

namespace Sailboat.Components.BoatVisuals
{
    /// <summary>
    /// Turns the rudder blade according to the rudder input. Purely visual.
    /// </summary>
    public sealed class RudderVisual : MonoBehaviour
    {
        [SerializeField] private SailboatInputHandler _input;

        [SerializeField] private Transform _rudder;

        [Tooltip("Blade angle at full rudder input, in degrees.")]
        [SerializeField, Range(0f, 60f)] private float _maxAngleDegrees = 30f;

        [Tooltip("How fast the blade turns towards the target angle, in degrees per second.")]
        [SerializeField, Min(0f)] private float _turnSpeed = 90f;

        private Quaternion _baseRotation;
        private float _angleDegrees;

        private void Awake()
        {
            if (_input == null || _rudder == null)
            {
                Debug.LogError(
                    $"{nameof(RudderVisual)} needs references to {nameof(SailboatInputHandler)} and the rudder transform.",
                    this);
                enabled = false;
                return;
            }

            _baseRotation = _rudder.localRotation;
        }

        private void Update()
        {
            float targetAngle = -_input.RudderAxis * _maxAngleDegrees;
            _angleDegrees = Mathf.MoveTowards(_angleDegrees, targetAngle, _turnSpeed * Time.deltaTime);

            _rudder.localRotation = _baseRotation * Quaternion.Euler(0f, _angleDegrees, 0f);
        }
    }
}

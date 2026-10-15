using Sailboat.Input;
using UnityEngine;

namespace Sailboat.Components
{
    /// <summary>
    /// Reads the player's input and exposes it as plain axis values in range -1..1.
    /// This is the only class that knows about the Input System.
    /// </summary>
    public sealed class SailboatInputHandler : MonoBehaviour
    {
        private SailboatInputActions _actions;

        /// <summary>
        /// Positive opens the sail, negative closes it.
        /// </summary>
        public float SailOpeningAxis => _actions.Sailboat.SailOpening.ReadValue<float>();

        /// <summary>
        /// Positive rotates the sail clockwise.
        /// </summary>
        public float SailRotationAxis => _actions.Sailboat.SailRotation.ReadValue<float>();

        /// <summary>
        /// Positive turns the bow to starboard (right).
        /// </summary>
        public float RudderAxis => _actions.Sailboat.Rudder.ReadValue<float>();

        private void Awake()
        {
            _actions = new SailboatInputActions();
        }

        private void OnEnable()
        {
            _actions.Sailboat.Enable();
        }

        private void OnDisable()
        {
            _actions.Sailboat.Disable();
        }

        private void OnDestroy()
        {
            _actions.Dispose();
        }
    }
}

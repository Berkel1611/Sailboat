using Sailboat.Config;
using Sailboat.Simulation;
using UnityEngine;

namespace Sailboat.Components
{
    /// <summary>
    /// Connects the scene with the sailing model: collects the sail setting, the rudder input and the wind,
    /// advances the simulation in FixedUpdate and applies the result to this transform.
    /// The motion is computed entirely by <see cref="SailingModel"/>.
    /// </summary>
    public sealed class SailboatController : MonoBehaviour
    {
        private const float GizmoHeight = 2f;
        private const float GizmoWindScale = 0.5f;
        private const float GizmoForceScale = 0.002f;
        private const float GizmoVelocityScale = 1f;

        [SerializeField] private SailboatConfig _config;
        [SerializeField] private Sail _sail;
        [SerializeField] private SailboatInputHandler _input;
        [SerializeField] private WindProvider _wind;

        private SailingModel _model;

        /// <summary>
        /// Current state of the boat after the last simulation step.
        /// </summary>
        public SailboatState State { get; private set; }

        /// <summary>
        /// Forces computed at the beginning of the last simulation step. Used for visuals and debug drawing.
        /// </summary>
        public SailboatForces Forces { get; private set; }

        private void Awake()
        {
            if (_config == null || _sail == null || _input == null || _wind == null)
            {
                Debug.LogError(
                    $"{nameof(SailboatController)} needs references to {nameof(SailboatConfig)}, " +
                    $"{nameof(Sail)}, {nameof(SailboatInputHandler)} and {nameof(WindProvider)}.",
                    this);
                enabled = false;
                return;
            }

            _model = new SailingModel(_config);

            // The simulation starts from wherever the boat was placed in the scene.
            Vector3 position = transform.position;
            float heading = transform.eulerAngles.y * Mathf.Deg2Rad;
            State = new SailboatState(new Vector2(position.x, position.z), heading, Vector2.zero, 0f);
        }

        private void FixedUpdate()
        {
            var controls = new SailboatControls(_sail.Opening01, _sail.AngleDegrees, _input.RudderAxis);
            Vector2 wind = _wind.Velocity;

            // Step() computes the same forces internally; computing them here as well keeps the model API simple.
            Forces = _model.ComputeForces(State, controls, wind);
            State = _model.Step(State, controls, wind, Time.fixedDeltaTime);

            ApplyStateToTransform();
        }

        private void ApplyStateToTransform()
        {
            // The model works on the water plane, so the height of the boat is left untouched.
            var position = new Vector3(State.Position.x, transform.position.y, State.Position.y);
            Quaternion rotation = Quaternion.Euler(0f, State.HeadingDegrees, 0f);
            transform.SetPositionAndRotation(position, rotation);
        }

        private void OnDrawGizmos()
        {
            if (_model == null)
            {
                return;
            }

            Vector3 origin = transform.position + Vector3.up * GizmoHeight;
            DrawArrow(origin, Forces.ApparentWind * GizmoWindScale, Color.cyan);
            DrawArrow(origin, Forces.SailForce * GizmoForceScale, Color.green);
            DrawArrow(origin, State.Velocity * GizmoVelocityScale, Color.yellow);
        }

        private static void DrawArrow(Vector3 origin, Vector2 planeVector, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(origin, new Vector3(planeVector.x, 0f, planeVector.y));
        }
    }
}



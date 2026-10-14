using UnityEngine;

namespace Sailboat.Config
{
    /// <summary>
    /// Physical parameters of the sailboat used by the sailing model.
    /// </summary>
    [CreateAssetMenu(fileName = "SailboatConfig", menuName = "Sailboat/Sailboat Config")]
    public sealed class SailboatConfig : ScriptableObject
    {
        [Header("Body")]
        [Tooltip("Mass of the boat in kilograms.")]
        [SerializeField, Min(1f)] private float _mass = 5000f;

        [Tooltip("Moment of inertia around the vertical axis.")]
        [SerializeField, Min(1f)] private float _inertia = 60000f;

        [Header("Sail")]
        [Tooltip("Sail area in square meters at full opening.")]
        [SerializeField, Min(0f)] private float _sailArea = 60f;

        [Tooltip("Air density.")]
        [SerializeField, Min(0f)] private float _airDensity = 1.225f;

        [Tooltip("Multiplier for the lift coefficient (sail shape / camber).")]
        [SerializeField, Min(0f)] private float _liftScale = 1f;

        [Tooltip("Multiplier for the drag coefficient.")]
        [SerializeField, Min(0f)] private float _dragScale = 1f;

        [Tooltip("Constant parasitic drag coefficient (mast, rigging, friction). Creates the no-go zone.")]
        [SerializeField, Min(0f)] private float _parasiticDrag = 0.25f;

        [Header("Hull and keel")]
        [Tooltip("Linear drag along the hull.")]
        [SerializeField, Min(0f)] private float _forwardLinearDrag = 50f;

        [Tooltip("Quadratic drag along the hull.")]
        [SerializeField, Min(0f)] private float _forwardQuadraticDrag = 90f;

        [Tooltip("Linear drag across the hull (keel).")]
        [SerializeField, Min(0f)] private float _lateralLinearDrag = 2000f;

        [Tooltip("Quadratic drag across the hull (keel).")]
        [SerializeField, Min(0f)] private float _lateralQuadraticDrag = 4400f;

        [Header("Rudder")]
        [Tooltip("Rudder torque per squared forward speed.")]
        [SerializeField, Min(0f)] private float _rudderStrength = 650f;

        [Tooltip("Linear yaw damping.")]
        [SerializeField, Min(0f)] private float _angularLinearDamping = 40000f;

        [Tooltip("Quadratic yaw damping.")]
        [SerializeField, Min(0f)] private float _angularQuadraticDamping = 0f;

        [Header("Simulation")]
        [Tooltip("Fixed simulation step in seconds.")]
        [SerializeField, Range(1f / 240f, 1f / 30f)] private float _timeStep = 1f / 120f;

        public float Mass => _mass;

        public float Inertia => _inertia;

        public float SailArea => _sailArea;

        public float AirDensity => _airDensity;

        public float LiftScale => _liftScale;

        public float DragScale => _dragScale;

        public float ParasiticDrag => _parasiticDrag;

        public float ForwardLinearDrag => _forwardLinearDrag;

        public float ForwardQuadraticDrag => _forwardQuadraticDrag;

        public float LateralLinearDrag => _lateralLinearDrag;

        public float LateralQuadraticDrag => _lateralQuadraticDrag;

        public float RudderStrength => _rudderStrength;

        public float AngularLinearDamping => _angularLinearDamping;

        public float AngularQuadraticDamping => _angularQuadraticDamping;

        public float TimeStep => _timeStep;
    }
}

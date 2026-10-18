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
        [SerializeField, Min(0f)] private float _liftScale = 1.3f;

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
        [SerializeField, Min(0f)] private float _lateralLinearDrag = 10000f;

        [Tooltip("Quadratic drag across the hull (keel).")]
        [SerializeField, Min(0f)] private float _lateralQuadraticDrag = 30000f;

        [Tooltip("Multiplier of the forward drag when the boat moves stern first.")]
        [SerializeField, Min(1f)] private float _reverseDragMultiplier = 10f;

        [Header("Rudder")]
        [Tooltip("Rudder torque per squared forward speed.")]
        [SerializeField, Min(0f)] private float _rudderStrength = 1200f;
        [Tooltip("Rudder torque per forward speed. Keeps the rudder effective at low speed.")]
        [SerializeField, Min(0f)] private float _rudderLinearStrength = 2400f;

        [Tooltip("Linear yaw damping.")]
        [SerializeField, Min(0f)] private float _angularLinearDamping = 40000f;

        [Tooltip("Quadratic yaw damping.")]
        [SerializeField, Min(0f)] private float _angularQuadraticDamping = 0f;

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

        public float ReverseDragMultiplier => _reverseDragMultiplier;

        public float RudderStrength => _rudderStrength;

        public float RudderLinearStrength => _rudderLinearStrength;

        public float AngularLinearDamping => _angularLinearDamping;

        public float AngularQuadraticDamping => _angularQuadraticDamping;
    }
}

using UnityEngine;

namespace Sailboat.Components.Presentation
{
    /// <summary>
    /// Follows the boat from behind.
    /// </summary>
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [Tooltip("Camera position relative to the target in the target's heading frame. Negative Z = behind the stern.")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 12f, -25f);

        [Tooltip("Height above the target pivot that the camera looks at.")]
        [SerializeField] private float _lookAtHeight = 3f;

        [Tooltip("Approximate time in seconds the camera needs to catch up with the boat position.")]
        [SerializeField, Min(0f)] private float _positionSmoothTime = 0.3f;

        [Tooltip("Approximate time in seconds the camera needs to swing behind the boat after a turn.")]
        [SerializeField, Min(0f)] private float _headingSmoothTime = 0.5f;

        private float _headingDegrees;
        private float _headingVelocity;
        private Vector3 _positionVelocity;

        private void Awake()
        {
            if (_target == null)
            {
                Debug.LogError($"{nameof(CameraFollow)} needs a reference to the target transform.", this);
                enabled = false;
                return;
            }

            _headingDegrees = _target.eulerAngles.y;
            transform.position = ComputeDesiredPosition();
            LookAtTarget();
        }

        private void LateUpdate()
        {
            // SmoothDampAngle takes the shorter way around.
            _headingDegrees = Mathf.SmoothDampAngle(
                _headingDegrees, _target.eulerAngles.y, ref _headingVelocity, _headingSmoothTime);

            transform.position = Vector3.SmoothDamp(
                transform.position, ComputeDesiredPosition(), ref _positionVelocity, _positionSmoothTime);

            LookAtTarget();
        }

        private Vector3 ComputeDesiredPosition()
        {
            // Only the heading is used, so a lateral heel or bobbing of the boat does not shake the camera.
            return _target.position + Quaternion.Euler(0f, _headingDegrees, 0f) * _offset;
        }

        private void LookAtTarget()
        {
            transform.LookAt(_target.position + Vector3.up * _lookAtHeight);
        }
    }

}
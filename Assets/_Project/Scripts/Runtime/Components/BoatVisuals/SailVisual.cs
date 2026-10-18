using Sailboat.Components.Boat;
using UnityEngine;

namespace Sailboat.Components.BoatVisuals
{
    /// <summary>
    /// Shows the sail state on the model: rotates the sail around the mast and furls it towards the boom
    /// and lowers the gaff together with the top of the sail.
    /// </summary>
    public sealed class SailVisual : MonoBehaviour
    {
        // A fully furled sail stays as a thin strip on the boom.
        private const float FurledScale = 0.05f;

        [SerializeField] private Sail _sail;

        [Tooltip("Transform on the mast axis, rotated around Y by the sail angle.")]
        [SerializeField] private Transform _rotationPivot;

        [Tooltip("Transform at the boom height, scaled along Y by the sail opening.")]
        [SerializeField] private Transform _furlPivot;

        [Tooltip("Optional upper spar of a gaff sail. Must be sibling of the furl pivot, with its origin at the mast.")]
        [SerializeField] private Transform _gaff;

        private Vector3 _gaffRaisedPosition;
        private Quaternion _gaffRaisedRotation;
        private Vector3 _gaffRaisedDirection;

        private void Awake()
        {
            if (_gaff != null)
            {
                _gaffRaisedPosition = _gaff.localPosition;
                _gaffRaisedRotation = _gaff.localRotation;
                _gaffRaisedDirection = FindGaffDirection();
            }
        }

        private void OnEnable()
        {
            if (_sail == null || _rotationPivot == null || _furlPivot == null)
            {
                Debug.LogError($"{nameof(SailVisual)} needs references to "+ 
                    $"{nameof(Sail)}, the rotation pivot and the furl pivot.", 
                    this);
                enabled = false;
                return;
            }

            if (_gaff != null && (_gaff.parent != _furlPivot.parent || _gaffRaisedDirection == Vector3.zero))
            {
                Debug.LogError(
                    $"{nameof(SailVisual)} needs the gaff to be a sibling of the furl pivot and to have a mesh.",
                    this);
                enabled = false;
                return;
            }

            _sail.Changed += Apply;
            Apply();
        }

        private void OnDisable()
        {
            if (_sail != null)
            {
                _sail.Changed -= Apply;
            }
        }

        private void Apply()
        {
            _rotationPivot.localRotation = Quaternion.Euler(0f, _sail.AngleDegrees, 0f);

            float furlScale = Mathf.Lerp(FurledScale, 1f, _sail.Opening01);

            Vector3 scale = _furlPivot.localScale;
            scale.y = furlScale;
            _furlPivot.localScale = scale;

            if (_gaff != null)
            {
                ApplyGaff(furlScale);
            }
        }

        private void ApplyGaff(float furlScale)
        {
            // The furl pivot scales the sail height around the boom, so the gaff height is scaled the same way
            // and the gaff stays on the top of the sail.
            float boomHeight = _furlPivot.localPosition.y;
            Vector3 position = _gaffRaisedPosition;
            position.y = boomHeight + (_gaffRaisedPosition.y - boomHeight) * furlScale;
            _gaff.localPosition = position;

            Vector3 direction = _gaffRaisedDirection;
            direction.y *= furlScale;
            _gaff.localRotation = Quaternion.FromToRotation(_gaffRaisedDirection, direction) * _gaffRaisedRotation;
        }

        /// <summary>
        /// Finds the direction of the gaff from its mesh: the longest axis od the mesh bounds,
        /// pointing to the side where the mesh lies relative to the origin at the mast.
        /// </summary>
        /// <returns>The direction in the space of the gaff's parent, or zero when the gaff has no mesh.</returns>
        private Vector3 FindGaffDirection()
        {
            var meshFilter = _gaff.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                return Vector3.zero;
            }

            Vector3 center = meshFilter.sharedMesh.bounds.center;
            return (_gaffRaisedRotation * Vector3.Scale(center, _gaff.localScale)).normalized;
        }
    }
}

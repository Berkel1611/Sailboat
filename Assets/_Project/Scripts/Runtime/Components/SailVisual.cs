using System;
using UnityEngine;

namespace Sailboat.Components
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

        [Header("Gaff")]
        [Tooltip("Optional upper spar of a gaff sail.")]
        [SerializeField] private Transform _gaff;

        [Tooltip("Local rotation of the gaff when the sail is fully furled and the gaff lies on the boom.")]
        [SerializeField] private Vector3 _gaffFurledEulerAngles;

        private Vector3 _gaffRaisedPosition;
        private Quaternion _gaffRaisedRotation;
        private Quaternion _gaffFurledRotation;

        private void Awake()
        {
            if (_gaff != null)
            {
                _gaffRaisedPosition = _gaff.localPosition;
                _gaffRaisedRotation = _gaff.localRotation;
                _gaffFurledRotation = Quaternion.Euler(_gaffFurledEulerAngles);
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

            _gaff.localRotation = Quaternion.Slerp(_gaffFurledRotation, _gaffRaisedRotation, _sail.Opening01);
        }
    }
}

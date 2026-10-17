using UnityEngine;

namespace Sailboat.Components
{
    /// <summary>
    /// Shows the sail state on the model: rotates the sail around the mast and furls it towards the boom.
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

            Vector3 scale = _furlPivot.localScale;
            scale.y = Mathf.Lerp(FurledScale, 1f, _sail.Opening01);
            _furlPivot.localScale = scale;
        }
    }
}

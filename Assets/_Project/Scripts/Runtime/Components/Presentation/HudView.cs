using Sailboat.Components.Boat;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sailboat.Components.Presentation
{
    /// <summary>
    /// Shows the sail opening and rotation in two UI Toolkit labels.
    /// Updates the text only when the rounded value changes, so no strings are allocated while nothing changes.
    /// </summary>
    public sealed class HudView : MonoBehaviour
    {
        private const string SailOpeningLabelName = "sail-opening-label";
        private const string SailRotationLabelName = "sail-rotation-label";

        [SerializeField] private PanelRenderer _panelRenderer;
        [SerializeField] private Sail _sail;

        private Label _openingLabel;
        private Label _rotationLabel;
        private int _shownOpening;
        private int _shownRotation;

        private void Awake()
        {
            if (_panelRenderer == null || _sail == null)
            {
                Debug.LogError($"{nameof(HudView)} needs references to {nameof(PanelRenderer)} and {nameof(Sail)}.", this);
                enabled = false;
                return;
            }

            _panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnDestroy()
        {
            if (_panelRenderer != null)
            {
                _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
            }
        }

        private void OnEnable()
        {
            if (_sail != null)
            {
                _sail.Changed += OnSailChanged;
            }
        }

        private void OnDisable()
        {
            if (_sail != null)
            {
                _sail.Changed -= OnSailChanged;
            }
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement root, int version)
        {
            _openingLabel = root.Q<Label>(SailOpeningLabelName);
            _rotationLabel = root.Q<Label>(SailRotationLabelName);

            if (_openingLabel == null || _rotationLabel == null)
            {
                Debug.LogError(
                    $"{nameof(HudView)} could not find labels '{SailOpeningLabelName}' and '{SailRotationLabelName}' in the UXML.",
                    this);
                return;
            }

            Refresh(force: true);
        }

        private void OnSailChanged()
        {
            Refresh(force: false);
        }

        private void Refresh(bool force)
        {
            if (_openingLabel == null || _rotationLabel == null)
            {
                return;
            }

            int opening = Mathf.RoundToInt(_sail.OpeningPercent);
            if (force || opening != _shownOpening)
            {
                _shownOpening = opening;
                _openingLabel.text = $"Sail opening: {opening}%";
            }

            int rotation = Mathf.RoundToInt(_sail.AngleDegrees);
            if (force || rotation != _shownRotation)
            {
                _shownRotation = rotation;
                _rotationLabel.text = $"Sail rotation: {rotation}°";
            }
        }
    }
}

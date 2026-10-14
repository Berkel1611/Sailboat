using UnityEngine;

namespace Sailboat.Config
{
    /// <summary>
    /// Wind parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "WindConfig", menuName = "Sailboat/Wind Config")]
    public sealed class WindConfig : ScriptableObject
    {
        [Header("Base wind")]
        [Tooltip("Direction the wind blows from. Measured in degrees clockwise from world +Z.")]
        [SerializeField, Range(0f, 360f)] private float _fromDirectionDegrees = 225f;

        [Tooltip("Wind speed in meters per second.")]
        [SerializeField, Min(0f)] private float _speed = 10f;

        [Header("Variation")]
        [Tooltip("Maximum deviation of the wind direction from the base value, in degrees (0 = constant wind).")]
        [SerializeField, Range(0f, 90f)] private float _directionVariationDegrees = 20f;

        [Tooltip("Maximum deviation of the wind speed as a fraction of the base speed (0 = constant speed).")]
        [SerializeField, Range(0f, 1f)] private float _speedVariation = 0.2f;

        [Tooltip("How fast the wind changes. Lower values give slower, smoother changes.")]
        [SerializeField, Min(0f)] private float _variationFrequency = 0.03f;

        public float FromDirectionDegrees => _fromDirectionDegrees;

        public float Speed => _speed;

        public float DirectionVariationDegrees => _directionVariationDegrees;

        public float SpeedVariation => _speedVariation;

        public float VariationFrequency => _variationFrequency;
    }
}
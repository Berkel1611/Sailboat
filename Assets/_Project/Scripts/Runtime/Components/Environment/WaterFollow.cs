using UnityEngine;

namespace Sailboat.Components.Environment
{
    /// <summary>
    /// Keeps the water plane under the boat, so the boat never sails off its edge.
    /// The plane moves in steps of one texture tile, which keeps the texture fixed in the world
    /// and lets the boat's motion stay visible.
    /// </summary>
    public sealed class WaterFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [Tooltip("World size of one texture tile in metres. Must match the material tiling: plane size / tiling.")]
        [SerializeField, Min(0.01f)] private float _tileSize = 10f;

        private void Awake()
        {
            if (_target == null)
            {
                Debug.LogError($"{nameof(WaterFollow)} needs a reference to the target transform.", this);
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            Vector3 position = transform.position;
            position.x = SnapToTile(_target.position.x);
            position.z = SnapToTile(_target.position.z);
            transform.position = position;
        }

        private float SnapToTile(float value)
        {
            return Mathf.Round(value / _tileSize) * _tileSize;
        }
    }
}

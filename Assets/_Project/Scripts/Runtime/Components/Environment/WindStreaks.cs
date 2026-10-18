using UnityEngine;

namespace Sailboat.Components.Environment
{
    /// <summary>
    /// Drives a particle system that draws white streaks moving with the true wind.
    /// Particles live in world space, so a camera moving with the boat shows the apparent wind automatically.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class WindStreaks : MonoBehaviour
    {
        [SerializeField] private WindProvider _wind;

        [Tooltip("Multiplier for the particle speed. 1 = particles move exactly with the wind.")]
        [SerializeField, Min(0f)] private float _speedScale = 1f;

        private ParticleSystem.VelocityOverLifetimeModule _velocity;

        private void Awake()
        {
            if (_wind == null)
            {
                Debug.LogError($"{nameof(WindStreaks)} needs a reference to {nameof(WindProvider)}.", this);
                enabled = false;
                return;
            }

            _velocity = GetComponent<ParticleSystem>().velocityOverLifetime;
            _velocity.enabled = true;
            _velocity.space = ParticleSystemSimulationSpace.World;
        }

        private void Update()
        {
            // Velocity over Lifetime also affects particles that already exist,
            // so all streaks turn together when the wind changes direction.
            Vector2 wind = _wind.Velocity * _speedScale;
            _velocity.x = wind.x;
            _velocity.y = 0f;
            _velocity.z = wind.y;
        }
    }
}
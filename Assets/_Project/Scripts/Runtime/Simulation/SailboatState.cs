using UnityEngine;

namespace Sailboat.Simulation
{
    /// <summary>
    /// Immutable kinematic state of the sailboat on the water plane.
    /// The simulation works in 2D: <c>x</c> maps to world X and <c>y</c> maps to world Z.
    /// </summary>
    public readonly struct SailboatState
    {
        public SailboatState(Vector2 position, float heading, Vector2 velocity, float angularVelocity)
        {
            Position = position;
            Heading = heading;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }

        /// <summary>
        /// Position on the water plane in meters.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// Heading (yaw) in radians. Zero means pointing along world +Z, positive turns boat to the right. 
        /// </summary>
        public float Heading { get; }

        /// <summary>
        /// Linear velocity in world space, in meters per second.
        /// </summary>
        public Vector2 Velocity { get; }

        /// <summary>
        /// Yaw rate in radians per second.
        /// </summary>
        public float AngularVelocity { get; }

        /// <summary>
        /// Unit vector pointing from stern to bow.
        /// </summary>
        public Vector2 Forward => new Vector2(Mathf.Sin(Heading), Mathf.Cos(Heading));

        /// <summary>
        /// Unit vector pointing to starboard (right side of the hull).
        /// </summary>
        public Vector2 Right => new Vector2(Mathf.Cos(Heading), -Mathf.Sin(Heading));

        /// <summary>
        /// Velocity component along the hull (positive -> moving forward).
        /// </summary>
        public float ForwardSpeed => Vector2.Dot(Velocity, Forward);

        /// <summary>
        /// Velocity component across the hull (leeway drift, positive -> to starboard).
        /// </summary>
        public float LateralSpeed => Vector2.Dot(Velocity, Right);

        /// <summary>
        /// Heading converted to degrees.
        /// </summary>
        public float HeadingDegrees => Heading * Mathf.Rad2Deg;
    }
}

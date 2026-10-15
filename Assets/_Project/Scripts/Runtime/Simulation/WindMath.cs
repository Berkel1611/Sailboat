using UnityEngine;

namespace Sailboat.Simulation
{
    /// <summary>
    /// Conversion between the way wind is configured and the way the sailing model uses it.
    /// </summary>
    public static class WindMath
    {
        /// <summary>
        /// Converts wind direction into a velocity vector on the water plane.
        /// </summary>
        /// <param name="fromDirectionDegrees">
        /// Where the wind comes from, in degrees: 0 = from world +Z, 90 = from world +X.
        /// </param>
        /// <param name="speed">Wind speed in meters per second.</param>
        /// <returns>Wind velocity pointing where wind blows to (x = world X, y = world Z).</returns>
        public static Vector2 VelocityFrom(float fromDirectionDegrees, float speed)
        {
            float radians = fromDirectionDegrees * Mathf.Deg2Rad;

            // (sin, cos) points towards the source of the wind; the air moves the opposite way.
            Vector2 towardsSource = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
            return -towardsSource * speed;
        }
    }
}

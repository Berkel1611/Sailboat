using UnityEngine;

namespace Sailboat.Simulation
{
    /// <summary>
    /// Force on a flat foil (sail, keel, rudder) in a 2D flow.
    /// Uses the symmetric flat-plate approximation without stall:
    ///     lift coefficient = sin(2a), drag coefficient = 1 - cos(2a).
    /// </summary>
    public static class FoilMath
    {
        private const float MinFlowSpeed = 0.0001f;
        private const float MinLiftDirectionSqrLength = 0.00000001f;

        /// <summary>
        /// Dynamic pressure q = 1/2 * p * v^2, in pascals.
        /// </summary>
        public static float DynamicPressure(float density, float speed)
        {
            return 0.5f * density * speed * speed;
        }

        /// <summary>
        /// Lift coefficient of a flat plate. Maximum at 45 degrees, zero at 0 degrees and 90 degrees.
        /// </summary>
        /// <param name="angleOfAttack">Angle between the flow and the foil surface in radians.</param>
        public static float LiftCoefficient(float angleOfAttack)
        {
            return Mathf.Sin(2f * angleOfAttack);
        }

        /// <summary>
        /// Drag coefficient of a flat plate. Zero at 0 degrees, maximum at 90 degrees.
        /// </summary>
        /// <param name="angleOfAttack">Angle between the flow and the foil surface in radians.</param>
        public static float DragCoefficient(float angleOfAttack)
        {
            return 1f - Mathf.Cos(2f * angleOfAttack);
        }

        /// <summary>
        /// Computes the total force (lift + drag) the flow exerts on the foil.
        /// </summary>
        /// <param name="flowVelocity">Velocity of the fluid relative to the foil, pointing where the fluid moves to (m/s).</param>
        /// <param name="normal">Unit vector perpendicular to the foil surface.</param>
        /// <param name="parameters">Physical description of the foil.</param>
        /// <returns>Force in newtons on the water plane.</returns>
        public static Vector2 ComputeForce(Vector2 flowVelocity, Vector2 normal, FoilParameters parameters)
        {
            float flowSpeed = flowVelocity.magnitude;
            if (flowSpeed < MinFlowSpeed || parameters.Area <= 0f)
            {
                return Vector2.zero;
            }

            Vector2 flowDirection = flowVelocity / flowSpeed;

            // Use the normal on the leeward side, so the force always points "with" the flow.
            Vector2 leewardNormal = Vector2.Dot(normal, flowDirection) >= 0f ? normal : -normal;

            // The angle between the flow and the surface equals 90 degrees minus the angle between the flow and the normal,
            // so its sine is the dot product of the flow direction and the normal.
            float sinAngleOfAttack = Mathf.Clamp01(Vector2.Dot(flowDirection, leewardNormal));
            float angleOfAttack = Mathf.Asin(sinAngleOfAttack);

            // Lift is perpendicular to the flow, on the side in leeward normal.
            // When the flow hits the foil head-on the direction is undefined, but the lift coefficient is zero anyway.
            Vector2 liftDirection = leewardNormal - sinAngleOfAttack * flowDirection;
            liftDirection = liftDirection.sqrMagnitude > MinLiftDirectionSqrLength
                ? liftDirection.normalized
                : Vector2.zero;

            float liftCoefficient = parameters.LiftScale * LiftCoefficient(angleOfAttack);
            float dragCoefficient = parameters.DragScale * (DragCoefficient(angleOfAttack) + parameters.ParasiticDrag);
            float pressureTimesArea = DynamicPressure(parameters.FluidDensity, flowSpeed) * parameters.Area;

            return pressureTimesArea * (liftCoefficient * liftDirection + dragCoefficient * flowDirection);
        }
    }
}

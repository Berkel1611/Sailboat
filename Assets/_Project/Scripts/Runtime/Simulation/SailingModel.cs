using System;
using Sailboat.Config;
using UnityEngine;

namespace Sailboat.Simulation
{
    /// <summary>
    /// Simplified sailing physics on the water plane.
    /// Each call to <see cref="Step"/> advances the boat by one fixed time step.
    /// </summary>
    public sealed class SailingModel
    {
        private const float FullTurn = 2f * Mathf.PI;

        private readonly SailboatConfig _config;

        public SailingModel(SailboatConfig config)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Computes all forces acting on the boat without moving it.
        /// </summary>
        /// <param name="state">Current state of the boat.</param>
        /// <param name="controls">Current player commands.</param>
        /// <param name="trueWind">Wind velocity in world space, pointing where the wind blows to (m/s).</param>
        public SailboatForces ComputeForces(SailboatState state, SailboatControls controls, Vector2 trueWind)
        {
            Vector2 apparentWind = trueWind - state.Velocity;
            Vector2 sailForce = ComputeSailForce(state, controls, apparentWind);
            Vector2 hullForce = ComputeHullForce(state);

            float forwardSpeed = state.ForwardSpeed;
            float rudderTorque = _config.RudderStrength * controls.Rudder * forwardSpeed * Mathf.Abs(forwardSpeed);
            float dampingTorque = -ComputeResistance(
                state.AngularVelocity,
                _config.AngularLinearDamping,
                _config.AngularQuadraticDamping);

            return new SailboatForces(apparentWind, sailForce, hullForce, rudderTorque, dampingTorque);
        }

        /// <summary>
        /// Advances the simulation by one step using semi-implicit Euler integration.
        /// </summary>
        /// <param name="state">Current state of the boat.</param>
        /// <param name="controls">Current player commands.</param>
        /// <param name="trueWind">Wind velocity in world space, pointing where the wind blows to (m/s).</param>
        /// <param name="deltaTime">Step duration in seconds.</param>
        /// <returns>State of the boat after the step.</returns>
        public SailboatState Step(SailboatState state, SailboatControls controls, Vector2 trueWind, float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return state;
            }

            SailboatForces forces = ComputeForces(state, controls, trueWind);

            Vector2 forward = state.Forward;
            Vector2 right = state.Right;

            // Resistance is applied first and can only slow the boat down to zero, never reverse its motion.
            // The sail force is added afterwards.
            float forwardSpeed = ApplyResistance(state.ForwardSpeed, Vector2.Dot(forces.HullForce, forward), _config.Mass, deltaTime)
                + Vector2.Dot(forces.SailForce, forward) / _config.Mass * deltaTime;
            float lateralSpeed = ApplyResistance(state.LateralSpeed, Vector2.Dot(forces.HullForce, right), _config.Mass, deltaTime)
                + Vector2.Dot(forces.SailForce, right) / _config.Mass * deltaTime;
            Vector2 velocity = forward * forwardSpeed + right * lateralSpeed;

            float angularVelocity = ApplyResistance(state.AngularVelocity, forces.DampingTorque, _config.Inertia, deltaTime)
                + forces.RudderTorque / _config.Inertia * deltaTime;

            // Semi-implicit Euler: position and heading use already updated velocities.
            Vector2 position = state.Position + velocity * deltaTime;
            float heading = Mathf.Repeat(state.Heading + angularVelocity * deltaTime, FullTurn);

            return new SailboatState(position, heading, velocity, angularVelocity);
        }

        /// <summary>
        /// Resistance of a linear-plus-quadratic drag model.
        /// </summary>
        private static float ComputeResistance(float speed, float linear, float quadratic)
        {
            return linear * speed + quadratic * speed * Mathf.Abs(speed);
        }

        /// <summary>
        /// Reduces the speed by the resistance, but never past zero.
        /// </summary>
        private static float ApplyResistance(float speed, float resistance, float inertia, float deltaTime)
        {
            float maxChange = Mathf.Abs(resistance) / inertia * deltaTime;
            return Mathf.MoveTowards(speed, 0f, maxChange);
        }

        private Vector2 ComputeSailForce(SailboatState state, SailboatControls controls, Vector2 apparentWind)
        {
            // At 0 degrees the sail lies along the hull, so its normal points to starboard.
            // Rotating the sail rotates the normal by the same angle.
            float sailHeading = state.Heading + controls.SailAngleDegrees * Mathf.Deg2Rad;
            Vector2 sailNormal = new Vector2(Mathf.Cos(sailHeading), -Mathf.Sin(sailHeading));

            var sail = new FoilParameters(
                _config.SailArea * controls.SailOpening,
                _config.AirDensity,
                _config.LiftScale,
                _config.DragScale,
                _config.ParasiticDrag);

            return FoilMath.ComputeForce(apparentWind, sailNormal, sail);
        }

        private Vector2 ComputeHullForce(SailboatState state)
        {
            // The keel makes lateral resistance much larger than forward resistance.
            float forwardResistance = ComputeResistance(state.ForwardSpeed, _config.ForwardLinearDrag, _config.ForwardQuadraticDrag);
            float lateralResistance = ComputeResistance(state.LateralSpeed, _config.LateralLinearDrag, _config.LateralQuadraticDrag);

            return -(state.Forward * forwardResistance + state.Right * lateralResistance);
        }
    }
}
using NUnit.Framework;
using Sailboat.Simulation;
using UnityEngine;

namespace Sailboat.Tests.EditMode
{
    public sealed class FoilMathTests
    {
        private const float Tolerance = 0.001f;
        private const float ForceTolerance = 0.5f;

        private const float AirDensity = 1.225f;
        private const float FoilArea = 10f;
        private const float ParasiticDrag = 0.25f;

        // Flow of 10 m/s blowing along world +Z. Dynamic pressure = 0.5 * 1.225 * 10^2 = 61.25 Pa.
        private static readonly Vector2 Flow = new Vector2(0f, 10f);
        private const float PressureTimesArea = 61.25f * FoilArea;

        [Test]
        public void DynamicPressure_KnownValues_ReturnsHalfDensityTimesSpeedSquared()
        {
            float pressure = FoilMath.DynamicPressure(AirDensity, 10f);

            Assert.That(pressure, Is.EqualTo(61.25f).Within(Tolerance));
        }

        [Test]
        public void LiftCoefficient_At45Degrees_IsMaximal()
        {
            float coefficient = FoilMath.LiftCoefficient(45f * Mathf.Deg2Rad);

            Assert.That(coefficient, Is.EqualTo(1f).Within(Tolerance));
        }

        [TestCase(0f)]
        [TestCase(90f)]
        public void LiftCoefficient_AtZeroAndRightAngle_IsZero(float angleDegrees)
        {
            float coefficient = FoilMath.LiftCoefficient(angleDegrees * Mathf.Deg2Rad);

            Assert.That(coefficient, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void ComputeForce_NoFlow_ReturnsZero()
        {
            Vector2 force = FoilMath.ComputeForce(Vector2.zero, Vector2.right, CreateFoil());

            Assert.That(force.magnitude, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void ComputeForce_ZeroArea_ReturnsZero()
        {
            var closedFoil = new FoilParameters(0f, AirDensity, 1f, 1f, ParasiticDrag);

            Vector2 force = FoilMath.ComputeForce(Flow, Vector2.up, closedFoil);

            Assert.That(force.magnitude, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void ComputeForce_FlowPerpendicularToFoil_ReturnsOnlyDragAlongFlow()
        {
            // Normal parallel to the flow means the flow hits the surface head-on (90°): drag coefficient = 2.
            Vector2 force = FoilMath.ComputeForce(Flow, Vector2.up, CreateFoil());

            Assert.That(force.x, Is.EqualTo(0f).Within(ForceTolerance));
            Assert.That(force.y, Is.EqualTo(PressureTimesArea * (2f + ParasiticDrag)).Within(ForceTolerance));
        }

        [Test]
        public void ComputeForce_FlowAlongFoil_ReturnsOnlyParasiticDrag()
        {
            // Normal perpendicular to the flow means the flow slides along the surface (0°): no lift, no shape drag.
            Vector2 force = FoilMath.ComputeForce(Flow, Vector2.right, CreateFoil());

            Assert.That(force.x, Is.EqualTo(0f).Within(ForceTolerance));
            Assert.That(force.y, Is.EqualTo(PressureTimesArea * ParasiticDrag).Within(ForceTolerance));
        }

        [Test]
        public void ComputeForce_FlowAt45Degrees_ProducesLiftPerpendicularToFlow()
        {
            // At 45° the lift coefficient is 1 and points to +X, the drag coefficient is 1 + parasitic and points to +Z.
            Vector2 normal = new Vector2(1f, 1f).normalized;

            Vector2 force = FoilMath.ComputeForce(Flow, normal, CreateFoil());

            Assert.That(force.x, Is.EqualTo(PressureTimesArea).Within(ForceTolerance));
            Assert.That(force.y, Is.EqualTo(PressureTimesArea * (1f + ParasiticDrag)).Within(ForceTolerance));
        }

        [Test]
        public void ComputeForce_FlippedNormal_ReturnsSameForce()
        {
            Vector2 normal = new Vector2(0.6f, 0.8f);

            Vector2 force = FoilMath.ComputeForce(Flow, normal, CreateFoil());
            Vector2 flippedForce = FoilMath.ComputeForce(Flow, -normal, CreateFoil());

            Assert.That(flippedForce.x, Is.EqualTo(force.x).Within(ForceTolerance));
            Assert.That(flippedForce.y, Is.EqualTo(force.y).Within(ForceTolerance));
        }

        private static FoilParameters CreateFoil()
        {
            return new FoilParameters(FoilArea, AirDensity, 1f, 1f, ParasiticDrag);
        }
    }
}

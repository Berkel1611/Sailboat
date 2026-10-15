using NUnit.Framework;
using Sailboat.Simulation;
using UnityEngine;

namespace Sailboat.Tests.EditMode
{
    public sealed class WindMathTests
    {
        private const float Tolerance = 0.0001f;
        private const float Speed = 10f;

        [Test]
        public void VelocityFrom_WindFromMinusZ_BlowsTowardsPlusZ()
        {
            // Wind from 180° comes from behind a boat heading 0°, so it pushes along world +Z.
            Vector2 velocity = WindMath.VelocityFrom(180f, Speed);

            Assert.That(velocity.x, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(velocity.y, Is.EqualTo(Speed).Within(Tolerance));
        }

        [Test]
        public void VelocityFrom_WindFromPlusX_BlowsTowardsMinusX()
        {
            Vector2 velocity = WindMath.VelocityFrom(90f, Speed);

            Assert.That(velocity.x, Is.EqualTo(-Speed).Within(Tolerance));
            Assert.That(velocity.y, Is.EqualTo(0f).Within(Tolerance));
        }

        [TestCase(0f)]
        [TestCase(45f)]
        [TestCase(225f)]
        public void VelocityFrom_AnyDirection_KeepsSpeed(float fromDirectionDegrees)
        {
            Vector2 velocity = WindMath.VelocityFrom(fromDirectionDegrees, Speed);

            Assert.That(velocity.magnitude, Is.EqualTo(Speed).Within(Tolerance));
        }
    }
}
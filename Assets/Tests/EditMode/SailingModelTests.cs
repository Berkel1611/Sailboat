using System;
using NUnit.Framework;
using Sailboat.Config;
using Sailboat.Simulation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sailboat.Tests.EditMode
{
    public sealed class SailingModelTests
    {
        private const float TimeStep = 1f / 120f;
        private const float Tolerance = 0.0001f;
        private const float WindSpeed = 10f;

        // The boat starts with heading 0, so its bow faces world +Z.
        private static readonly Vector2 WindFromStern = new Vector2(0f, WindSpeed);
        private static readonly Vector2 WindFromBow = new Vector2(0f, -WindSpeed);

        private SailboatConfig _config;
        private SailingModel _model;

        [SetUp]
        public void SetUp()
        {
            // A config created in code uses the default values from SailboatConfig, not the tuned asset.
            _config = ScriptableObject.CreateInstance<SailboatConfig>();
            _model = new SailingModel(_config);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void Constructor_NullConfig_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SailingModel(null));
        }

        [Test]
        public void Step_ZeroDeltaTime_ReturnsUnchangedState()
        {
            var state = new SailboatState(new Vector2(3f, 4f), 0.5f, new Vector2(1f, 2f), 0.1f);

            SailboatState result = _model.Step(state, FullSail(45f), WindFromStern, 0f);

            Assert.That(result.Position, Is.EqualTo(state.Position));
            Assert.That(result.Velocity, Is.EqualTo(state.Velocity));
            Assert.That(result.Heading, Is.EqualTo(state.Heading));
            Assert.That(result.AngularVelocity, Is.EqualTo(state.AngularVelocity));
        }

        [Test]
        public void Step_NoWind_BoatStaysStill()
        {
            SailboatState result = Simulate(AtRest(), FullSail(45f), Vector2.zero, 10f);

            Assert.That(result.Position.magnitude, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.Velocity.magnitude, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void Step_SailClosed_BoatDoesNotMove()
        {
            var controls = new SailboatControls(0f, 45f, 0f);

            SailboatState result = Simulate(AtRest(), controls, WindFromStern, 10f);

            Assert.That(result.Velocity.magnitude, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void Step_WindFromStern_ReachesSteadySpeedAroundFourMetersPerSecond()
        {
            SailboatState result = Simulate(AtRest(), FullSail(45f), WindFromStern, 120f);

            Assert.That(result.ForwardSpeed, Is.InRange(3.5f, 4.5f));
            Assert.That(result.Position.y, Is.GreaterThan(0f));
        }

        [TestCase(-45f)]
        [TestCase(0f)]
        [TestCase(20f)]
        [TestCase(45f)]
        public void Step_WindFromBow_BoatDoesNotMoveForward(float sailAngleDegrees)
        {
            SailboatState result = Simulate(AtRest(), FullSail(sailAngleDegrees), WindFromBow, 30f);

            Assert.That(result.ForwardSpeed, Is.LessThanOrEqualTo(Tolerance));
        }

        [Test]
        public void Step_RudderWhileAtRest_BoatDoesNotTurn()
        {
            var controls = new SailboatControls(0f, 0f, 1f);

            SailboatState result = Simulate(AtRest(), controls, Vector2.zero, 5f);

            Assert.That(result.AngularVelocity, Is.EqualTo(0f).Within(Tolerance));
            Assert.That(result.Heading, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void Step_RudderToStarboardWhileMoving_BowTurnsRight()
        {
            SailboatState result = Simulate(MovingForward(), new SailboatControls(0f, 0f, 1f), Vector2.zero, 1f);

            Assert.That(SignedHeadingDegrees(result), Is.GreaterThan(0f));
        }

        [Test]
        public void Step_RudderToPortWhileMoving_BowTurnsLeft()
        {
            SailboatState result = Simulate(MovingForward(), new SailboatControls(0f, 0f, -1f), Vector2.zero, 1f);

            Assert.That(SignedHeadingDegrees(result), Is.LessThan(0f));
        }

        [Test]
        public void Step_MovingBackward_SlowsDownFasterThanMovingForward()
        {
            var sailClosed = new SailboatControls(0f, 0f, 0f);

            SailboatState forward = Simulate(MovingForward(), sailClosed, Vector2.zero, 1f);
            SailboatState backward = Simulate(MovingBackward(), sailClosed, Vector2.zero, 1f);

            Assert.That(Mathf.Abs(backward.ForwardSpeed), Is.LessThan(Mathf.Abs(forward.ForwardSpeed)));
        }

        private static SailboatState AtRest()
        {
            return new SailboatState(Vector2.zero, 0f, Vector2.zero, 0f);
        }

        private static SailboatState MovingForward()
        {
            return new SailboatState(Vector2.zero, 0f, new Vector2(0f, 4f), 0f);
        }

        private static SailboatState MovingBackward()
        {
            return new SailboatState(Vector2.zero, 0f, new Vector2(0f, -4f), 0f);
        }

        private static SailboatControls FullSail(float sailAngleDegrees)
        {
            return new SailboatControls(1f, sailAngleDegrees, 0f);
        }

        /// <summary>
        /// Heading in degrees within -180..180, so a small turn to port gives a small negative value instead of ~360.
        /// </summary>
        private static float SignedHeadingDegrees(SailboatState state)
        {
            return Mathf.DeltaAngle(0f, state.HeadingDegrees);
        }

        private SailboatState Simulate(SailboatState state, SailboatControls controls, Vector2 trueWind, float seconds)
        {
            int steps = Mathf.RoundToInt(seconds / TimeStep);
            for (int i = 0; i < steps; i++)
            {
                state = _model.Step(state, controls, trueWind, TimeStep);
            }

            return state;
        }
    }
}
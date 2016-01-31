using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     A 2-dimensional point mass approximation.
    /// </summary>
    public sealed class PointMass : IPointMass
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PointMass" /> class with the specified initial values for the position
        ///     and velocity.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="radius">The radius used for collision checking.</param>
        /// <param name="maxForce">The maximum force.</param>
        /// <param name="maxSpeed">The maximum speed. </param>
        /// <param name="position">The position.</param>
        /// <param name="velocity">The velocity.</param>
        public PointMass(float mass, float radius, float maxForce, float maxSpeed, Vector2 position, Vector2 velocity)
        {
            Contract.Requires<ArgumentException>(mass > 0);
            Contract.Requires<ArgumentException>(radius > 0);
            Contract.Requires<ArgumentException>(maxForce > 0);
            Contract.Requires<ArgumentException>(maxSpeed > 0);

            Mass = mass;
            Radius = radius;
            MaxForce = maxForce;
            MaxSpeed = maxSpeed;
            Position = position;
            Velocity = velocity;
        }

        /// <summary>The maximum force. The value passed to <see cref="SetForce" /> will be truncated to this value.</summary>
        public float MaxForce { get; }

        /// <summary>The maximum speed. <c>Velocity.Length</c> will never exceed this value.</summary>
        public float MaxSpeed { get; }

        /// <summary>The mass.</summary>
        public float Mass { get; }

        /// <summary>The radius used for collision checking.</summary>
        public float Radius { get; }

        /// <summary>The position as calculated by <c>Position + Velocity*time</c>.</summary>
        public Vector2 Position { get; private set; }

        /// <summary>The velocity as calculated by <c>Velocity + Acceleration*time</c>.</summary>
        public Vector2 Velocity { get; private set; }

        /// <summary>The acceleration calculated as <c>Acceleration = force/Mass</c>.</summary>
        public Vector2 Acceleration { get; private set; }

        /// <summary>
        ///     Sets the force on the point mass and recalculates the <see cref="Acceleration" />.
        /// </summary>
        /// <param name="value">The force to be excerted on the point mass.</param>
        public void SetForce(Vector2 value) => Acceleration = value.ClampMagnitude(MaxForce)/Mass;

        /// <summary>
        ///     Simulates the point mass for a specified period of time and updates the position and the velocity.
        /// </summary>
        /// <param name="time">The time period over which the position and velocity should be integrated.</param>
        public void Simulate(float time)
            => Position += (Velocity = (Velocity + Acceleration * time).ClampMagnitude(MaxSpeed)) * time;

        /// <summary>
        ///     Sets the force and simulates the point mass.
        /// </summary>
        /// <param name="force">The force to be excerted on the point mass.</param>
        /// <param name="time">The time period over which the position and velocity should be integrated.</param>
        public void Simulate(Vector2 force, float time)
        {
            SetForce(force);
            Simulate(time);
        }

        /// <summary>
        ///     Invert the velocity and acceleration when a collision occurs
        /// </summary>
        internal void ResolveCollision(Vector2 normal)
            => Velocity = Vector2.Reflect(Velocity, normal);

        internal void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
        }
    }
}
using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    public class PointMass : IPointMass
    {
        public PointMass(float mass, float maxForce, float maxSpeed, Vector2 position, Vector2 velocity)
        {
            Contract.Requires<ArgumentException>(mass > 0);
            Contract.Requires<ArgumentException>(maxForce > 0);
            Contract.Requires<ArgumentException>(maxSpeed > 0);

            Mass = mass;
            MaxForce = maxForce;
            MaxSpeed = maxSpeed;
            Position = position;
            Velocity = velocity;
        }

        public float Mass { get; }

        public float MaxForce { get; }

        public float MaxSpeed { get; }

        public Vector2 Position { get; private set; }

        public Vector2 Velocity { get; private set; }

        public void ApplyForce(Vector2 force, float time)
        {
            // Prevent force from becoming too large and calculate acceleration.
            var acceleration = force.ClampMagnitude(MaxForce)/Mass;

            // Integrate acceleration and prevent particle from exceeding the maximum speed.
            Velocity = (Velocity + acceleration*time).ClampMagnitude(MaxSpeed);

            // Integrate velocity.
            Position += Velocity*time;
        }

        public override string ToString() =>
            "{Mass=" + Mass +
            ",MaxForce=" + MaxForce +
            ",MaxSpeed=" + MaxSpeed +
            ",Position=" + Position +
            ",Velocity=" + Velocity + "}";

        internal void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
        }
    }
}

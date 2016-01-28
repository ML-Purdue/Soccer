using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    public sealed class Vehicle : IVehicle
    {
        private Vector2 _steeringForce;

        public Vehicle(float mass, float maxForce, float maxSpeed, float radius, Vector2 position, Vector2 velocity)
        {
            Contract.Requires<ArgumentException>(mass > 0);
            Contract.Requires<ArgumentException>(maxForce > 0);
            Contract.Requires<ArgumentException>(maxSpeed > 0);
            Contract.Requires<ArgumentException>(radius > 0);

            Mass = mass;
            MaxForce = maxForce;
            MaxSpeed = maxSpeed;
            Radius = radius;
            Position = position;
            Velocity = velocity;
        }

        public float Mass { get; }

        public float MaxForce { get; }

        public float MaxSpeed { get; }

        public float Radius { get; }

        public Vector2 Position { get; private set; }

        public Vector2 Velocity { get; private set; }

        private Vector2 Acceleration => _steeringForce.ClampMagnitude(MaxForce)/Mass;

        public void SetSteeringDirection(Vector2 value) => _steeringForce = value;

        public void Simulate(float time) =>
            Position += (Velocity = (Velocity + Acceleration*time).ClampMagnitude(MaxSpeed))*time;

        internal void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
        }
    }
}
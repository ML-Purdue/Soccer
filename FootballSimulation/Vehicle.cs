using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    public sealed class Vehicle : PointMass
    {
        private IDirection _steeringStrategy = NullSteeringStrategy.Instance;

        public Vehicle(float mass, float maxForce, float maxSpeed, Vector2 position, Vector2 velocity) :
            base(mass, maxForce, maxSpeed, position, velocity)
        {
        }

        public IDirection SteeringStrategy
        {
            get { return _steeringStrategy; }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                _steeringStrategy = value;
            }
        }

        public void Simulate(float time) => ApplyForce(SteeringStrategy.Direction, time);

        public override string ToString() =>
            "{Mass=" + Mass +
            ",MaxForce=" + MaxForce +
            ",MaxSpeed=" + MaxSpeed +
            ",Position=" + Position +
            ",Velocity=" + Velocity +
            ",SteeringStrategy=" + SteeringStrategy.GetType().AssemblyQualifiedName + "}";
    }
}
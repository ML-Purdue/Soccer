using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    public sealed class Vehicle : PointMass, IVehicle
    {
        public Vehicle(float mass, float maxForce, float maxSpeed, Vector2 position, Vector2 velocity) :
            base(mass, maxForce, maxSpeed, position, velocity)
        {
        }

        public IDirection SteeringStrategy { get; set; }

        public void Simulate(float time) => ApplyForce(SteeringStrategy.Direction, time);

        public override string ToString() =>
            "{Mass=" + Mass +
            ",MaxForce=" + MaxForce +
            ",MaxSpeed=" + MaxSpeed +
            ",Position=" + Position +
            ",Velocity=" + Velocity +
            ",SteeringStrategy=" + SteeringStrategy.GetType().AssemblyQualifiedName + "}";

        [ContractInvariantMethod]
        private void Invariants() => Contract.Invariant(SteeringStrategy != null);
    }
}
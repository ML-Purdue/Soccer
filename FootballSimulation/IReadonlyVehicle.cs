namespace FootballSimulation
{
    public interface IReadonlyVehicle : IPointMass
    {
        IDirection SteeringStrategy { get; }
    }
}
namespace FootballSimulation
{
    public interface IVehicle : IReadonlyVehicle
    {
        new IDirection SteeringStrategy { set; }
    }
}
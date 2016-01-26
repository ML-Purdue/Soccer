namespace FootballSimulation
{
    public interface IVehicle
    {
        IDirection SteeringStrategy { get; set; }
    }
}
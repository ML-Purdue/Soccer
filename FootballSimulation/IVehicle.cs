using System.Numerics;

namespace FootballSimulation
{
    public interface IVehicle
    {
        float Mass { get; }

        float MaxForce { get; }

        float MaxSpeed { get; }

        float Radius { get; }

        Vector2 Position { get; }

        Vector2 Velocity { get; }
    }
}
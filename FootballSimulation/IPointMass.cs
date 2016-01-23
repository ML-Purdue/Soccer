using System.Numerics;

namespace FootballSimulation
{
    public interface IPointMass
    {
        float Mass { get; }

        float MaxForce { get; }

        float MaxSpeed { get; }

        Vector2 Position { get; }

        Vector2 Velocity { get; }
    }
}
using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     A 2-dimensional point mass approximation.
    /// </summary>
    public interface IPointMass
    {
        /// <summary>The mass.</summary>
        float Mass { get; }

        /// <summary>The radius used for collision checking.</summary>
        float Radius { get; }

        /// <summary>The position as calculated by <c>Position + Velocity*time</c>.</summary>
        Vector2 Position { get; }

        /// <summary>The velocity as calculated by <c>Velocity + Acceleration*time</c>.</summary>
        Vector2 Velocity { get; }

        /// <summary>The acceleration calculated as <c>Acceleration = force/Mass</c>.</summary>
        Vector2 Acceleration { get; }
    }
}
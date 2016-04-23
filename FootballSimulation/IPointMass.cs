using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     A 2-dimensional point mass approximation.
    /// </summary>
    public interface IPointMass
    {
        /// <summary>The maximum force. <see cref="Force" /> will be truncated to this value.</summary>
        float MaxForce { get; }

        /// <summary>The maximum speed. <c>Velocity.Length</c> will never exceed this value.</summary>
        float MaxSpeed { get; }

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

        /// <summary>The net force on the point mass.</summary>
        Vector2 Force { get; }

        /// <summary>
        /// Id of the player
        /// </summary>
        string id { get; set; }

        /// <summary>
        ///     Calculate the frictionCoefficient force given the friction coefficient between the point mass
        ///     and the medium in which the point mass is moving.
        /// </summary>
        /// <param name="frictionCoefficient">The friction coefficient.</param>
        /// <returns>A vector opposite to the direction of motion representing the friction force.</returns>
        Vector2 GetFriction(float frictionCoefficient);
    }
}
using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     Represents the act of kicking the ball.
    /// </summary>
    public struct Kick
    {
        /// <summary>The player that kicked the ball.</summary>
        public IPointMass Player { get; }

        /// <summary>The desired force on the ball to be exerted by the player.</summary>
        public Vector2 Force { get; }

        /// <summary>Indicates that no player on the team kicked the ball.</summary>
        public static readonly Kick None = new Kick();

        /// <summary>
        ///     Creates a new instance of the <see cref="Kick" /> class with the specified player and force.
        /// </summary>
        /// <param name="player">The player to kick the ball.</param>
        /// <param name="force">The desired force on the ball to be exerted by the player.</param>
        public Kick(IPointMass player, Vector2 force)
        {
            Contract.Requires<ArgumentNullException>(player != null);
            Player = player;
            Force = Vector2.Zero;
        }
    }
}
using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    public struct Kick
    {
        public IVehicle Player { get; }

        public Vector2 Force { get; }

        public static readonly Kick None = new Kick(null, Vector2.Zero);

        public Kick(IVehicle player, Vector2 force)
        {
            Contract.Requires<ArgumentNullException>(player != null);

            Player = player;
            Force = force;
        }
    }
}
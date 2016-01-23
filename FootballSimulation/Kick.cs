using System.Numerics;

namespace FootballSimulation
{
    public struct Kick
    {
        public readonly IVehicle Player;

        public readonly Vector2 Force;

        public static readonly Kick None = new Kick(null, Vector2.Zero);

        public Kick(IVehicle player, Vector2 force)
        {
            Player = player;
            Force = force;
        }
    }
}
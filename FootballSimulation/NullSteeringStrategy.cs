using System.Numerics;

namespace FootballSimulation
{
    public sealed class NullSteeringStrategy : IDirection
    {
        private NullSteeringStrategy()
        {
        }

        public static NullSteeringStrategy Instance => new NullSteeringStrategy();

        public Vector2 Direction => Vector2.Zero;
    }
}
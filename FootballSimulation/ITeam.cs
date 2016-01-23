using System.Collections.Generic;

namespace FootballSimulation
{
    public interface ITeam
    {
        string Name { get; }

        RectangleF GoalBounds { get; }

        IReadOnlyCollection<IReadonlyVehicle> Players { get; }

        int Points { get; }
    }
}
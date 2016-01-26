using System.Collections.ObjectModel;

namespace FootballSimulation
{
    public interface ITeam
    {
        string Strategy { get; }

        RectangleF GoalBounds { get; }

        ReadOnlyCollection<IReadonlyVehicle> Players { get; }

        int Points { get; }
    }
}
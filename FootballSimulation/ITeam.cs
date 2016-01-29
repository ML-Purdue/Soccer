using System.Collections.ObjectModel;

namespace FootballSimulation
{
    public interface ITeam
    {
        ITeamStrategy Strategy { get; }

        RectangleF GoalBounds { get; }

        ReadOnlyCollection<IVehicle> Players { get; }

        int Points { get; }
    }
}
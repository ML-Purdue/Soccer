using System.Collections.ObjectModel;
using System.Drawing;

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
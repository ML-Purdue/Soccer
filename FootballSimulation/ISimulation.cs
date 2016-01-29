using System.Collections.ObjectModel;
using System.Drawing;

namespace FootballSimulation
{
    public interface ISimulation
    {
        ReadOnlyCollection<ITeam> Teams { get; }

        IVehicle Ball { get; }

        RectangleF PitchBounds { get; }
    }
}
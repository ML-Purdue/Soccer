using System.Collections.ObjectModel;

namespace FootballSimulation
{
    public interface ISimulation
    {
        ReadOnlyCollection<ITeam> Teams { get; }

        IVehicle Ball { get; }

        RectangleF PitchBounds { get; }
    }
}
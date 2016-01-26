using System.Collections.ObjectModel;

namespace FootballSimulation
{
    public interface ISimulation
    {
        ReadOnlyCollection<ITeam> Teams { get; }

        IPointMass Ball { get; }

        RectangleF PitchBounds { get; }

        float PlayerRadius { get; }

        float BallRadius { get; }
    }
}
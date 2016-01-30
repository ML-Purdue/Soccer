using System.Collections.ObjectModel;
using System.Drawing;

namespace FootballSimulation
{
    /// <summary>
    ///     Represents a simulation of an indoor football game.
    /// </summary>
    public interface ISimulation
    {
        /// <summary>The teams playing against one another.</summary>
        ReadOnlyCollection<ITeam> Teams { get; }

        /// <summary>The ball.</summary>
        IPointMass Ball { get; }

        /// <summary>The pitch boundaries.</summary>
        RectangleF PitchBounds { get; }
    }
}
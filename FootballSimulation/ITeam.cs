using System.Collections.ObjectModel;
using System.Drawing;

namespace FootballSimulation
{
    /// <summary>
    ///     Represents a simulated team.
    /// </summary>
    public interface ITeam
    {
        /// <summary>The team players.</summary>
        ReadOnlyCollection<IPointMass> Players { get; }

        /// <summary>The bounds of the team's goal.</summary>
        RectangleF GoalBounds { get; }

        /// <summary>The total number of goals scored by the team.</summary>
        int GoalsScored { get; }
    }
}
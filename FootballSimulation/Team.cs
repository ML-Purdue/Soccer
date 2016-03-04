using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;

namespace FootballSimulation
{
    /// <summary>
    ///     Represents a simulated team.
    /// </summary>
    public abstract class Team : ITeam
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Team" /> class with the specified strategy, players, and goal bounds.
        /// </summary>
        /// <param name="players">The team players.</param>
        /// <param name="goalBounds">The bounds of the goal.</param>
        protected Team(ReadOnlyCollection<PointMass> players, RectangleF goalBounds)
        {
            Contract.Requires<ArgumentNullException>(players != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(players, p => p != null));

            Players = players;
            GoalBounds = goalBounds;
        }

        /// <summary>The team players.</summary>
        public ReadOnlyCollection<PointMass> Players { get; }

        /// <summary>The team players.</summary>
        ReadOnlyCollection<IPointMass> ITeam.Players => Players.ToList<IPointMass>().AsReadOnly();

        /// <summary>The bounds of the goal.</summary>
        public RectangleF GoalBounds { get; }

        /// <summary>The total number of goals scored.</summary>
        public int GoalsScored { get; private set; }

        /// <summary>
        ///     Executes the team strategy.
        /// </summary>
        /// <param name="simulation">The simulation in which the team is taking part.</param>
        /// <returns>The kick.</returns>
        public abstract Kick Execute(ISimulation simulation);

        internal void Simulate(float time) => Players.ForEach(p => p.Simulate(time));

        internal void OnGoalScored() => GoalsScored++;
    }
}
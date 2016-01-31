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
    public sealed class Team : ITeam
    {
        private readonly ReadOnlyCollection<PointMass> _players;
        private ITeamStrategy _strategy;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Team" /> class with the specified strategy, players, and goal bounds.
        /// </summary>
        /// <param name="strategy">The team strategy.</param>
        /// <param name="players">The team players.</param>
        /// <param name="goalBounds">The bounds of the goal.</param>
        public Team(ITeamStrategy strategy, ReadOnlyCollection<PointMass> players, RectangleF goalBounds)
        {
            Contract.Requires<ArgumentNullException>(strategy != null);
            Contract.Requires<ArgumentNullException>(players != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(players, p => p != null));
            Contract.Requires<ArgumentException>(goalBounds.Width > 0 && goalBounds.Height > 0);

            _strategy = strategy;
            _players = players;
            GoalBounds = goalBounds;
        }

        /// <summary>The team strategy.</summary>
        public ITeamStrategy Strategy
        {
            get { return _strategy; }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                _strategy = value;
            }
        }

        /// <summary>The team players.</summary>
        public ReadOnlyCollection<PointMass> Players => _players;

        /// <summary>The bounds of the goal.</summary>
        public RectangleF GoalBounds { get; }

        /// <summary>The total number of goals scored.</summary>
        public int GoalsScored { get; private set; }

        /// <summary>Executes the team strategy.</summary>
        /// <param name="simulation">The simulation in which the is taking part.</param>
        /// <returns>The kick.</returns>
        public Kick ExecuteStrategy(ISimulation simulation)
        {
            Contract.Requires<ArgumentNullException>(simulation != null);
            Contract.Requires<ArgumentException>(Contract.Exists(simulation.Teams, t => t == this));

            var kick = _strategy.Execute(simulation, this);
            if (!IsKickValid(kick.Player, simulation))
                throw new InvalidOperationException("Invalid kick.");
            return kick;
        }

        /// <summary>
        ///     Simulates the team for a specified time period and updates the positions and velocities of the team players.
        /// </summary>
        /// <param name="time">The time period.</param>
        public void Simulate(float time) => _players.ForEach(p => p.Simulate(time));

        internal void OnGoalScored() => GoalsScored++;

        private bool IsKickValid(IPointMass player, ISimulation simulation) =>
            _players.Any(p => p == player) &&
            (player.Position - simulation.Ball.Position).Length() < player.Radius + simulation.Ball.Radius;
    }
}
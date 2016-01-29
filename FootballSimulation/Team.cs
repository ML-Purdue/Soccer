using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FootballSimulation
{
    public sealed class Team : ITeam
    {
        private readonly ReadOnlyCollection<Vehicle> _players;

        public Team(ITeamStrategy strategy, ReadOnlyCollection<Vehicle> players, RectangleF goalBounds)
        {
            Contract.Requires<ArgumentNullException>(strategy != null);
            Contract.Requires<ArgumentNullException>(players != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(players, p => p != null));
            Contract.Requires<ArgumentException>(goalBounds.Width > 0 && goalBounds.Height > 0);

            Strategy = strategy;
            _players = players;
            GoalBounds = goalBounds;
        }

        public ITeamStrategy Strategy { get; set; }

        public ReadOnlyCollection<IVehicle> Players => _players.ToList<IVehicle>().AsReadOnly();

        public RectangleF GoalBounds { get; }

        public int Points { get; private set; }

        public Kick ExecuteStrategy(ISimulation simulation)
        {
            Contract.Requires<ArgumentNullException>(simulation != null);

            var kick = Strategy.Execute(simulation, this);
            if (!IsKickValid(kick.Player, simulation))
                throw new InvalidOperationException("Invalid kick.");
            return kick;
        }

        public void Simulate(float time) => _players.ForEach(p => p.Simulate(time));

        internal void OnGoalScored() => Points++;

        private bool IsKickValid(IVehicle player, ISimulation simulation) =>
            _players.Any(p => p == player) && (player.Position - simulation.Ball.Position).Length() <
            player.Radius + simulation.Ball.Radius;
    }
}
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FootballSimulation
{
    public sealed class Team : ITeam
    {
        private readonly ReadOnlyCollection<Vehicle> _players;
        private readonly ITeamStrategy _strategy;

        public Team(ITeamStrategy strategy, ReadOnlyCollection<Vehicle> players, RectangleF goalBounds)
        {
            Contract.Requires<ArgumentNullException>(strategy != null);
            Contract.Requires<ArgumentNullException>(players != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(players, p => p != null));
            Contract.Requires<ArgumentException>(goalBounds.Width > 0 && goalBounds.Height > 0);

            _strategy = strategy;
            _players = players;
            GoalBounds = goalBounds;
        }

        public string Strategy => _strategy.Name;

        public ReadOnlyCollection<IPointMass> Players => _players.ToList<IPointMass>().AsReadOnly();

        public RectangleF GoalBounds { get; }

        public int Points { get; private set; }

        public Kick ExecuteStrategy(ISimulation simulation)
        {
            Contract.Requires<ArgumentNullException>(simulation != null);

            var kick = _strategy.Execute(simulation, this);
            if (!IsKickValid(kick, simulation))
                throw new InvalidOperationException("Invalid kick.");
            return kick;
        }

        public void Simulate(float time) => _players.ForEach(p => p.Simulate(time));

        public override string ToString() =>
            "{StrategyName=" + Strategy +
            ",GoalBounds=" + GoalBounds +
            ",Players={" + string.Join<Vehicle>(",", _players.ToArray()) + "}" +
            ",Points=" + Points + "}";

        internal void OnGoalScored() => Points++;

        private bool IsKickValid(Kick kick, ISimulation simulation) =>
            _players.Any(p => p == kick.Player) && (kick.Player.Position - simulation.Ball.Position).Length() <
                (simulation.PlayerRadius + simulation.BallRadius);
    }
}
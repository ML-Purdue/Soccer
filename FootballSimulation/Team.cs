using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FootballSimulation
{
    public sealed class Team : ITeam
    {
        private readonly ReadOnlyCollection<Vehicle> _players;
        internal ITeamStrategy Strategy;

        public Team(string name, RectangleF goalBounds, ITeamStrategy strategy, ReadOnlyCollection<Vehicle> players)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(strategy != null);
            Contract.Requires<ArgumentNullException>(players != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(players, p => p != null));

            Name = name;
            GoalBounds = goalBounds;
            Strategy = strategy;
            _players = players;
        }

        public string Name { get; }

        public RectangleF GoalBounds { get; }

        public IReadOnlyCollection<IReadonlyVehicle> Players => _players;

        public int Points { get; private set; }

        public Kick ExecuteStrategy() => Strategy.Execute();

        public void Simulate(float time) => _players.ForEach(p => p.Simulate(time));

        public override string ToString() =>
            "{Name=" + Name +
            ",GoalBounds=" + GoalBounds +
            ",Strategy=" + Strategy.GetType().AssemblyQualifiedName +
            ",Players={" + string.Join<Vehicle>(",", _players.ToArray()) + "}}";

        internal void OnGoalScored() => Points++;
    }
}
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace FootballSimulation
{
    public abstract class TeamStrategy : ITeamStrategy
    {
        protected TeamStrategy(ReadOnlyCollection<IVehicle> team, ReadOnlyCollection<IReadonlyVehicle> other, IPointMass ball)
        {
            Contract.Requires<ArgumentNullException>(team != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(team, v => v != null));
            Contract.Requires<ArgumentNullException>(other != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(other, v => v != null));
            Contract.Requires<ArgumentNullException>(ball != null);

            Team = team;
            Other = other;
            Ball = ball;
        }

        protected ReadOnlyCollection<IVehicle> Team { get; }

        protected ReadOnlyCollection<IReadonlyVehicle> Other { get; }

        protected IPointMass Ball { get; }

        public abstract Kick Execute();
    }
}
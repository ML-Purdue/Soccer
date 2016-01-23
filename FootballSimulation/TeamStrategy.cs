using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace FootballSimulation
{
    public abstract class TeamStrategy : ITeamStrategy
    {
        protected TeamStrategy(ReadOnlyCollection<IVehicle> team, ReadOnlyCollection<IReadonlyVehicle> other)
        {
            Contract.Requires<ArgumentNullException>(team != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(team, v => v != null));
            Contract.Requires<ArgumentNullException>(other != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(other, v => v != null));

            Team = team;
            Other = other;
        }

        protected ReadOnlyCollection<IVehicle> Team { get; }

        protected ReadOnlyCollection<IReadonlyVehicle> Other { get; }

        public abstract Kick Execute();
    }
}
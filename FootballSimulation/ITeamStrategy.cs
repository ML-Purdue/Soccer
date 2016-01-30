namespace FootballSimulation
{
    /// <summary>
    ///     Represents the strategy used by a team to manipulate the players.
    /// </summary>
    public interface ITeamStrategy
    {
        /// <summary>The name.</summary>
        string Name { get; }

        /// <summary>
        ///     Executes the strategy.
        /// </summary>
        /// <param name="simulation">The simulation in which the team is participating.</param>
        /// <param name="team">The team on which the strategy is to be applied.</param>
        /// <returns>A <see cref="Kick" /> object.</returns>
        Kick Execute(ISimulation simulation, Team team);
    }
}
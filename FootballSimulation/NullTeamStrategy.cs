namespace FootballSimulation
{
    /// <summary>
    ///     The default implementation of the <see cref="ITeamStrategy" /> interface.
    /// </summary>
    public sealed class NullTeamStrategy : ITeamStrategy
    {
        private NullTeamStrategy()
        {
        }

        /// <summary>An instance of the <see cref="NullTeamStrategy" /> class.</summary>
        public static NullTeamStrategy Instance => new NullTeamStrategy();

        /// <summary>Null</summary>
        public string Name => "Null";

        /// <summary>
        ///     Returns <c>Kick.None</c>.
        /// </summary>
        public Kick Execute(IFootballSimulation simulation, Team team) => Kick.None;
    }
}
namespace FootballSimulation
{
    public sealed class NullTeamStrategy : ITeamStrategy
    {
        private NullTeamStrategy()
        {
        }

        public static NullTeamStrategy Instance => new NullTeamStrategy();

        public Kick Execute() => Kick.None;
    }
}
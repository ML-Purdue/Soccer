namespace FootballSimulation
{
    public sealed class NullTeamStrategy : ITeamStrategy
    {
        private NullTeamStrategy()
        {
        }

        public static NullTeamStrategy Instance => new NullTeamStrategy();

        public string Name => "Null";

        public Kick Execute(ISimulation simulation, Team team) => Kick.None;
    }
}
namespace FootballSimulation
{
    public interface ITeamStrategy
    {
        string Name { get; }

        Kick Execute(ISimulation simulation, Team team);
    }
}
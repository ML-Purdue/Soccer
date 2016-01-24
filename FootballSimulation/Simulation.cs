using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;

namespace FootballSimulation
{
    public enum SimulationState
    {
        Resetting,
        Playing
    }

    public sealed class Simulation
    {
        public delegate void SimulateHandler(float time);

        private const float EpsilonSquared = 1;

        private readonly PointMass _ball;
        private readonly Vector2 _ballStartingPosition;
        private readonly SimulateHandler _simulatePlayingHandler;
        private readonly SimulateHandler _simulateResettingHandler;
        private readonly IEnumerable<IEnumerable<Vector2>> _startingPositions;
        private readonly IReadOnlyCollection<Team> _teams;

        public SimulateHandler Simulate;

        public Simulation(
            IReadOnlyCollection<Team> teams,
            PointMass ball,
            RectangleF pitchBounds,
            float playerRadius,
            float ballRadius)
        {
            Contract.Requires<ArgumentNullException>(teams != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(teams, t => t != null));
            Contract.Requires<ArgumentNullException>(ball != null);
            Contract.Requires<ArgumentException>(playerRadius > 0);
            Contract.Requires<ArgumentException>(ballRadius > 0);

            _simulateResettingHandler = SimulateResetting;
            _simulatePlayingHandler = SimulatePlaying;
            _teams = teams;
            _startingPositions = GetStartingPositions(teams);
            _ball = ball;
            _ballStartingPosition = ball.Position;
            PitchBounds = pitchBounds;
            PlayerRadius = playerRadius;
            BallRadius = ballRadius;
            Simulate = _simulatePlayingHandler;
        }

        public IReadOnlyCollection<ITeam> Teams => _teams;

        public IPointMass Ball => _ball;

        public RectangleF PitchBounds { get; }

        public float PlayerRadius { get; }

        public float BallRadius { get; }

        public EventHandler<Team> GoalScored { get; set; }

        public SimulationState State =>
            Simulate == _simulatePlayingHandler ? SimulationState.Playing : SimulationState.Resetting;

        private static IEnumerable<IEnumerable<Vector2>> GetStartingPositions(IEnumerable<Team> teams) =>
            teams.ToList().Select(t => from p in t.Players select p.Position);

        private void SimulatePlaying(float time)
        {
            var kicks = ExecuteStrategies();
            SimulateTeams(time);
            SimulateBall(kicks, time);
            CheckGoals();
        }

        private IEnumerable<Kick> ExecuteStrategies() =>
            _teams.Select(team =>
            {
                var kick = team.ExecuteStrategy();
                Contract.Requires<ArgumentException>(Contract.Exists(team.Players, p => p == kick.Player));
                return kick;
            });

        private Vector2 ResolveBallDirection(IEnumerable<Kick> kicks)
        {
            throw new NotImplementedException();
        }

        private void SimulateTeams(float time) =>
            _teams.ForEach(t => t.Simulate(time));

        private void SimulateBall(IEnumerable<Kick> kicks, float time) =>
            _ball.ApplyForce(ResolveBallDirection(kicks), time);

        private void CheckGoals() =>
            _teams.Where(t => t.GoalBounds.Contains(_ball.Position)).ForEach(OnGoalScored);

        private void SimulateResetting(float time)
        {
            ExecuteStrategies();
            SimulateTeams(time);
            if (AreTeamsReset(EpsilonSquared))
                OnReset();
        }

        private bool AreTeamsReset(float epsilonSquared) =>
            _teams.Zip(_startingPositions, (team, positions) =>
                positions.Zip(from p in team.Players select p.Position,
                    (a, b) => (a - b).LengthSquared() <= epsilonSquared).All(x => x)).All(x => x);

        private void OnBeginReset()
        {
            _teams.ForEach(t => t.Strategy = NullTeamStrategy.Instance); // TODO: Switch to MoveToStrategy.
            Simulate = _simulateResettingHandler;
        }

        private void OnReset()
        {
            _ball.Reset(_ballStartingPosition);
            Simulate = _simulatePlayingHandler;
        }

        private void OnGoalScored(Team team)
        {
            team.OnGoalScored();
            GoalScored?.Invoke(this, team);
            OnBeginReset();
        }
    }
}
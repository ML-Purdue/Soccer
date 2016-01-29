using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;

namespace FootballSimulation
{
    public sealed class FootballSimulation : ISimulation
    {
        private readonly Vehicle _ball;
        private readonly Vector2 _ballStartingPosition;
        private readonly IEnumerable<IEnumerable<Vector2>> _startingPositions;
        private readonly ReadOnlyCollection<Team> _teams;

        private SimulateState _simulate;

        public FootballSimulation(ReadOnlyCollection<Team> teams, Vehicle ball, RectangleF pitchBounds)
        {
            Contract.Requires<ArgumentNullException>(teams != null);
            Contract.Requires<ArgumentNullException>(ball != null);
            Contract.Requires<ArgumentException>(pitchBounds.Width > 0 && pitchBounds.Height > 0);
            Contract.Requires<ArgumentException>(Contract.ForAll(teams, t => t != null));
            Contract.Requires<ArgumentException>(Contract.ForAll(teams, t => pitchBounds.Contains(t.GoalBounds)));
            Contract.Requires<ArgumentException>(Contract.ForAll(teams,
                t => Contract.ForAll(t.Players, p => pitchBounds.Contains(p.Position))));
            Contract.Requires<ArgumentException>(pitchBounds.Contains(ball.Position));

            _simulate = SimulatePlaying;
            _teams = teams;
            _startingPositions = GetStartingPositions(teams);
            _ball = ball;
            _ballStartingPosition = ball.Position;
            PitchBounds = pitchBounds;
        }

        public EventHandler<Team> GoalScored { get; set; }

        private bool IsReset =>
            _teams.Zip(_startingPositions, (t, s) => t.Players.Zip(s, (a, b) =>
                (a.Position - b).LengthSquared() < a.Radius).All(x => x)).All(x => x);

        public ReadOnlyCollection<ITeam> Teams => _teams.ToList<ITeam>().AsReadOnly();

        public IVehicle Ball => _ball;

        public RectangleF PitchBounds { get; }

        private static IEnumerable<IEnumerable<Vector2>> GetStartingPositions(IEnumerable<Team> teams) =>
            from t in teams select from p in t.Players select p.Position;

        public void Simulate(float time) => _simulate(time);

        private void SimulatePlaying(float time)
        {
            var kicks = from team in _teams select team.ExecuteStrategy(this);
            _teams.ForEach(t => t.Simulate(time));
            _ball.SetSteeringDirection(ResolveBallDirection(kicks));
            _ball.Simulate(time);
            _teams.Where(t => t.GoalBounds.Contains(_ball.Position)).ForEach(OnGoalScored);
        }

        private void SimulateResetting(float time)
        {
            _teams.ForEach(t => t.ExecuteStrategy(this));
            _teams.ForEach(t => t.Simulate(time));
            if (IsReset) OnReset();
        }

        private Vector2 ResolveBallDirection(IEnumerable<Kick> kicks)
        {
            // Don't forget to take into account collision with pitch boundaries.
            throw new NotImplementedException();
        }

        private void OnGoalScored(Team team)
        {
            team.OnGoalScored();
            GoalScored?.Invoke(this, team);
            _teams.ForEach(t => t.Strategy = NullTeamStrategy.Instance); // TODO: Change to TeamResetStrategy.
            _simulate = SimulateResetting;
        }
        
        private void OnReset()
        {
            _teams.ForEach(t => t.Strategy = NullTeamStrategy.Instance);
            _ball.Reset(_ballStartingPosition);
            _simulate = SimulatePlaying;
        }

        private delegate void SimulateState(float time);
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;

namespace FootballSimulation
{
    public sealed class Simulation : ISimulation
    {
        private readonly PointMass _ball;
        private readonly Vector2 _ballStartingPosition;
        private readonly IEnumerable<Reset> _resets;
        private readonly ReadOnlyCollection<Team> _teams;

        private SimulateState _simulate;

        public Simulation(
            ReadOnlyCollection<Team> teams,
            PointMass ball,
            RectangleF pitchBounds,
            float playerRadius,
            float ballRadius)
        {
            Contract.Requires<ArgumentNullException>(teams != null);
            Contract.Requires<ArgumentNullException>(ball != null);
            Contract.Requires<ArgumentException>(pitchBounds.Width > 0 && pitchBounds.Height > 0);
            Contract.Requires<ArgumentException>(playerRadius > 0);
            Contract.Requires<ArgumentException>(ballRadius > 0);
            Contract.Requires<ArgumentException>(Contract.ForAll(teams, t => t != null));
            Contract.Requires<ArgumentException>(Contract.ForAll(teams, t => pitchBounds.Contains(t.GoalBounds)));
            Contract.Requires<ArgumentException>(Contract.ForAll(teams,
                t => Contract.ForAll(t.Players, p => pitchBounds.Contains(p.Position))));
            Contract.Requires<ArgumentException>(pitchBounds.Contains(ball.Position));

            _simulate = SimulatePlaying;
            _teams = teams;
            _resets = from t in teams select new Reset(t);
            _ball = ball;
            _ballStartingPosition = ball.Position;
            PitchBounds = pitchBounds;
            PlayerRadius = playerRadius;
            BallRadius = ballRadius;
        }

        public EventHandler<Team> GoalScored { get; set; }

        public ReadOnlyCollection<ITeam> Teams => _teams.ToList<ITeam>().AsReadOnly();

        public IPointMass Ball => _ball;

        public RectangleF PitchBounds { get; }

        public float PlayerRadius { get; }

        public float BallRadius { get; }

        public void Simulate(float time) => _simulate(time);

        private void SimulatePlaying(float time)
        {
            var kicks = from team in _teams select team.ExecuteStrategy(this);
            _teams.ForEach(t => t.Simulate(time));
            _ball.ApplyForce(ResolveBallDirection(kicks), time);
            _teams.Where(t => t.GoalBounds.Contains(_ball.Position)).ForEach(OnGoalScored);
        }

        private void SimulateResetting(float time)
        {
            _resets.ForEach(s => s.Execute());
            _teams.ForEach(t => t.Simulate(time));
            if (_resets.All(s => s.IsReset))
                return;
            _ball.Reset(_ballStartingPosition);
            _simulate = SimulatePlaying;
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
            _simulate = SimulateResetting;
        }

        private delegate void SimulateState(float time);

        private sealed class Reset
        {
            private const float Epsilon = 1;
            private readonly IEnumerable<Vector2> _positions;
            private readonly Team _team;

            public Reset(Team team)
            {
                _team = team;
                _positions = from p in team.Players select p.Position;
            }

            public bool IsReset =>
                _positions.Zip(from p in _team.Players select p.Position,
                    (a, b) => (a - b).LengthSquared() <= Epsilon).All(x => x);

            public void Execute()
            {
                throw new NotImplementedException();
            }
        }
    }
}
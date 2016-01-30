using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     Represents a simulation of an indoor football game.
    /// </summary>
    public sealed class Simulation : ISimulation
    {
        private readonly PointMass _ball;
        private readonly Vector2 _ballStartingPosition;
        private readonly IEnumerable<IEnumerable<Vector2>> _startingPositions;
        private readonly ReadOnlyCollection<Team> _teams;

        private SimulateState _simulate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Simulation" /> class.
        /// </summary>
        /// <param name="teams">The teams to be played against each other.</param>
        /// <param name="ball">The ball.</param>
        /// <param name="pitchBounds">The pitch boundaries.</param>
        public Simulation(ReadOnlyCollection<Team> teams, PointMass ball, RectangleF pitchBounds)
        {
            Contract.Requires<ArgumentNullException>(teams != null);
            Contract.Requires<ArgumentNullException>(ball != null);
            Contract.Requires<ArgumentException>(pitchBounds.Width > 0 && pitchBounds.Height > 0);
            Contract.Requires<ArgumentException>(Contract.ForAll(teams, t => IsTeamValid(t, pitchBounds)));
            Contract.Requires<ArgumentException>(pitchBounds.Contains(ball.Position));

            _simulate = SimulatePlaying;
            _teams = teams;
            _startingPositions = GetStartingPositions(teams);
            _ball = ball;
            _ballStartingPosition = ball.Position;
            PitchBounds = pitchBounds;
        }

        /// <summary>Called when a goal is scored.</summary>
        public EventHandler<Team> GoalScored { get; set; }

        private bool IsReset
            =>
                _teams.Zip(_startingPositions,
                    (t, s) => t.Players.Zip(s, (a, b) => (a.Position - b).LengthSquared() < a.Radius).All(x => x))
                    .All(x => x);

        /// <summary>The teams playing against one another.</summary>
        public ReadOnlyCollection<ITeam> Teams => _teams.ToList<ITeam>().AsReadOnly();

        /// <summary>The ball.</summary>
        public IPointMass Ball => _ball;

        /// <summary>The pitch boundaries.</summary>
        public RectangleF PitchBounds { get; }

        private static bool IsTeamValid(Team team, RectangleF pitchBounds)
            =>
                team != null && pitchBounds.Contains(team.GoalBounds) &&
                team.Players.All(p => pitchBounds.Contains(p.Position));

        private static IEnumerable<IEnumerable<Vector2>> GetStartingPositions(IEnumerable<Team> teams)
            => from t in teams select from p in t.Players select p.Position;

        /// <summary>
        ///     Simulates one step of the football game.
        /// </summary>
        /// <param name="time">The time step length.</param>
        public void Simulate(float time) => _simulate(time);

        private void SimulatePlaying(float time)
        {
            var kicks = from team in _teams select team.ExecuteStrategy(this);
            _teams.ForEach(t => t.Simulate(time));
            _ball.SetForce(ResolveBallDirection(kicks));
            _ball.Simulate(time);
            // TODO: Deal with the ball bouncing off players
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
            // Deal with the kicks
            var combinedKickForce = new Vector2(0);
            kicks.ForEach(k => combinedKickForce += k.Force);
            return combinedKickForce;
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
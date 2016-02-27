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
        /// <param name="friction">The friction coefficient.</param>
        public Simulation(ReadOnlyCollection<Team> teams, PointMass ball, RectangleF pitchBounds, float friction)
        {
            Contract.Requires<ArgumentNullException>(teams != null);
            Contract.Requires<ArgumentNullException>(ball != null);
            Contract.Requires<ArgumentException>(friction >= 0);
            Contract.Requires<ArgumentException>(pitchBounds.Width > 0 && pitchBounds.Height > 0);
            Contract.Requires<ArgumentException>(Contract.ForAll(teams, t => t != null && t.IsValid(pitchBounds)));
            Contract.Requires<ArgumentException>(pitchBounds.Contains(ball.Position));

            _simulate = SimulatePlaying;
            _teams = teams;
            _startingPositions = new[] { teams[0].PlayerPositions, teams[1].PlayerPositions }; // TODO: Fix this
            _ball = ball;
            _ballStartingPosition = ball.Position;
            PitchBounds = pitchBounds;
            Friction = friction;
        }
        
        /// <summary>The teams playing against one another.</summary>
        public ReadOnlyCollection<ITeam> Teams => _teams.ToList<ITeam>().AsReadOnly();

        /// <summary>The ball.</summary>
        public IPointMass Ball => _ball;

        /// <summary>The pitch boundaries.</summary>
        public RectangleF PitchBounds { get; }

        /// <summary>The friction between the pitch and the ball.</summary>
        public float Friction { get; }

        /// <summary>
        ///     Simulates one step of the football game.
        /// </summary>
        /// <param name="time">The time step length.</param>
        public void Simulate(float time) => _simulate(time);

        // Execute the strategies and simulate the teams and ball.
        private void SimulatePlaying(float time)
        {
            var kicks = from team in _teams select team.ExecuteStrategy(this);
            _teams.ForEach(t => t.Simulate(time));
            SimulateBall(time, kicks);
        }

        // Moves the players of both teams back to their starting positions.
        private void SimulateResetting(float time)
        {
            if (_teams.Zip(_startingPositions, (t, s) => t.Players.Zip(s, (p, q) =>
            {
                var slowingRadius = 10;
                p.SetForce(SteeringStrategies.Arrive(p, q, p.MaxSpeed, slowingRadius));
                p.Simulate(time);
                return (p.Position - q).LengthSquared() < p.Radius;
            }).All(x => x)).All(x => x)) OnReset();
        }

        // Updates the ball's position and velocity. Also handles collisions and goals.
        private void SimulateBall(float time, IEnumerable<Kick> kicks)
        {
            _ball.SetForce(ResolveBallDirection(kicks));
            _ball.Simulate(time);
            var collision = CollisionMath.CircleRectangleCollide(_ball.Position, _ball.Radius, PitchBounds);
            if (collision != null) _ball.ResolveCollision(collision.Value.Normal);
            _teams.Where(t => t.GoalBounds.Contains(_ball.Position)).ForEach(OnGoalScored);
        }

        // Determines the ball's direction after it is kicked.
        // TODO: Need to take the players' orientations relative to their velocities into account.
        private Vector2 ResolveBallDirection(IEnumerable<Kick> kicks)
        {
            var totalKickForce = Vector2.Zero;
            kicks.ForEach(k => totalKickForce += k.Force);
            return totalKickForce + _ball.GetFriction(Friction);
        }

        // Called when a goal is scored.
        private void OnGoalScored(Team scoringTeam)
        {
            scoringTeam.OnGoalScored();
            _simulate = SimulateResetting;
        }

        // Called when the team's have reached their starting positions after executing a reset.
        private void OnReset()
        {
            _ball.Reset(_ballStartingPosition);
            _simulate = SimulatePlaying;
        }

        private delegate void SimulateState(float time);
    }
}
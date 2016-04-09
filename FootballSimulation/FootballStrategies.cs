using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    /// </summary>
    public static class FootballStrategies
    {
        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="otherPlayers"></param>
        /// <param name="passingLaneWidth"></param>
        /// <returns></returns>
        public static bool IsPlayerOpenForPass(
            IPointMass player,
            IPointMass target,
            IEnumerable<IPointMass> otherPlayers,
            float passingLaneWidth)
        {
            var angle = Math.PI/2 -
                        Math.Atan2(target.Position.Y - player.Position.Y, target.Position.X - player.Position.X);
            var v = passingLaneWidth/2*new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));

            return
                otherPlayers.All(
                    p => !PointInTriangle(p.Position, player.Position, v + target.Position, target.Position - v));
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        // TODO: Finish this
        public static Kick PassToPlayer(PointMass player, PointMass target)
        {
            return new Kick(player, Vector2.Zero); // force will be based on distance between the two players
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="team"></param>
        /// <param name="otherTeam"></param>
        /// <param name="simulation"></param>
        /// <param name="passingLaneWidth"></param>
        /// <returns></returns>
        public static IEnumerable<IPointMass> GetOpenPlayers(
            PointMass player,
            ITeam team,
            ITeam otherTeam,
            ISimulation simulation,
            float passingLaneWidth)
            => from p in team.Players
                where p != player && IsPlayerOpenForPass(player, p, otherTeam.Players, passingLaneWidth)
                select p;

        /// <summary>
        /// </summary>
        /// <param name="players"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static IPointMass ClosestPlayerToPoint(IEnumerable<IPointMass> players, IPointMass target, float max)
        {
            IPointMass closest = null;
            var len = float.PositiveInfinity;

            foreach (var p in players)
            {
                var l = (p.Position - SteeringStrategies.FuturePosition(p, target, max)).Length();
                if (l > len) continue;
                closest = p;
                len = l;
            }

            return closest;
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="allPlayers"></param>
        /// <param name="pitchBounds"></param>
        /// <param name="playerOverlapRadius"></param>
        /// <param name="edgeOverlapRadius"></param>
        public static void SpreadOut(
            PointMass player,
            IEnumerable<IPointMass> allPlayers,
            RectangleF pitchBounds,
            float playerOverlapRadius,
            float edgeOverlapRadius)
        {
            var v = (from otherPlayer in allPlayers
                where player != otherPlayer
                select player.Position - otherPlayer.Position
                into between
                where between.Length() < playerOverlapRadius
                select between).Aggregate(Vector2.Zero, (current, between) => current + Vector2.Normalize(between));

            var force = Vector2.Normalize(v)*10;
            var stoppingForce = -player.Velocity;

            force = v.Length() > 0 ? force : stoppingForce;

            if (Math.Abs(player.Position.X - pitchBounds.X) < edgeOverlapRadius && player.Velocity.X < 0)
                force = new Vector2(-player.Velocity.X, force.Y);
            if (Math.Abs(player.Position.X - pitchBounds.Right) < edgeOverlapRadius && player.Velocity.X > 0)
                force = new Vector2(-player.Velocity.X, force.Y);
            if (Math.Abs(player.Position.Y - pitchBounds.Y) < edgeOverlapRadius && player.Velocity.Y < 0)
                force = new Vector2(force.X, -player.Velocity.Y);
            if (Math.Abs(player.Position.Y - pitchBounds.Bottom) < edgeOverlapRadius && player.Velocity.Y > 0)
                force = new Vector2(force.X, -player.Velocity.Y);

            player.Force = force;
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
            => (p1.X - p3.X)*(p2.Y - p3.Y) - (p2.X - p3.X)*(p1.Y - p3.Y);

        private static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            var b1 = Sign(pt, v1, v2) < 0.0f;
            var b2 = Sign(pt, v2, v3) < 0.0f;
            var b3 = Sign(pt, v3, v1) < 0.0f;

            return b1 == b2 && b2 == b3;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FootballSimulation
{
    public static class FootballStrategies
    {

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
            => (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);

        private static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            var b1 = Sign(pt, v1, v2) < 0.0f;
            var b2 = Sign(pt, v2, v3) < 0.0f;
            var b3 = Sign(pt, v3, v1) < 0.0f;

            return b1 == b2 && b2 == b3;
        }

        public static bool IsPlayerOpenForPass(
            IPointMass player,
            IPointMass target,
            IEnumerable<IPointMass> otherPlayers,
            float passingLaneWidth)
        {
            var angle = Math.PI / 2 - Math.Atan2(target.Position.Y - player.Position.Y, target.Position.X - player.Position.X);
            var v = passingLaneWidth / 2 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            foreach (var p in otherPlayers)
                if (PointInTriangle(p.Position, player.Position, v + target.Position, target.Position - v))
                    return false;

            return true;
        }

        // TODO: Finish this
        public static Kick PassToPlayer(PointMass player, PointMass target)
        {
            return new Kick(player, Vector2.Zero); // force will be based on distance between the two players
        }

        public static IEnumerable<IPointMass> GetOpenPlayers(
            PointMass player,
            ITeam team,
            ITeam otherTeam,
            ISimulation simulation,
            float passingLaneWidth)
            => from p in team.Players
               where p != player && IsPlayerOpenForPass(player, p, otherTeam.Players, passingLaneWidth)
               select p;

        public static Kick ChaseBall(PointMass player, Team team, IPointMass ball, ISimulation simulation)
        {
            var otherTeam = simulation.Teams[0] != team ? simulation.Teams[0] : simulation.Teams[1];

            player.Force = SteeringStrategies.Pursue(player, (PointMass)ball, 1);
            if ((player.Position - ball.Position).Length() < 20)
                return new Kick(player, new Vector2(-100, 0));

            return Kick.None;
        }

        public static IPointMass ClosestToPoint(IEnumerable<IPointMass> players, PointMass target, float max)
        {
            var closest = players.First();
            var len = (closest.Position - SteeringStrategies.FuturePosition((PointMass)players.First(), target, max)).Length();

            players.ForEach(p =>
            {
                var l = (p.Position - SteeringStrategies.FuturePosition((PointMass)p, target, max)).Length();
                if (l < len)
                {
                    closest = p;
                    len = l;
                }
            });

            return closest;
        }

        public static void SpreadOut(
            PointMass player,
            IEnumerable<IPointMass> allPlayers,
            RectangleF pitchBounds,
            float PlayerOverlapRadius,
            float EdgeOverlapRadius)
        {
            Vector2 v = Vector2.Zero;

            foreach (var otherPlayer in allPlayers)
            {
                if (player == otherPlayer)
                    continue;

                var between = player.Position - otherPlayer.Position;
                if (between.Length() < PlayerOverlapRadius)
                    v += Vector2.Normalize(between);
            }

            var force = Vector2.Normalize(v) * 10;
            var stoppingForce = -player.Velocity;

            force = v.Length() > 0 ? force : stoppingForce;

            if (Math.Abs(player.Position.X - pitchBounds.X) < EdgeOverlapRadius && player.Velocity.X < 0)
                force = new Vector2(-player.Velocity.X, force.Y);
            if (Math.Abs(player.Position.X - pitchBounds.Right) < EdgeOverlapRadius && player.Velocity.X > 0)
                force = new Vector2(-player.Velocity.X, force.Y);
            if (Math.Abs(player.Position.Y - pitchBounds.Y) < EdgeOverlapRadius && player.Velocity.Y < 0)
                force = new Vector2(force.X, -player.Velocity.Y);
            if (Math.Abs(player.Position.Y - pitchBounds.Bottom) < EdgeOverlapRadius && player.Velocity.Y > 0)
                force = new Vector2(force.X, -player.Velocity.Y);

            player.Force = force;
        }
    }
}

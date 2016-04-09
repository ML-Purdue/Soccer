using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     Steering Behaviors based on http://www.red3d.com/cwr/steer/gdc99/
    /// </summary>
    public static class SteeringStrategies
    {
        private static readonly ReadOnlyCollection<float> TimeFactorTable =
            new[]
            {
                4.00f, // ahead  parallel
                1.80f, // ahead  perpendicular
                0.85f, // ahead  anti-parallel
                1.00f, // aside  parallel
                0.80f, // aside  perpendicular
                4.00f, // aside  anti-parallel
                0.50f, // behind parallel 
                2.00f, // behind perpendicular
                2.00f // behind anti-parallel
            }.ToList().AsReadOnly();

        /// <summary>
        ///     Move to and stop at a specified position.
        /// </summary>
        /// <param name="player">The player that should arrive at the specified position.</param>
        /// <param name="target">The target position.</param>
        /// <param name="maxSpeed">The maximum speed at which the player should move towards the target position.</param>
        /// <param name="slowingRadius">The radius in which the player should begin to slow down.</param>
        /// <returns>The force vector that should be applied to the player.</returns>
        public static Vector2 Arrive(IPointMass player, Vector2 target, float maxSpeed, float slowingRadius)
        {
            Contract.Requires<ArgumentException>(player != null);
            Contract.Requires<ArgumentException>(maxSpeed >= 0);
            Contract.Requires<ArgumentException>(slowingRadius >= 0);

            var targetOffset = target - player.Position;
            var distance = targetOffset.Length();
            var rampedSpeed = maxSpeed*(distance/slowingRadius);
            var clippedSpeed = Math.Min(rampedSpeed, player.MaxSpeed);
            var direction = targetOffset/distance;
            var desiredVelocity = clippedSpeed*direction;

            return desiredVelocity - player.Velocity;
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static float GetSlowingRadius(IPointMass player)
        {
            Contract.Requires<ArgumentException>(player != null);
            return player.MaxSpeed*player.MaxSpeed/(2*player.MaxForce/player.Mass);
        }

        /// <summary>
        ///     Move toward a specified position.
        /// </summary>
        /// <param name="player">The player that should seek the specified position.</param>
        /// <param name="target">The target position.</param>
        /// <param name="desiredSpeed">The desired speed at which the player should move towards the target.</param>
        /// <returns>The force vector that should be applied to the player.</returns>
        public static Vector2 Seek(IPointMass player, Vector2 target, float desiredSpeed)
        {
            Contract.Requires<ArgumentException>(player != null);
            Contract.Requires<ArgumentException>(desiredSpeed >= 0);

            return Vector2.Normalize(target - player.Position)*desiredSpeed - player.Velocity;
        }

        /// <summary>
        ///     Estimates the future position of target.
        /// </summary>
        /// <param name="player">The player on which the future position estimate is to be performed.</param>
        /// <param name="target">The target position of the player.</param>
        /// <param name="max">The maximum estimated time.</param>
        /// <returns>The future position of the player.</returns>
        public static Vector2 FuturePosition(IPointMass player, IPointMass target, float max)
        {
            Contract.Requires<ArgumentException>(player != null);
            Contract.Requires<ArgumentException>(target != null);
            Contract.Requires<ArgumentException>(max >= 0);

            var estimatedTime = max;
            var invSpeed = 1/player.Velocity.Length();

            if (!float.IsNaN(invSpeed))
            {
                int p, f;

                var offset = target.Position - player.Position;
                var parallelness = Vector2.Dot(Vector2.Normalize(player.Velocity), Vector2.Normalize(target.Velocity));
                if (float.IsNaN(parallelness)) p = 1;
                else p = parallelness < -0.707f ? 2 : (parallelness > 0.707f ? 0 : 1);
                var forwardness = Vector2.Dot(Vector2.Normalize(player.Velocity), Vector2.Normalize(offset));
                if (float.IsNaN(forwardness)) f = 3;
                else f = forwardness < -0.707f ? 6 : (forwardness > 0.707f ? 0 : 3);

                estimatedTime = Math.Min(offset.Length()*invSpeed*TimeFactorTable[p + f], max);
            }

            return target.Position + target.Velocity*estimatedTime;
        }

        /// <summary>
        ///     Move toward a target's future position. This method assumes the player will move at their maximum speed.  It also
        ///     ignores friction on the target.
        /// </summary>
        /// <param name="player">The player that should pursue the specified location.</param>
        /// <param name="target">The target position that the player should pursue.</param>
        /// <param name="maxEstimatedTime">The maximum estimated time.</param>
        /// <returns>The force vector that shoud be applied to the player.</returns>
        public static Vector2 Pursue(IPointMass player, IPointMass target, float maxEstimatedTime)
            => Seek(player, FuturePosition(player, target, maxEstimatedTime), player.MaxSpeed);
    }
}
using System;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     Steering Behaviors based on http://www.red3d.com/cwr/steer/gdc99/
    /// </summary>
    public static class SteeringStrategies
    {
        /// <summary>
        ///     Move to and stop at a specified position.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="slowingRadius"></param>
        /// <returns></returns>
        public static Vector2 Arrive(PointMass player, Vector2 target, float maxSpeed, float slowingRadius)
        {
            Contract.Requires<ArgumentException>(player != null);

            var targetOffset = target - player.Position;
            var distance = targetOffset.Length();
            var rampedSpeed = maxSpeed * (distance / slowingRadius);
            var clippedSpeed = Math.Min(rampedSpeed, player.MaxSpeed);
            var direction = targetOffset / distance;
            var desiredVelocity = clippedSpeed * direction;

            return desiredVelocity - player.Velocity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static float GetSlowingRadius(PointMass player)
        {
            Contract.Requires<ArgumentException>(player != null);

            return player.MaxSpeed * player.MaxSpeed / (2 * player.MaxForce / player.Mass);
        }

        /// <summary>
        ///     Move toward a specified position.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="desiredSpeed"></param>
        /// <returns></returns>
        public static Vector2 Seek(PointMass player, Vector2 target, float desiredSpeed)
        {
            Contract.Requires<ArgumentException>(player != null);
            return Vector2.Normalize(target - player.Position) * desiredSpeed - player.Velocity;
        }

        private static int IntervalComparison(float x, float lower, float upper) => x < lower ? (x > upper ? 1 : 0) : -1;

        /// <summary>
        ///     Move toward a target's future position. This method assumes the player will move at their maximum speed.  It also
        ///     ignores friction on the target. Ported from https://github.com/meshula/OpenSteer/blob/master/include/OpenSteer/SteerLibrary.h
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="maxPredictionTime"></param>
        /// <returns></returns>
        public static Vector2 Pursue(PointMass player, PointMass target, float maxPredictionTime)
        {
            var offset = target.Position - player.Position;
            var distance = offset.Length();
            var unitOffset = offset / distance;

            var parallelness = Vector2.Dot(Vector2.Normalize(player.Velocity), Vector2.Normalize(target.Velocity));
            if (float.IsNaN(parallelness)) parallelness = 0;
            var forwardness = Vector2.Dot(Vector2.Normalize(player.Velocity), unitOffset);
            if (float.IsNaN(forwardness)) forwardness = 0;

            var f = IntervalComparison(forwardness, -0.707f, 0.707f);
            var p = IntervalComparison(parallelness, -0.707f, 0.707f);
            var t = 0f;

            switch (f)
            {
                case 1:
                    switch (p)
                    {
                        case 1: t = 4; break;
                        case 0: t = 1.8f; break;
                        case -1: t = 0.85f; break;
                    }
                    break;

                case 0:
                    switch (p)
                    {
                        case 1: t = 1f; break;
                        case 0: t = 0.8f; break;
                        case -1: t = 4; break;
                    }
                    break;

                case -1:
                    switch (p)
                    {
                        case 1: t = 0.5f; break;
                        case 0: t = 2f; break;
                        case -1: f = 2; break;
                    }
                    break;
            }

            var directTravelTime = distance / player.Velocity.Length();
            float etl;

            if (float.IsInfinity(directTravelTime))
            {
                etl = maxPredictionTime;
            }
            else
            {
                var et = directTravelTime * t;
                etl = et > maxPredictionTime ? maxPredictionTime : et;
            }

            var targetPosition = target.Position + target.Velocity * etl;

            return Seek(player, targetPosition, player.MaxSpeed);
        }
    }
}

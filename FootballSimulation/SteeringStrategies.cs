using System;
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
        /// <param name="slowingRadius"></param>
        /// <returns></returns>
        public static Vector2 Arrive(PointMass player, Vector2 target, float maxSpeed, float slowingRadius)
        {
            var targetOffset = target - player.Position;
            var distance = targetOffset.Length();
            if (distance == 0)
            {
                return SlowDown(player);
            }
            var rampedSpeed = player.MaxSpeed * (distance / slowingRadius);
            var clippedSpeed = Math.Min(rampedSpeed, player.MaxSpeed);
            var desiredVelocity = clippedSpeed / distance * targetOffset;

            return desiredVelocity - player.Velocity;
        }

        /// <summary>
        ///     Move toward a specified position.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="desiredSpeed"></param>
        /// <returns></returns>
        public static Vector2 Seek(PointMass player, Vector2 target, float desiredSpeed)
            => Vector2.Normalize(target - player.Position) * desiredSpeed - player.Velocity;

        public static Vector2 SlowDown(PointMass player)
        {
            return player.Velocity * -1;
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="desiredSpeed"></param>
        /// <returns></returns>
        public static Vector2 SeekNormalized(PointMass player, Vector2 target, float desiredSpeed)
            => Vector2.Normalize(Seek(player, target, desiredSpeed));

        /// <summary>
        ///     Move away from a specified position.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="desiredSpeed"></param>
        /// <returns></returns>
        public static Vector2 Flee(PointMass player, Vector2 target, float desiredSpeed)
            => -1 * Seek(player, target, desiredSpeed);

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="desiredSpeed"></param>
        /// <returns></returns>
        public static Vector2 FleeNormalized(PointMass player, Vector2 target, float desiredSpeed)
            => Vector2.Normalize(Flee(player, target, desiredSpeed));

        /// <summary>
        ///     Move toward a target's future position. This method assumes the player will move at their maximum speed.  It also
        ///     ignores friction on the target.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Pursue(PointMass player, PointMass target)
        {
            // Calculate the time till intersection.
            // We want to move the perpendicular distance between the player and the target in the same amount of time to move the parallel distance to the estimated intersection point.
            // Formulas:
            //     t = perpDist / Vy
            //     Vx * t = initialParallelDist + Ball.ParallelVelocity * t         (ignores friction)
            //       => Vx * perpDist / Vy = initialParallelDist + Ball.ParallelVelocity * perpDist / Vy
            //       => Vx * perpDist = initialParallelDist * Vy + Ball.ParallelVelocity * perpDist
            //       => Vx = initialParallelDist * Vy / perpDist + Ball.ParallelVelocity
            //     player.MaxSpeed ^2 = Vx^2 + Vy^2
            //       => player.MaxSpeed ^2 = (initialParallelDist * Vy / perpDist + Ball.ParallelVelocity)^2 + Vy^2
            //       => lolno (keep these equations for future reference)

            //     We can solve for Vy/Vx together, this is equal to tan(theta)
            //     The above boils down to:
            //     (1/Ball.ParallelVelocity)^2  = (initialParallelDist/(perpDist*Ball.ParallelVelocity))^2 * t^2 - 2*initialParallelDist/(perpDist * Ball.ParallelVelocity^2) = 1/maxSpeed^2 + t^2/maxSpeed^2
            //              NOTE t is not time, it is Vy/Vx
            //        Just solve this for t
            //          https://www.wolframalpha.com/input/?i=t%5E2(d%5E2%2F(y%5E2*b%5E2)+-+1%2Fm%5E2)+-+2*d*t%2F(y*b%5E2)+%2B+1%2Fb%5E2+-+1%2Fm%5E2+%3D+0+solve+for+t

            //      => t = (sqrt(b^2 y^2 (y^2 (m^2-b^2)+d^2 m^2))+d m^2 y)/(d^2 m^2-b^2 y^2) if d^2 m^2!=b^2 y^2
            //      => t = (sqrt(b^2 y^2 (y^2 (m^2-b^2)+d^2 m^2))-d m^2 y)/(b^2 y^2-d^2 m^2) if d^2 m^2!=b^2 y^2
            //          NOTE t is not time, it is Vy/Vx
            // The final angle we want to travel towards is:
            // tan-1(t)

            double initial_Distance = Vector2.Distance(player.Position, target.Position);
            Vector2 initial_Vector = target.Position - player.Position;
            double cos_angle_Distance_xAxis = Vector2.Dot(initial_Vector, Vector2.UnitX)/initial_Distance;
            double angle_Distance_xAxis = Math.Acos(cos_angle_Distance_xAxis);
            double cos_angle_targetVector_xAxis = Vector2.Dot(target.Velocity, Vector2.UnitX) / target.Velocity.Length();
            double angle_targetVector_xAxis = Math.Acos(cos_angle_targetVector_xAxis);
            double angle = angle_Distance_xAxis - angle_targetVector_xAxis;
            double initial_Parellel_Distance = initial_Distance * Math.Cos(angle);
            double initial_Perpendicular_Distance = initial_Distance * Math.Sin(angle);
            double t = (initial_Parellel_Distance * Math.Pow((player.MaxSpeed), 2) * initial_Perpendicular_Distance + Math.Sqrt(Math.Pow(target.Velocity.Length(), 2) * Math.Pow(initial_Perpendicular_Distance, 2) * (Math.Pow(initial_Perpendicular_Distance, 2) * ((Math.Pow(player.MaxSpeed, 2)) - Math.Pow(target.Velocity.Length(), 2)) + Math.Pow(initial_Parellel_Distance, 2) * Math.Pow(player.MaxSpeed, 2)) / ((Math.Pow(initial_Parellel_Distance, 2)) * Math.Pow(player.MaxSpeed, 2) - Math.Pow(target.Velocity.Length(), 2) * Math.Pow(initial_Perpendicular_Distance, 2))));
            double pursue_Angle = Math.Atan(t);
            double angle_xAxis = pursue_Angle + angle_targetVector_xAxis;
            Vector2 pursue = new Vector2((float)Math.Cos(angle_xAxis), (float)Math.Sin(angle_xAxis)) * player.MaxSpeed;
            return pursue - player.Velocity; // This '- player.Velocity' is technically incorrect as the above assumes the player is at rest initially.  If major issues arise, remove this.
                                             // If things still don't work, redo the math.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 PursueNormalized(PointMass player, PointMass target)
            => Vector2.Normalize(Pursue(player, target));

        /// <summary>
        ///     Move away from a target's future position.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Evade(PointMass player, PointMass target)
            => -1 * Pursue(player, target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 EvadeNormalized(PointMass player, PointMass target)
            => Vector2.Normalize(Evade(player, target));
    }
}

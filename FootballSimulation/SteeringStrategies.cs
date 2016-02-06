using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    /// Steering Behaviors based on http://www.red3d.com/cwr/steer/gdc99/
    /// </summary>
    public static class SteeringStrategies
    {
        /// <summary>
        /// Move to and stop at a specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Arrive(Vector2 position, Vector2 target)
        {
            /*
                target_offset = target - position
                distance = length (target_offset)
                ramped_speed = max_speed * (distance / slowing_distance)
                clipped_speed = minimum (ramped_speed, max_speed)
                desired_velocity = (clipped_speed / distance) * target_offset
                steering = desired_velocity - velocity
            */

            return Vector2.Zero;
        }

        /// <summary>
        /// Move toward a specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Seek(PointMass player, Vector2 target, float desiredSpeed)
        {
            var desiredVelocity = Vector2.Normalize(player.Position - target).ClampMagnitude(desiredSpeed);
            return desiredVelocity - player.Velocity;
        }
        public static Vector2 SeekNormalized(PointMass player, Vector2 target, float desiredSpeed) => Vector2.Normalize(Seek(player, target, desiredSpeed));

        /// <summary>
        /// Move away from a specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Flee(PointMass player, Vector2 target, float desiredSpeed)
        {
            var desiredVelocity = Vector2.Normalize(target - player.Position).ClampMagnitude(desiredSpeed);
            return desiredVelocity - player.Velocity;
        }
        public static Vector2 FleeNormalized(PointMass player, Vector2 target, float desiredSpeed) => Vector2.Normalize(Flee(player, target, desiredSpeed));

        /// <summary>
        /// Move toward a target's future position.
        /// This method assumes the player will move at their maximum speed.  It also ignores friction on the target.
        /// </summary>
        /// <param name="position"></param>
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


        }

        /// <summary>
        /// Move away from a target's future position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Evade(PointMass player, PointMass target) => Vector2.Zero;
    }
}

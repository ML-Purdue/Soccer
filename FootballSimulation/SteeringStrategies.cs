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
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Pursue(PointMass player, PointMass target) => Vector2.Zero;

        /// <summary>
        /// Move away from a target's future position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 Evade(PointMass player, PointMass target) => Vector2.Zero;
    }
}

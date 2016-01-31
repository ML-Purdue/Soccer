using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    /// 
    /// </summary>
    public static class SteeringStrategies
    {
        /// <summary>
        /// 
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
    }
}

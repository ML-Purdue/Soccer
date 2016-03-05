using System.Drawing;
using System.Numerics;

namespace FootballSimulation
{
    internal static class CollisionMath
    {
        /// <summary>
        ///     Determines if a circle and rectangle intersect.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>Returns a <see cref="Vector2" /> representing the normal if a collision occurs; otherwise <c>null</c>.</returns>
        public static Vector2? GetCircleRectangleCollisionNormal(Vector2 center, float radius, RectangleF rectangle)
        {
            var contact = center;

            if (contact.X < rectangle.Left) contact.X = rectangle.Left;
            if (contact.X > rectangle.Right) contact.X = rectangle.Right;
            if (contact.Y < rectangle.Top) contact.Y = rectangle.Top;
            if (contact.Y > rectangle.Bottom) contact.Y = rectangle.Bottom;

            var v = new Vector2(contact.X - center.X, contact.Y - center.Y);
            var length = v.Length();

            return length > 0 && length < radius ? v/length : (Vector2?) null;
        }
    }
}
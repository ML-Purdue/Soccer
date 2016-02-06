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
        /// <returns>This method returns a <see cref="Collision" /> structue if a collision occurs; otherwise <c>null</c>.</returns>
        public static Collision? CircleRectangleCollide(Vector2 center, float radius, RectangleF rectangle)
        {
            var contact = center;

            if (contact.X < rectangle.Left) contact.X = rectangle.Left;
            if (contact.X > rectangle.Right) contact.X = rectangle.Right;
            if (contact.Y < rectangle.Top) contact.Y = rectangle.Top;
            if (contact.Y > rectangle.Bottom) contact.Y = rectangle.Bottom;

            var v = new Vector2(contact.X - center.X, contact.Y - center.Y);
            var length = v.Length();

            return length > 0 && length < radius
                ? new Collision(contact, v / length)
                : (Collision?) null;
        }

        /// <summary>
        ///     Container for collision information.
        /// </summary>
        public struct Collision
        {
            /// <summary>The point of contact for the collision.</summary>
            public Vector2 Contact { get; }

            /// <summary>A normal vector perpendicular to the collision plane.</summary>
            public Vector2 Normal { get; }

            /// <summary>
            ///     Initializes an instance of the <see cref="Collision" /> structure with the specified contact point and collision normal.
            /// </summary>
            /// <param name="contact">The point of contact for the collision.</param>
            /// <param name="normal">A normal vector perpendicular to the collision plane.</param>
            public Collision(Vector2 contact, Vector2 normal)
            {
                Contact = contact;
                Normal = normal;
            }
        }
    }
}
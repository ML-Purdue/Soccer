using System.Drawing;
using System.Numerics;

namespace FootballSimulation
{
    public static class RectangleFExtensions
    {
        /// <summary>
        ///     Determines if the specified point is contained within this <see cref="RectangleF" /> structure.
        /// </summary>
        /// <param name="rectangle">The <see cref="RectangleF" /> to test.</param>
        /// <param name="vector">The <see cref="Vector2" /> to test.</param>
        /// <returns>
        ///     This method returns <c>true</c> if the point is contained within this <see cref="RectangleF" /> structure;
        ///     otherwise <c>false</c>.
        /// </returns>
        public static bool Contains(this RectangleF rectangle, Vector2 vector) =>
            rectangle.Contains(vector.X, vector.Y);
    }
}
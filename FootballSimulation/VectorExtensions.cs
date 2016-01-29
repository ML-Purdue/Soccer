using System.Numerics;

namespace FootballSimulation
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Vector2" /> structure.
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        ///     Returns a copy of the <see cref="Vector2" /> with its magnitude clamped to <code>maxLength</code>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2" /> to clamp.</param>
        /// <param name="maxLength">The maximum of length of the returned <see cref="Vector2" />.</param>
        /// <returns>The clamped <see cref="Vector2" />.</returns>
        public static Vector2 ClampMagnitude(this Vector2 vector, float maxLength)
        {
            var length = vector.Length();
            return length > maxLength ? vector*maxLength/length : vector;
        }
    }
}
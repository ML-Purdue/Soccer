using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FootballSimulation
{
    internal static class EnumerableExtensions
    {
        /// <summary>Performs the specified action on each element of the <see cref="IEnumerable{T}" />.</summary>
        /// <param name="source">
        ///     The <see cref="IEnumerable{T}" /> on which the specified action will be performed.
        /// </param>
        /// <param name="action">
        ///     The <see cref="Action{T}" /> delegate to perform on each elements of the <see cref="IEnumerable{T}" />.
        /// </param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(action != null);

            foreach (var item in source)
                action(item);
        }
    }
}
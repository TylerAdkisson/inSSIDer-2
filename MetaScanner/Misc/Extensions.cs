using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inSSIDer.Misc
{
    public static class Extensions
    {
        /// <summary>
        /// Merges two sequences favoring elements in the second
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            // Remove diff elements from source
            var sourceMinus = source.Except(second, new GenericComparer<T>(comparer));

            // Merge
            var concat = source.Concat(second);

            return concat;
        }
    }
}

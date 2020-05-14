using System;
using System.Collections.Generic;

namespace Flogex.CeremonyGenerator
{
    internal static class EnumerableExtensions
    {
        public static bool Contains<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            foreach (var item in source)
                if (predicate(item)) return true;

            return false;
        }
    }
}

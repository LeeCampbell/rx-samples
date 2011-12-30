using System;
using System.Collections.Generic;

namespace RxSamples.WpfApplication.Examples.TwapChart
{
    public static class DeprecatedExtentions
    {
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            while (true)
            {
                foreach (var element in source)
                {
                    yield return element;
                }
            }
        }
    }
}
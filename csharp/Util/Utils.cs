using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Util
{
    public static class Utils
    {
        public static long ParseBinary(string input)
        {
            long value = 0;

            foreach (char c in input)
            {
                value <<= 1;

                if (c == '1')
                {
                    value |= 1;
                }
            }

            return value;
        }

        [Conditional("SAMPLE")]
        public static void TraceForSample(string message)
        {
            Console.WriteLine(message);
        }

        public static long Product<T>(this IEnumerable<T> source, Func<T, long> selector)
        {
            long product = 1;

            foreach (long val in source.Select(selector))
            {
                checked
                {
                    product *= val;
                }
            }

            return product;
        }

        public static T SafeIndex<T>(this List<T> list, int index, T defaultValue = default)
        {
            if (index < 0 || list is null || index >= list.Count)
            {
                return defaultValue;
            }

            return list[index];
        }
    }
}

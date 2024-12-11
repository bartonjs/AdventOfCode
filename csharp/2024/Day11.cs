using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day11
    {
        internal static void Problem1()
        {
            long ret = 0;
            Dictionary<(long, int), long> cache = new();

            foreach (string s in Data.Enumerate())
            {
                foreach (long datum in s.AsLongs(' '))
                {
                    ret += Count(datum, 25, cache);
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            Dictionary<(long, int), long> cache = new();

            foreach (string s in Data.Enumerate())
            {
                foreach (long datum in s.AsLongs(' '))
                {
                    ret += Count(datum, 75, cache);
                }
            }

            Console.WriteLine(ret);
        }

        private static long Count(long datum, int count, Dictionary<(long, int), long> cache)
        {
            if (count <= 0)
            {
                return 1;
            }

            (long Datum, int Count) key = (datum, count);

            if (cache.TryGetValue(key, out long cached))
            {
                return cached;
            }

            int countM1 = count - 1;

            if (datum == 0)
            {
                return cache[key] = Count(1, countM1, cache);
            }

            string val = datum.ToString();

            if (val.Length % 2 == 0)
            {
                long topHalf = long.Parse(val.AsSpan(0, val.Length / 2));
                long bottomHalf = long.Parse(val.AsSpan(val.Length / 2));

                return cache[key] = (Count(topHalf, countM1, cache) + Count(bottomHalf, countM1, cache));
            }

            return cache[key] = Count(datum * 2024, countM1, cache);
        }
    }
}
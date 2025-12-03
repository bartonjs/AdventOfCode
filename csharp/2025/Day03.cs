using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day03
    {
        internal static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                IEnumerable<char> chars = s;
                int max = -1;

                foreach (char c in chars.Distinct())
                {
                    int idx = s.IndexOf(c);

                    if (idx + 1 == s.Length)
                    {
                        continue;
                    }

                    char maxChar = chars.Skip(idx + 1).Max();

                    int loc = (c - '0') * 10 + (maxChar - '0');

                    if (loc > max)
                    {
                        max = loc;
                    }
                }

                ret += max;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                long joltage = Joltage(s, 0, 12, []);
                Utils.TraceForSample($"{s} => {joltage}");
                ret += joltage;
            }

            Console.WriteLine(ret);
        }

        internal static long Joltage(string s, int startIdx, int level, Dictionary<object, long> cache, long carryIn = 0)
        {
            int last = s.Length - level;
            long ret = 0;

            if (level == 0)
                return carryIn;

            object key = (startIdx, level, carryIn);

            if (cache.TryGetValue(key, out long prev))
                return prev;

            for (int i = startIdx; i <= last; i++)
            {
                long loc = s[i] - '0';
                loc *= (long)Math.Pow(10, level - 1);

                if (loc * 10 - 1 < ret)
                {
                    continue;
                }

                loc = Joltage(s, i + 1, level - 1, cache, loc);
                ret = long.Max(ret, loc);
            }

            ret += carryIn;
            cache[key] = ret;
            return ret;
        }
    }
}

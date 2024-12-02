using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day02
    {
        internal static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                List<long> values = s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();

                if (IsSafe(values))
                {
                    ret++;
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                List<long> allValues = s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                bool safe = IsSafe(allValues);

                if (!safe)
                {
                    for (int i = 0; i < allValues.Count; i++)
                    {
                        List<long> curValues = new List<long>(allValues);
                        curValues.RemoveAt(i);

                        if (IsSafe(curValues))
                        {
                            //Console.WriteLine($"Made {string.Join(",", allValues)} safe by removing index {i}: {string.Join(",", curValues)}");
                            safe = true;
                            break;
                        }
                    }
                }

                if (safe)
                {
                    ret++;
                }
                else
                {
                    //Console.WriteLine($"UNSAFE: {string.Join(",", allValues)}");
                }
            }

            Console.WriteLine(ret);
        }

        private static bool IsSafe(List<long> values)
        {
            long sign = long.Sign(values[1] - values[0]);

            // Only took 30 minutes to discover the need for this line...
            if (sign == 0)
            {
                return false;
            }

            for (int i = 1; i < values.Count; i++)
            {
                long prev = values[i - 1];
                long cur = values[i];
                long delta = cur - prev;

                if (long.Sign(delta) != sign || long.Abs(delta) > 3)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
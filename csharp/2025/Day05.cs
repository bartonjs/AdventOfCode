using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day05
    {
        private static (IntervalSet Intervals, List<long> Values) Load()
        {
            IntervalSet intervals = new();
            List<long> values = null;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrEmpty(s))
                {
                    values = new();
                    continue;
                }

                if (values is not null)
                {
                    values.Add(long.Parse(s));
                }
                else
                {
                    int hyphen = s.IndexOf('-');
                    intervals.AddInterval(long.Parse(s.AsSpan(0, hyphen)), long.Parse(s.AsSpan(hyphen + 1)));
                }
            }

            return (intervals, values);
        }

        internal static void Problem1()
        {
            long ret = 0;
            (IntervalSet intervals, List<long> values) = Load();

            foreach (long value in values)
            {
                if (intervals.Contains(value))
                {
                    ret++;
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            (IntervalSet ranges, var values) = Load();

            foreach (var range in ranges)
            {
                ret += range.WidthInclusive;
            }

            Console.WriteLine(ret);
        }
    }
}

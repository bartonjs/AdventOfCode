using System;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day07
    {
        internal static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                int colon = s.IndexOf(':');
                long target = long.Parse(s.AsSpan(0, colon));
                long[] values = s.Substring(colon + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse).ToArray();

                if (CanMakeTarget(target, values))
                {
                    ret += target;
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                int colon = s.IndexOf(':');
                long target = long.Parse(s.AsSpan(0, colon));
                long[] values = s.Substring(colon + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse).ToArray();

                if (CanMakeTarget2(target, values))
                {
                    Utils.TraceForSample(s);
                    ret += target;
                }
            }

            Console.WriteLine(ret);
        }

        private static bool CanMakeTarget(long target, long[] values)
        {
            return CanMakeTarget(target, values[0], values, 1);
        }

        private static bool CanMakeTarget(long target, long part, long[] values, int skip)
        {
            if (part > target)
            {
                return false;
            }

            if (skip == values.Length)
            {
                return target == part;
            }

            if (CanMakeTarget(target, part + values[skip], values, skip + 1))
            {
                return true;
            }

            return CanMakeTarget(target, part * values[skip], values, skip + 1);
        }

        private static bool CanMakeTarget2(long target, long[] values)
        {
            return CanMakeTarget2(target, values[0], values, 1);
        }

        private static bool CanMakeTarget2(long target, long part, long[] values, int skip)
        {
            if (part > target)
            {
                return false;
            }

            if (skip == values.Length)
            {
                return target == part;
            }

            long next = values[skip];

            if (CanMakeTarget2(target, part + next, values, skip + 1))
            {
                return true;
            }

            if (CanMakeTarget2(target, part * next, values, skip + 1))
            {
                return true;
            }

            return CanMakeTarget2(target, long.Parse(part.ToString() + next), values, skip + 1);
        }
    }
}
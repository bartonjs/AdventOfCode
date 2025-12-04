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
                long joltage = Joltage(s, 0, 12);
                Utils.TraceForSample($"{s} => {joltage}");
                ret += joltage;
            }

            Console.WriteLine(ret);
        }

        internal static long Joltage(string s, int startIdx, int level)
        {
            if (level <= 0)
            {
                return 0;
            }

            int levelM1 = level - 1;
            int endIdx = s.Length - startIdx - levelM1;
            (char val, int idx) = s.Skip(startIdx).Take(endIdx).MaxWithIndex();
            idx += startIdx;

            long hereValue = (val - '0') * Utils.Pow10(levelM1);

            return hereValue + Joltage(s, idx + 1, levelM1);
        }
    }
}

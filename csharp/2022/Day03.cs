using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal class Day03
    {
        private static int Score(char c)
        {
            if (c < 'a')
            {
                return (c - 'A') + 27;
            }

            return (c - 'a') + 1;
        }

        internal static void Problem1()
        {
            int score = 0;

            foreach (string s in Data.Enumerate())
            {
                ReadOnlySpan<char> part2 = s.AsSpan(s.Length / 2);

                foreach (char c in s.AsSpan(0, part2.Length))
                {
                    if (part2.IndexOf(c) >= 0)
                    {
                        score += Score(c);
                        break;
                    }
                }
            }

            Console.WriteLine(score);
        }

        internal static void Problem2()
        {
            int score = 0;
            List<string> allData = new List<string>(Data.Enumerate());

            for (int i = 0; i < allData.Count; i += 3)
            {
                ReadOnlySpan<char> one = allData[i];
                ReadOnlySpan<char> two = allData[i + 1];
                ReadOnlySpan<char> three = allData[i + 2];

                foreach (char c in one)
                {
                    if (two.IndexOf(c) >= 0 && three.IndexOf(c) >= 0)
                    {
                        score += Score(c);
                        break;
                    }
                }
            }

            Console.WriteLine(score);
        }
    }
}
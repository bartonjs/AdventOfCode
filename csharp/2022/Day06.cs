using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal class Day06
    {
        private static int FindUniqueTrail(string s, int count)
        {
            HashSet<char> set = new HashSet<char>();

            int stop = s.Length - 1;

            for (int i = count; i < stop; i++)
            {
                for (int j = i - count; j < i; j++)
                {
                    set.Add(s[j]);
                }

                if (set.Count == count)
                {
                    return i;
                }

                set.Clear();
            }

            throw new KeyNotFoundException();
        }

        internal static void Problem1()
        {
            foreach (string s in Data.Enumerate())
            {
                Console.WriteLine(FindUniqueTrail(s, 4));
            }
        }

        internal static void Problem2()
        {
            foreach (string s in Data.Enumerate())
            {
                Console.WriteLine(FindUniqueTrail(s, 14));
            }
        }
    }
}
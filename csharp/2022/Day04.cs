using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal class Day04
    {
        internal static IEnumerable<(int Low1, int High1, int Low2, int High2)> EnumerateRanges()
        {
            foreach (string str in Data.Enumerate())
            {
                ReadOnlySpan<char> s = str;
                int h1 = s.IndexOf('-');
                int c = s.IndexOf(',');
                int h2 = s.Slice(c).IndexOf('-') + c;

                int low1 = int.Parse(s[ 0..h1 ]);
                int high1 = int.Parse(s[ (h1 + 1)..c ]);
                int low2 = int.Parse(s[ (c + 1)..h2 ]);
                int high2 = int.Parse(s[ (h2 + 1).. ]);

                yield return (low1, high1, low2, high2);
            }
        }

        internal static void Problem1()
        {
            int score = 0;

            foreach ((int lo1, int hi1, int lo2, int hi2) in EnumerateRanges())
            {
                if (lo1 >= lo2 && hi1 <= hi2)
                {
                    //Console.WriteLine($"[{lo1},{hi1}] is contained in [{lo2}, {hi2}]");
                    score++;
                }
                else if (lo2 >= lo1 && hi2 <= hi1)
                {
                    //Console.WriteLine($"[{lo1},{hi1}] contains [{lo2}, {hi2}]");
                    score++;
                }
            }

            Console.WriteLine(score);
        }

        internal static void Problem2()
        {
            int score = 0;

            foreach ((int lo1, int hi1, int lo2, int hi2) in EnumerateRanges())
            {
                if (lo1 <= hi2 && hi1 >= lo2)
                {
                    //Console.WriteLine($"[{lo1},{hi1}] overlaps1 [{lo2}, {hi2}]");
                    score++;
                }
                else if (lo2 <= hi1 && hi2 >= lo1)
                {
                    //Console.WriteLine($"[{lo1},{hi1}] overlaps2 [{lo2}, {hi2}]");
                    score++;

                }
            }

            Console.WriteLine(score);
        }
    }
}
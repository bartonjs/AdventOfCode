using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day5
    {
        internal static void Problem1()
        {
            RunProblem(false);
        }

        internal static void Problem2()
        {
            RunProblem(true);
        }

        private static void RunProblem(bool withDiagonals)
        {
            int[][] floor = new int[1000][];

            for (int i = 0; i < floor.Length; i++)
            {
                floor[i] = new int[1000];
            }

            foreach ((int x1, int y1, int x2, int y2) in EnumerateData())
            {
                if (x1 == x2)
                {
                    int min = Math.Min(y1, y2);
                    int max = Math.Max(y1, y2);

                    for (int y = min; y <= max; y++)
                    {
                        floor[x1][y]++;
                    }
                }
                else if (y1 == y2)
                {
                    int min = Math.Min(x1, x2);
                    int max = Math.Max(x1, x2);

                    for (int x = min; x <= max; x++)
                    {
                        floor[x][y1]++;
                    }
                }
                else if (withDiagonals)
                {
                    int stepY = y2 > y1 ? 1 : -1;
                    int y = y1;

                    if (x2 > x1)
                    {

                        for (int x = x1; x <= x2; x++, y += stepY)
                        {
                            floor[x][y]++;
                        }
                    }
                    else
                    {
                        for (int x = x1; x >= x2; x--, y += stepY)
                        {
                            floor[x][y]++;
                        }
                    }
                }
            }

            int peakMax = floor.SelectMany(x => x).Max();
            int peakCount = floor.SelectMany(x => x).Count(x => x == peakMax);
            int crosses = floor.SelectMany(x => x).Count(x => x > 1);

            Console.WriteLine($"The tallest peak is {peakMax}, there are {peakCount} of them.");
            Console.WriteLine($"There are {crosses} crosses.");
        }

        internal static IEnumerable<(int X1, int Y1, int X2, int Y2)> EnumerateData()
        {
            foreach (string line in Data.Enumerate())
            {
                ReadOnlySpan<char> rem = line;
                int comma = rem.IndexOf(',');
                int x1 = int.Parse(rem.Slice(0, comma));

                rem = rem.Slice(comma + 1);
                int space = rem.IndexOf(' ');
                int y1 = int.Parse(rem.Slice(0, space));

                rem = rem.Slice(space + 4);
                comma = rem.IndexOf(',');
                int x2 = int.Parse(rem.Slice(0, comma));
                int y2 = int.Parse(rem.Slice(comma + 1));

                yield return (x1, y1, x2, y2);
            }
        }
    }
}
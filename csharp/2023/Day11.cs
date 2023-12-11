using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day11
    {
        internal static void Problem1()
        {
            MetaProblem(1);
        }

        internal static void Problem2()
        {
            MetaProblem(1_000_000 - 1);
        }

        private static void MetaProblem(long expansion)
        {
            long ret = 0;
            HashSet<int> emptyCols = null;

            int origWidth = -1;
            List<LongPoint> galaxies = new List<LongPoint>();
            long y = 0;

            foreach (string s in Data.Enumerate())
            {
                int galaxyLoc = s.IndexOf('#');

                if (emptyCols is null)
                {
                    emptyCols = new HashSet<int>(Enumerable.Range(0, s.Length));
                    origWidth = s.Length;
                }

                if (galaxyLoc == -1)
                {
                    y += expansion;
                }
                else
                {
                    while (galaxyLoc != -1)
                    {
                        emptyCols.Remove(galaxyLoc);
                        galaxies.Add(new LongPoint(galaxyLoc, y));

                        galaxyLoc = s.IndexOf('#', galaxyLoc + 1);
                    }
                }

                y++;
            }

            for (int i = origWidth - 1; i >= 0; i--)
            {
                if (emptyCols.Contains(i))
                {
                    for (int galaxyId = 0; galaxyId < galaxies.Count; galaxyId++)
                    {
                        LongPoint point = galaxies[galaxyId];

                        if (point.X >= i)
                        {
                            galaxies[galaxyId] = new LongPoint(point.X + expansion, point.Y);
                        }
                    }
                }
            }

            for (int fromId = 0; fromId < galaxies.Count; fromId++)
            {
                LongPoint fromPoint = galaxies[fromId];

                for (int toId = fromId + 1; toId < galaxies.Count; toId++)
                {
                    LongPoint toPoint = galaxies[toId];

                    ret += fromPoint.ManhattanDistance(toPoint);
                }
            }

            Console.WriteLine(ret);
        }
    }
}
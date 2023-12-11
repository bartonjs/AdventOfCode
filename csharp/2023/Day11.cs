using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day11
    {
        internal static void Problem1()
        {
            long ret = 0;
            HashSet<int> emptyCols = null;

            int origWidth = -1;
            List<Point> galaxies = new List<Point>();
            int y = 0;

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
                    y++;
                }
                else
                {
                    while (galaxyLoc != -1)
                    {
                        emptyCols.Remove(galaxyLoc);
                        galaxies.Add(new Point(galaxyLoc, y));

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
                        if (galaxies[galaxyId].X >= i)
                        {
                            galaxies[galaxyId] = galaxies[galaxyId].East();
                        }
                    }
                }
            }

            for (int fromId = 0; fromId < galaxies.Count; fromId++)
            {
                Point fromPoint = galaxies[fromId];

                for (int toId = fromId + 1; toId < galaxies.Count; toId++)
                {
                    Point toPoint = galaxies[toId];

                    ret += fromPoint.ManhattanDistance(toPoint);
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            HashSet<int> emptyCols = null;

            const long Expansion = 999_999;

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
                    y += Expansion;
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
                            galaxies[galaxyId] = new LongPoint(point.X + Expansion, point.Y);
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
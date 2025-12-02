using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day10
    {
        const Directions2D NEWS = Directions2D.North | Directions2D.East | Directions2D.South |
                                  Directions2D.West;

        private static (DynamicPlane<int>, List<Point>) Load()
        {
            DynamicPlane<int> world = null;
            List<Point> starts = new();
            int rowIdx = 0;

            foreach (string s in Data.Enumerate())
            {
                int[] row = s.Select(c => c - '0').ToArray();

                if (world is null)
                {
                    world = new DynamicPlane<int>(row);
                }
                else
                {
                    world.PushY(row);
                }

                for (int col = 0; col < row.Length; col++)
                {
                    if (row[col] == 0)
                    {
                        starts.Add(new Point(col, rowIdx));
                    }
                }

                rowIdx++;
            }

            return (world, starts);
        }

        internal static void Problem1()
        {
            HashSet<Point> reached = new();
            long ret = 0;

            (DynamicPlane<int> world, List<Point> trailheads) = Load();

            foreach (Point p in trailheads)
            {
                Discover(p, world, reached);
                ret += reached.Count;
                reached.Clear();
            }

            Console.WriteLine(ret);

            static void Discover(Point p, DynamicPlane<int> world, HashSet<Point> reached)
            {
                int curP1 = world[p] + 1;

                if (curP1 > 9)
                {
                    reached.Add(p);
                    return;
                }

                foreach (Point neighbor in p.GetNeighbors(NEWS))
                {
                    if (world.TryGetValue(neighbor, out int val) && val == curP1)
                    {
                        Discover(neighbor, world, reached);
                    }
                }
            }
        }

        internal static void Problem2()
        {
            long ret = 0;

            (DynamicPlane<int> world, List<Point> trailheads) = Load();

            foreach (Point p in trailheads)
            {
                ret += Score(p, world);
            }

            Console.WriteLine(ret);

            static long Score(Point p, DynamicPlane<int> world)
            {
                int curP1 = world[p] + 1;
                long ret = 0;

                if (curP1 > 9)
                {
                    return 1;
                }

                foreach (Point neighbor in p.GetNeighbors(NEWS))
                {
                    if (world.TryGetValue(neighbor, out int val) && val == curP1)
                    {
                        ret += Score(neighbor, world);
                    }
                }

                return ret;
            }
        }
    }
}
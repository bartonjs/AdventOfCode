using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day18
    {
#if SAMPLE
        private const int WorldSize = 7;
#else
        private const int WorldSize = 71;
#endif

        private static IEnumerable<Point> Load()
        {
            foreach (string s in Data.Enumerate())
            {
                string[] arr = s.Split(',');
                yield return new Point(int.Parse(arr[0]), int.Parse(arr[1]));
            }
        }

        internal static void Problem1()
        {
            Console.WriteLine(
                Cost(
                    Load(),
#if SAMPLE
                    12));
#else
                    1024));
#endif
        }

        internal static void Problem2()
        {
            List<Point> allBlocks = new List<Point>(Load());

#if SAMPLE
            int low = 12;
#else
            int low = 1024;
#endif
            int high = allBlocks.Count;

            while (high > low)
            {
                int mid = (high - low) / 2 + low;
                int cost = Cost(allBlocks, mid);

                if (cost == int.MaxValue)
                {
                    Utils.TraceForSample($"Failed at {mid}, reducing high");
                    high = int.Max(low, mid - 1);
                }
                else
                {
                    Utils.TraceForSample($"Succeeded at {mid}, raising low");
                    low = int.Min(high, mid + 1);
                }
            }

            int target = low + 1;

            while (Cost(allBlocks, target) == int.MaxValue)
            {
                target--;
            }

            while (Cost(allBlocks, target) != int.MaxValue)
            {
                target++;
            }

            Utils.TraceForSample($"Choosing point at {target} as lowest failure.");
            Utils.TraceForSample($"Target: {Cost(allBlocks, target)}");
            Utils.TraceForSample($"Target - 1: {Cost(allBlocks, target-1)}");

            Point whatFell = allBlocks[target - 1];
            Console.WriteLine($"{whatFell.X},{whatFell.Y}");
        }

        private static int Cost(IEnumerable<Point> points, int take)
        {
            FixedPlane<bool> plane = new FixedPlane<bool>(WorldSize, WorldSize);

            foreach (Point p in points.Take(take))
            {
                plane[p] = true;
            }

            return Pathing.AStar(
                plane,
                new Point(0, 0),
                new Point(WorldSize - 1, WorldSize - 1),
                static (from, world) => from.GetNeighbors(Point.AllDirections).Where(p => world.TryGetValue(p, out bool blocked) && !blocked).Select(p => (p, 1)),
                static (candidate, end, world) => candidate.ManhattanDistance(end));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day18
    {
#if SAMPLE
        private const int WorldSize = 7;
        private const int Part1Take = 12;
#else
        private const int WorldSize = 71;
        private const int Part1Take = 1024;
#endif

        private static FixedPlane<int> Load()
        {
            return Load(out _, out _);
        }

        private static FixedPlane<int> Load(out int count, out List<string> allBlocks)
        {
            FixedPlane<int> plane = new FixedPlane<int>(WorldSize, WorldSize);
            plane.Fill(int.MaxValue);
            List<string> blocks = new();
            int i = 1;

            foreach (string s in Data.Enumerate())
            {
                blocks.Add(s);
                Point point = Point.ParsePair(s);
                Debug.Assert(plane[point] == int.MaxValue);
                plane[point] = i;
                i++;
            }

            count = i;
            allBlocks = blocks;
            return plane;
        }

        internal static void Problem1()
        {
            Console.WriteLine(Cost(Load(), Part1Take));
        }

        internal static void Problem2()
        {
            int low = Part1Take;
            FixedPlane<int> plane = Load(out int high, out List<string> allBlocks);

            while (high >= low)
            {
                int mid = (high - low) / 2 + low;
                int cost = Cost(plane, mid);

                if (cost == int.MaxValue)
                {
                    Utils.TraceForSample($"Failed at {mid}, reducing high");
                    high = mid - 1;
                }
                else
                {
                    Utils.TraceForSample($"Succeeded at {mid}, raising low");
                    low = mid + 1;
                }
            }

            Utils.TraceForSample($"L={low}, H={high}");
            int minFail = low;
            int index = minFail - 1;

            Utils.TraceForSample($"Choosing point at {minFail} as lowest failure.");
            Utils.TraceForSample($"Target: {Cost(plane, minFail)}");
            Utils.TraceForSample($"Target - 1: {Cost(plane, index)}");

            Console.WriteLine(allBlocks[index]);
        }

        private static int Cost(FixedPlane<int> plane, int take)
        {
            return Pathing.AStar(
                (plane, take),
                new Point(0, 0),
                new Point(WorldSize - 1, WorldSize - 1),
                static (from, world) =>
                    from.GetNeighbors(Point.AllDirections)
                        .Where(p => world.plane.TryGetValue(p, out int value) && value > world.take).Select(p => (p, 1)),
                static (candidate, end, world) => candidate.ManhattanDistance(end));
        }
    }
}
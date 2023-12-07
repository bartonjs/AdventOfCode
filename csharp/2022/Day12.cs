using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day12
    {
        private static (Plane<int>, Point, Point) LoadData()
        {
            DynamicPlane<int> plane = null;
            Point start = default;
            Point end = default;

            foreach (string line in Data.Enumerate())
            {
                int startIdx = line.IndexOf('S');
                int endIdx = line.IndexOf('E');
                int[] row = line.Select(c => c - 'a').ToArray();

                if (startIdx >= 0)
                {
                    row[startIdx] = 0;
                    start = new Point(startIdx, plane?.Height ?? 0);
                }

                if (endIdx >= 0)
                {
                    row[endIdx] = 25;
                    end = new Point(endIdx, plane?.Height ?? 0);
                }

                if (plane is null)
                {
                    plane = new DynamicPlane<int>(row);
                }
                else
                {
                    plane.PushY(row);
                }
            }

            return (plane, start, end);
        }

        [GeneratedRegex(@"^(.) (\d+)")]
        private static partial Regex Regex();

        internal static void Problem1()
        {
            (Plane<int> grid, Point start, Point end) = LoadData();

            Console.WriteLine(FindCheapestPath(grid, start, end));
        }

        internal static void Problem2()
        {
            (Plane<int> grid, Point start, Point end) = LoadData();
            int lowest = int.MaxValue;

            foreach (Point p in grid.AllPoints())
            {
                if (grid[p] == 0)
                {
                    lowest = Math.Min(lowest, FindCheapestPath(grid, p, end));
                }
            }

            Console.WriteLine(lowest);
        }

        private static int FindCheapestPath(Plane<int> grid, Point start, Point end)
        {
            return Pathing.AStar(
                grid,
                start,
                end,
                Neighbors,
                (candidate, end, world) => candidate.ManhattanDistance(end));

            static IEnumerable<(Point Neighbor, int Cost)> Neighbors(Point from, Plane<int> world)
            {
                int curHeight = world[from];
                Point plusX = new Point(from.X + 1, from.Y);
                Point minusX = new Point(from.X - 1, from.Y);
                Point plusY = new Point(from.X, from.Y + 1);
                Point minusY = new Point(from.X, from.Y - 1);
                int testHeight;

                if (world.TryGetValue(plusX, out testHeight) && testHeight - curHeight <= 1)
                {
                    yield return (plusX, 1);
                }

                if (world.TryGetValue(plusY, out testHeight) && testHeight - curHeight <= 1)
                {
                    yield return (plusY, 1);
                }

                if (world.TryGetValue(minusX, out testHeight) && testHeight - curHeight <= 1)
                {
                    yield return (minusX, 1);
                }

                if (world.TryGetValue(minusY, out testHeight) && testHeight - curHeight <= 1)
                {
                    yield return (minusY, 1);
                }
            }
        }
    }
}
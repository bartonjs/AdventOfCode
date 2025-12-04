using System;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day04
    {
        private static DynamicPlane<bool> Load()
        {
            DynamicPlane<bool> plane = null;

            foreach (string line in Data.Enumerate())
            {
                bool[] row = line.Select(c => c == '@').ToArray();

                if (plane == null)
                {
                    plane = new DynamicPlane<bool>(row);
                }
                else
                {
                    plane.PushY(row);
                }
            }

            return plane;
        }

        internal static void Problem1()
        {
            long ret = 0;

            DynamicPlane<bool> plane = Load();

            for (int x = 0; x < plane.Width; x++)
            {
                for (int y = 0; y < plane.Height; y++)
                {
                    Point point = new Point(x, y);

                    if (IsAccessible(plane, point))
                    {
                        ret++;
                    }
                }
            }

            Console.WriteLine(ret);
        }

        private static bool IsAccessible(DynamicPlane<bool> plane, Point point)
        {
            bool hasPaper;

            if (plane.TryGetValue(point, out hasPaper) && hasPaper)
            {
                int neighbors = 0;

                foreach (Point neighbor in point.Get8Neighbors())
                {
                    if (plane.TryGetValue(neighbor, out hasPaper) && hasPaper)
                    {
                        neighbors++;
                    }
                }

                if (neighbors < 4)
                {
                    return true;
                }
            }

            return false;
        }

        internal static void Problem2()
        {
            long ret = 0;
            DynamicPlane<bool> plane = Load();
            bool modified = true;

            while (modified)
            {
                modified = false;

                for (int x = 0; x < plane.Width; x++)
                {
                    for (int y = 0; y < plane.Height; y++)
                    {
                        Point point = new Point(x, y);

                        if (IsAccessible(plane, point))
                        {
                            ret++;
                            plane[point] = false;
                            modified = true;
                        }
                    }
                }
            }

            Console.WriteLine(ret);
        }
    }
}

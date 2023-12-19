using System;
using System.Diagnostics;
using System.Globalization;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day18
    {
        internal static void Problem1()
        {
            Console.WriteLine(MetaProblem(Parse));

            static (char Direction, long Value) Parse(string[] parts)
            {
                return (parts[0][0], long.Parse(parts[1]));
            }
        }

        internal static void Problem2()
        {
            Console.WriteLine(MetaProblem(Parse));

            static (char Direction, long Value) Parse(string[] parts)
            {
                string part = parts[2];

                char direction = part[7] switch
                {
                    '0' => 'R',
                    '1' => 'D',
                    '2' => 'L',
                    '3' => 'U',
                };

                long value = long.Parse(part.AsSpan(2, 5), NumberStyles.HexNumber);

                return (direction, value);
            }
        }

        private static long MetaProblem(Func<string[], (char Direction, long Value)> selector)
        {
            long curY = 0;

            long perimeter = 0;
            // https://www.mathsisfun.com/geometry/area-irregular-polygons.html
            long mathIsFunArea = 0;

            // Vertical lines don't contribute to area, they just change the amount of area from
            // the next horizontal line.

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                (char part, long value) = selector(parts);

                if (part == 'R')
                {
                    mathIsFunArea -= value * curY;
                }
                else if (part == 'L')
                {
                    mathIsFunArea += value * curY;
                }
                else if (part == 'U')
                {
                    curY -= value;
                }
                else
                {
                    Debug.Assert(part == 'D');
                    curY += value;
                }

                perimeter += value;
            }

            long area = long.Abs(mathIsFunArea);

            // Each of the coordinates represents the middle of a #,
            // so half the area is included in the calculation, but half isn't.
            area += perimeter / 2;

            // 4 of the corners will be missing one quarter of their area from the stroke-width
            // catchup.  4 * 1/4 == 1.
            area++;

            return area;
        }
    }
}
using System;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day13
    {
        internal static void Problem1()
        {
            long ret = 0;
            DynamicPlane<char> plane = null;
            long row, col;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrEmpty(s))
                {
                    if (plane is not null)
                    {
                        (row, col) = FindReflection(plane);
                        plane = null;

                        ret += col;
                        ret += 100 * row;

                        Console.WriteLine($"Reflection at Col={col}, Row={row}");
                    }

                    continue;
                }

                if (plane is null)
                {
                    plane = new DynamicPlane<char>(s.ToCharArray());
                }
                else
                {
                    plane.PushY(s.ToCharArray());
                }
            }

            (row, col) = FindReflection(plane);
            plane = null;

            ret += col;
            ret += 100 * row;
            Console.WriteLine($"Reflection at Col={col}, Row={row}");

            Console.WriteLine(ret);
        }

        private static (long Row, long Col) FindReflection(DynamicPlane<char> plane, long exceptRow = -1, long exceptCol = -1)
        {
            int colMax = plane.Width;
            int rowMax = plane.Height;

            for (int col = 1; col < colMax; col++)
            {
                if (col != exceptCol && CheckColReflection(plane, col))
                {
                    return (0, col);
                }
            }

            for (int row = 1; row < rowMax; row++)
            {
                if (row != exceptRow && CheckRowReflection(plane, row))
                {
                    return (row, 0);
                }
            }

            if (exceptCol < 0 && exceptRow < 0)
            {
                throw new InvalidOperationException();
            }
            
            return (0, 0);

            static bool CheckColReflection(DynamicPlane<char> plane, int col)
            {
                int left, right;

                for (left = col - 1, right = col; left >= 0 && right < plane.Width; left--, right++)
                {
                    for (int row = 0; row < plane.Height; row++)
                    {
                        if (plane[new Point(left, row)] != plane[new Point(right, row)])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            static bool CheckRowReflection(DynamicPlane<char> plane, int row)
            {
                int up, down;

                for (up = row - 1, down = row; up >= 0 && down < plane.Height; up--, down++)
                {
                    for (int col = 0; col < plane.Width; col++)
                    {
                        if (plane[new Point(col, up)] != plane[new Point(col, down)])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        private static (long Row, long Col) Smudge(DynamicPlane<char> plane, long exceptRow, long exceptCol)
        {
            foreach (Point point in plane.AllPoints())
            {
                char orig = plane[point];
                char alt = orig == '#' ? '.' : '#';

                plane[point] = alt;

                var ret = FindReflection(plane, exceptRow, exceptCol);

                if (ret.Row != 0 || ret.Col != 0)
                {
                    Utils.TraceForSample($"Found smudge at {point}");
                    return ret;
                }

                plane[point] = orig;
            }

            throw new InvalidOperationException();
        }

        internal static void Problem2()
        {
            long ret = 0;
            DynamicPlane<char> plane = null;
            long row, col;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrEmpty(s))
                {
                    if (plane is not null)
                    {
                        (row, col) = FindReflection(plane);
                        (row, col) = Smudge(plane, row, col);

                        plane = null;

                        ret += col;
                        ret += 100 * row;

                        Console.WriteLine($"Reflection at Col={col}, Row={row}");
                    }

                    continue;
                }

                if (plane is null)
                {
                    plane = new DynamicPlane<char>(s.ToCharArray());
                }
                else
                {
                    plane.PushY(s.ToCharArray());
                }
            }

            (row, col) = FindReflection(plane);
            (row, col) = Smudge(plane, row, col);
            plane = null;

            ret += col;
            ret += 100 * row;
            Console.WriteLine($"Reflection at Col={col}, Row={row}");

            Console.WriteLine(ret);
        }
    }
}
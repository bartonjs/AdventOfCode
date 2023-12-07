using System;
using System.IO;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day14
    {
        private const string Numbers = "0123456789";

        private static (byte[,], int maxY) LoadGrid()
        {
            byte[,] grid = new byte[1000, 500];
            int maxY = 0;

            foreach (string line in Data.Enumerate())
            {
                int x = 0;
                int y = 0;
                bool first = true;

                ReadOnlySpan<char> s = line;

                while (true)
                {
                    int stopIdx = s.IndexOfAnyExcept(Numbers);
                    int parseX = int.Parse(s.Slice(0, stopIdx));

                    s = s.Slice(stopIdx + 1);
                    stopIdx = s.IndexOfAnyExcept(Numbers);

                    if (stopIdx < 0)
                    {
                        stopIdx = s.Length;
                    }

                    int parseY = int.Parse(s.Slice(0, stopIdx));
                    s = s.Slice(stopIdx);

                    if (!first)
                    {
                        if (x == parseX)
                        {
                            if (y > parseY)
                            {
                                for (; y >= parseY; y--)
                                {
                                    grid[x, y] = 1;
                                }
                            }
                            else
                            {
                                for (; y <= parseY; y++)
                                {
                                    grid[x, y] = 1;
                                }
                            }
                        }
                        else if (x > parseX)
                        {
                            for (; x >= parseX; x--)
                            {
                                grid[x, y] = 1;
                            }
                        }
                        else
                        {
                            for (; x <= parseX; x++)
                            {
                                grid[x, y] = 1;
                            }
                        }
                    }
                    else
                    {
                        first = false;
                    }

                    x = parseX;
                    y = parseY;
                    maxY = Math.Max(y, maxY);

                    if (s.IsEmpty)
                    {
                        break;
                    }

                    stopIdx = s.IndexOfAny(Numbers);
                    s = s.Slice(stopIdx);
                }
            }

            return (grid, maxY);
        }

        private static int FillSand(byte[,] grid, int fillX, int fillY, int maxY)
        {
            int sandAdded = 0;

            while (true)
            {
                int sandX = fillX;
                int sandY = fillY;

                while (true)
                {
                    if (grid[sandX, sandY + 1] == 0)
                    {
                        sandY++;
                    }
                    else if (grid[sandX - 1, sandY + 1] == 0)
                    {
                        sandX--;
                        sandY++;
                    }
                    else if (grid[sandX + 1, sandY + 1] == 0)
                    {
                        sandX++;
                        sandY++;
                    }
                    else
                    {
                        grid[sandX, sandY] = 2;
                        sandAdded++;

                        if (grid[fillX, fillY] != 0)
                        {
                            return sandAdded;
                        }

                        break;
                    }

                    if (sandY > maxY)
                    {
                        return sandAdded;
                    }
                }
            }
        }

        internal static void Problem1()
        {
            (byte[,] grid, int maxY) = LoadGrid();

            int added = FillSand(grid, 500, 0, maxY);
            Console.WriteLine(added);
        }

        internal static void Problem2()
        {
            (byte[,] grid, int maxY) = LoadGrid();

            int maxX = grid.GetLength(0);

            for (int x = maxX - 1; x >= 0; x--)
            {
                grid[x, maxY + 2] = 1;
            }

            int added = FillSand(grid, 500, 0, maxY + 2);
            Console.WriteLine(added);

            int maxUsedX = int.MinValue;
            int minUsedX = int.MaxValue;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = maxY + 1; y >= 0; y--)
                {
                    if (grid[x, y] != 0)
                    {
                        minUsedX = x;
                        break;
                    }
                }

                if (minUsedX < int.MaxValue)
                {
                    break;
                }
            }

            for (int x = maxX - 1; x >= 0; x--)
            {
                for (int y = maxY + 1; y >= 0; y--)
                {
                    if (grid[x, y] != 0)
                    {
                        maxUsedX = x;
                        break;
                    }
                }

                if (maxUsedX > int.MinValue)
                {
                    break;
                }
            }

            using (FileStream fs = File.OpenWrite("ascii.txt"))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine($"Sand X-coordinates range from {minUsedX} to {maxUsedX}");
                int stopX = maxUsedX + 4;

                for (int y = 0; y <= maxY + 2; y++)
                {
                    for (int x = minUsedX - 3; x < stopX; x++)
                    {
                        writer.Write(grid[x, y] switch { 0 => '.', 1 => '#', 2 => 'o' });
                    }

                    writer.WriteLine();
                }
            }
        }
    }
}
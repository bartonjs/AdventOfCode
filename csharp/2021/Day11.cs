using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day11
    {
        internal static void Problem1()
        {
            int[][] grid = ReadGrid();
            HashSet<(int, int)> flashed = new HashSet<(int, int)>();
            Queue<(int, int)> flash = new Queue<(int, int)>();

            long sum = 0;

            for (int i = 0; i < 100; i++)
            {
                sum += Round(grid, flashed, flash);

#if SAMPLE
                if (i < 11 || i % 10 == 9)
                {
                    Console.WriteLine($"After round {i+1}:");

                    for (int row = 0; row < grid.Length; row++)
                    {
                        foreach (int val in grid[row])
                        {
                            Console.Write(val);
                        }

                        Console.WriteLine();
                    }

                    Console.WriteLine($"  Total flashes is now {sum}");
                    Console.WriteLine();
                }
#endif
            }

            Console.WriteLine(sum);
        }

        internal static void Problem2()
        {
            int[][] grid = ReadGrid();
            HashSet<(int, int)> flashed = new HashSet<(int, int)>();
            Queue<(int, int)> flash = new Queue<(int, int)>();

            int rounds = 0;

            while (grid.SelectMany(x => x).Any(x => x != 0))
            {
                rounds++;
                Round(grid, flashed, flash);
            }

            Console.WriteLine(rounds);
        }

        private static int Round(int[][] grid, HashSet<(int, int)> flashed, Queue<(int,int)> flash)
        {
            flashed.Clear();
            flash.Clear();
            int timesFlashed = 0;

            for (int rowIdx = grid.Length - 1; rowIdx >= 0; rowIdx--)
            {
                int[] row = grid[rowIdx];

                for (int colIdx = 0; colIdx < row.Length; colIdx++)
                {
                    if (++row[colIdx] > 9)
                    {
                        if (flashed.Add((rowIdx, colIdx)))
                        {
                            flash.Enqueue((rowIdx, colIdx));
                        }
                    }
                }
            }

            while (flash.TryDequeue(out var tuple))
            {
                timesFlashed++;

                for (int rowIdx = tuple.Item1 - 1; rowIdx <= tuple.Item1 + 1; rowIdx++)
                {
                    for (int colIdx = tuple.Item2 - 1; colIdx <= tuple.Item2 + 1; colIdx++)
                    {
                        if (rowIdx >= 0 && colIdx >= 0 && rowIdx < 10 && colIdx < 10 &&
                            !(rowIdx == tuple.Item1 && colIdx == tuple.Item2))
                        {
                            if (++grid[rowIdx][colIdx] > 9)
                            {
                                if (flashed.Add((rowIdx, colIdx)))
                                {
                                    flash.Enqueue((rowIdx, colIdx));
                                }
                            }
                        }
                    }
                }
            }

            for (int rowIdx = grid.Length - 1; rowIdx >= 0; rowIdx--)
            {
                int[] row = grid[rowIdx];

                for (int colIdx = 0; colIdx < row.Length; colIdx++)
                {
                    if (row[colIdx] > 9)
                    {
                        row[colIdx] = 0;
                    }
                }
            }

            return timesFlashed;
        }

        private static int[][] ReadGrid()
        {
            int[][] grid = new int[10][];
            int row = 0;

            foreach (string line in Data.Enumerate())
            {
                grid[row] = line.AsEnumerable().Select(c => (int)(c - '0')).ToArray();
                row++;
            }

            return grid;
        }
    }
}
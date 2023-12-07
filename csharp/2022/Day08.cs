using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal class Day08
    {
        private static int[][] LoadGrid()
        {
            List<int[]> grid = new();

            foreach (string s in Data.Enumerate())
            {
                grid.Add(s.Select(c => c - '0').ToArray());
            }

            return grid.ToArray();
        }

        internal static void Problem1()
        {
            int visible = 0;
            int[][] grid = LoadGrid();

            visible += 2 * grid[0].Length;
            visible += 2 * grid.Length;
            visible -= 4;

            for (int row = grid.Length - 2; row > 0; row--)
            {
                for (int col = grid[0].Length - 2; col > 0; col--)
                {
                    if (IsVisible1(grid, row, col))
                    {
                        visible++;
                    }
                }
            }

            static bool IsVisible(int[][] grid, int row, int col, int rowPlus, int colPlus)
            {
                int val = grid[ row ][ col ];

                for (
                    int tr = row + rowPlus, tc = col + colPlus;
                    tr >= 0 && tr < grid.Length && tc >= 0 && tc < grid.Length;
                    tr += rowPlus, tc += colPlus)
                {
                    if (grid[tr][tc] >= val)
                    {
                        return false;
                    }
                }

                return true;
            }

            static bool IsVisible1(int[][] grid, int row, int col)
            {
                return
                    IsVisible(grid, row, col, 1, 0) ||
                    IsVisible(grid, row, col, -1, 0) ||
                    IsVisible(grid, row, col, 0, 1) ||
                    IsVisible(grid, row, col, 0, -1);
            }

            Console.WriteLine(visible);
        }

        internal static void Problem2()
        {
            int[][] grid = LoadGrid();
            int best = int.MinValue;

            for (int row = grid.Length - 2; row > 0; row--)
            {
                for (int col = grid[0].Length - 2; col > 0; col--)
                {
                    best = Math.Max(best, ScenicScore1(grid, row, col));
                }
            }

            Console.WriteLine(best);

            static int ScenicScore(int[][] grid, int row, int col, int rowPlus, int colPlus)
            {
                int val = grid[row][col];
                int vis = 0;

                for (
                    int tr = row + rowPlus, tc = col + colPlus;
                    tr >= 0 && tr < grid.Length && tc >= 0 && tc < grid.Length;
                    tr += rowPlus, tc += colPlus)
                {
                    vis++;

                    if (grid[tr][tc] >= val)
                    {
                        break;
                    }
                }

                return vis;
            }

            static int ScenicScore1(int[][] grid, int row, int col)
            {
                return
                    ScenicScore(grid, row, col, 1, 0) *
                    ScenicScore(grid, row, col, -1, 0) *
                    ScenicScore(grid, row, col, 0, 1) *
                    ScenicScore(grid, row, col, 0, -1);
            }
        }
    }
}
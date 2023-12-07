using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day9
    {
        internal static void Problem1()
        {
            int[][] map = ReadMap();
            List<(int Row, int Col)> lowPoints = FindLowpoints(map);
            int score = lowPoints.Count;

            foreach (var tuple in lowPoints)
            {
                score += map[tuple.Row][tuple.Col];
            }

            Console.WriteLine(score);
        }

        internal static void Problem2()
        {
            int[][] map = ReadMap();
            List<(int Row, int Col)> lowPoints = FindLowpoints(map);
            List<int> basinSizes = new List<int>(lowPoints.Count);

            foreach ((int row, int col) in lowPoints)
            {
                basinSizes.Add(GetBasinSize(map, row, col));
            }

            long product = 1;

            foreach (int val in basinSizes.OrderByDescending(x => x).Take(3))
            {
                product *= val;
            }

            Console.WriteLine(product);
        }

        private static int GetBasinSize(int[][] map, int row, int col)
        {
            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            Queue<(int, int)> pending = new Queue<(int, int)>();
            pending.Enqueue((row, col));

            int size = 0;

            while (pending.TryDequeue(out var point))
            {
                if (visited.Add(point) && map[point.Item1][point.Item2] < 9)
                {
                    size++;

                    pending.Enqueue((point.Item1 - 1, point.Item2));
                    pending.Enqueue((point.Item1 + 1, point.Item2));
                    pending.Enqueue((point.Item1, point.Item2 - 1));
                    pending.Enqueue((point.Item1, point.Item2 + 1));
                }
            }

            return size;
        }

        private static List<(int Row, int Col)> FindLowpoints(int[][] map)
        {
            List<(int, int)> lowPoints = new List<(int, int)>();

            for (int row = map.Length - 2; row > 0; row--)
            {
                int[] rowData = map[row];

                for (int col = rowData.Length - 2; col > 0; col--)
                {
                    int cur = rowData[col];

                    if (cur < rowData[col - 1] && cur < rowData[col + 1] &&
                        cur < map[row - 1][col] && cur < map[row + 1][col])
                    {
                        lowPoints.Add((row, col));
                    }
                }
            }

            return lowPoints;
        }
        private static int[][] ReadMap()
        {
            int[][] map = new int[102][];
            int row;

            for (row = 0; row < map.Length; row++)
            {
                map[row] = new int[102];
                map[row].AsSpan().Fill(9);
            }

            row = 0;

            foreach (string line in Data.Enumerate())
            {
                row++;

                line.AsEnumerable().Select(c => (int)c - (int)'0').ToArray().CopyTo(map[row].AsSpan(1));
            }

            return map;
        }
    }
}
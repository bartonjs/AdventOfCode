using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day15
    {
        internal static void Problem1()
        {
            int[][] map = LoadMap();
            Console.WriteLine(AStarCost(map));
        }

        internal static void Problem2()
        {
            int[][] start = LoadMap();
            int startCols = start[0].Length;
            int[][] fullMap = new int[start.Length * 5][];

            for (int rowId = 0; rowId < fullMap.Length; rowId++)
            {
                int rowPhase = Math.DivRem(rowId, start.Length, out int seedRowId);
                int[] seedRow = start[seedRowId];
                int[] newRow = new int[startCols * 5];
                fullMap[rowId] = newRow;

                for (int colId = 0; colId < newRow.Length; colId++)
                {
                    int colPhase = Math.DivRem(colId, startCols, out int seedColId);

                    newRow[colId] = seedRow[seedColId] + rowPhase + colPhase;

                    if (newRow[colId] > 9)
                    {
                        newRow[colId] -= 9;
                        Debug.Assert(newRow[colId] <= 9);
                    }
                }
            }

            Console.WriteLine(AStarCost(fullMap));
        }

        private static int AStarCost(int[][] map)
        {
            List<(int, int)> openSet = new();
#if SAMPLE
            Dictionary<(int, int), (int, int)> cameFrom = new Dictionary<(int, int), (int, int)>();
#endif
            Dictionary<(int, int), int> gScore = new Dictionary<(int, int), int>();
            Dictionary<(int, int), int> fScore = new Dictionary<(int, int), int>();

            int lastRowId = map.Length - 1;
            int lastColId = map[0].Length - 1;

            {
                var start = (0, 0);

                openSet.Add(start);
                gScore[start] = 0;
                fScore[start] = lastRowId + lastColId;
            }

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(n => fScore.GetValueOrDefault(n, int.MaxValue)).First();
                int gScoreCurrent = gScore[current];

                if (current.Item1 == lastRowId && current.Item2 == lastColId)
                {
#if SAMPLE
                    List<(int, int)> path = new List<(int, int)>();

                    path.Add(current);

                    while (cameFrom.TryGetValue(current, out current))
                    {
                        path.Add(current);
                    }

                    path.Reverse();
                    Console.WriteLine(string.Join(Environment.NewLine, path));
#endif
                    return gScoreCurrent;
                }

                openSet.Remove(current);

                foreach (var neighbor in Neighbors(current, map))
                {
                    (int newRowId, int newColId) = neighbor;
                    int gScoreTentative = gScoreCurrent + map[newRowId][newColId];

                    if (gScoreTentative < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    {
#if SAMPLE
                        cameFrom[neighbor] = current;
#endif
                        gScore[neighbor] = gScoreTentative;
                        fScore[neighbor] = gScoreTentative + (lastRowId - newRowId) + (lastColId - newColId);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            throw new InvalidOperationException();
        }

        private static IEnumerable<(int, int)> Neighbors((int, int) current, int[][] map)
        {
            if (current.Item1 > 0)
            {
                yield return (current.Item1 - 1, current.Item2);
            }

            if (current.Item1 < map.Length - 1)
            {
                yield return (current.Item1 + 1, current.Item2);
            }

            if (current.Item2 > 0)
            {
                yield return (current.Item1, current.Item2 - 1);
            }

            if (current.Item2 < map[current.Item1].Length - 1)
            {
                yield return (current.Item1, current.Item2 + 1);
            }
        }

        private static int[][] LoadMap()
        {
            List<int[]> map = new();

            foreach (string line in Data.Enumerate())
            {
                int[] row = line.AsEnumerable().Select(c => c - '0').ToArray();
                map.Add(row);
            }

            return map.ToArray();
        }
    }
}

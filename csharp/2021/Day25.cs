using System;
using System.Diagnostics;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day25
    {
        internal static void Problem1()
        {
            int[,] map = LoadGrid();
            int move = 1;

            while (Update(map))
            {
                move++;
            }

            
            Console.WriteLine(move);
        }

        private static bool Update(int[,] map)
        {
            bool movedRight = UpdateRight(map);
            bool movedDown = UpdateDown(map);

            return movedRight || movedDown;
        }

        private static bool UpdateDown(int[,] map)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            bool moved = false;

            for (int col = cols - 1; col >= 0; col--)
            {
                for (int row = rows - 1; row >= 0; row--)
                {
                    int destRow = (row + 1) % rows;

                    if (map[row, col] == 2 && map[destRow, col] == 0)
                    {
                        map[destRow, col] = -1;
                        map[row, col] = -2;
                        moved = true;
                    }
                }
            }

            for (int col = cols - 1; col >= 0; col--)
            {
                for (int row = rows - 1; row >= 0; row--)
                {
                    if (map[row, col] == -1)
                    {
                        map[row, col] = 2;
                    }
                    else if (map[row, col] == -2)
                    {
                        map[row, col] = 0;
                    }
                }
            }

            return moved;
        }

        private static bool UpdateRight(int[,] map)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            bool moved = false;

            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = cols - 1; col >= 0; col--)
                {
                    int destCol = (col + 1) % cols;

                    if (map[row, col] == 1 && map[row, destCol] == 0)
                    {
                        map[row, destCol] = -1;
                        map[row, col] = -2;
                        moved = true;
                    }
                }
            }

            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = cols - 1; col >= 0; col--)
                {
                    if (map[row, col] == -1)
                    {
                        map[row, col] = 1;
                    }
                    else if (map[row, col] == -2)
                    {
                        map[row, col] = 0;
                    }
                }
            }

            return moved;
        }

        private static int[,] LoadGrid()
        {
#if SAMPLE
            int[,] map = new int[9, 10];
#else
            int[,] map = new int[137, 139];
#endif
            int row = 0;
            int cols = map.GetLength(1);

            foreach (string line in Data.Enumerate())
            {
                Debug.Assert(line.Length == cols);

                int col = 0;

                foreach (char c in line)
                {
                    switch (c)
                    {
                        case '.':
                            break;
                        case '>':
                            map[row, col] = 1;
                            break;
                        case 'v':
                            map[row, col] = 2;
                            break;
                    }

                    col++;
                }

                row++;
            }

            return map;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day04
    {
        private static List<List<char>> Load()
        {
            List<List<char>> ret = new();

            foreach (string s in Data.Enumerate())
            {
                ret.Add(s.ToList());
            }

            return ret;
        }

        internal static void Problem1()
        {
            long ret = 0;

            List<List<char>> grid = Load();

            for (int i = 0; i < grid.Count; i++)
            {
                List<char> row = grid[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == 'X')
                    {
                        for (int jDir = -1; jDir <= 1; jDir++)
                        {
                            for (int iDir = -1; iDir <= 1; iDir++)
                            {
                                if ((jDir | iDir) == 0)
                                {
                                    continue;
                                }

                                if (grid.SafeIndex(i + iDir)?.SafeIndex(j + jDir) == 'M')
                                {
                                    if (grid.SafeIndex(i + 2 * iDir)?.SafeIndex(j + 2 * jDir) == 'A')
                                    {
                                        if (grid.SafeIndex(i + 3 * iDir)?.SafeIndex(j + 3 * jDir) == 'S')
                                        {
                                            ret++;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            List<List<char>> grid = Load();

            for (int i = 0; i < grid.Count; i++)
            {
                List<char> row = grid[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == 'M')
                    {
                        if (row.SafeIndex(j + 2) == 'M')
                        {
                            if (grid.SafeIndex(i + 1)?.SafeIndex(j + 1) == 'A')
                            {
                                List<char> otherRow = grid.SafeIndex(i + 2);

                                if (otherRow?.SafeIndex(j) == 'S' && otherRow?.SafeIndex(j + 2) == 'S')
                                {
                                    ret++;
                                }
                            }

                            if (grid.SafeIndex(i - 1)?.SafeIndex(j + 1) == 'A')
                            {
                                List<char> otherRow = grid.SafeIndex(i - 2);

                                if (otherRow?.SafeIndex(j) == 'S' && otherRow?.SafeIndex(j + 2) == 'S')
                                {
                                    ret++;
                                }
                            }
                        }

                        if (grid.SafeIndex(i + 2)?[j] == 'M')
                        {
                            if (grid.SafeIndex(i + 1)?.SafeIndex(j + 1) == 'A')
                            {
                                List<char> otherRow = grid.SafeIndex(i + 2);

                                if (row.SafeIndex(j + 2) == 'S' && otherRow?.SafeIndex(j + 2) == 'S')
                                {
                                    ret++;
                                }
                            }

                            if (grid.SafeIndex(i + 1)?.SafeIndex(j - 1) == 'A')
                            {
                                List<char> otherRow = grid.SafeIndex(i + 2);

                                if (row.SafeIndex(j - 2) == 'S' && otherRow?.SafeIndex(j - 2) == 'S')
                                {
                                    ret++;
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine(ret);
        }
    }
}
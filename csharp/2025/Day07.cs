using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day07
    {
        private static DynamicPlane<char> Load()
        {
            DynamicPlane<char> grid = null;

            foreach (string s in Data.Enumerate())
            {
                if (grid is null)
                {
                    grid = new DynamicPlane<char>(s.ToCharArray());
                }
                else
                {
                    grid.PushY(s.ToCharArray());
                }
            }

            return grid;
        }

        internal static void Problem1()
        {
            long ret = 0;

            DynamicPlane<char> grid = Load();

            for (int row = 0; row < grid.Height; row++)
            {
                for (int col = 0; col < grid.Width; col++)
                {
                    Point here = new Point(col, row);

                    if (grid[here] is 'S' or '|')
                    {
                        Point under = here.South();

                        if (grid.TryGetValue(under, out char there))
                        {
                            if (there == '^')
                            {
                                Point left = under.West();
                                Point right = under.East();

                                if (grid.TryGetValue(left, out char lSplit))
                                {
                                    if (lSplit == '.')
                                    {
                                        grid[left] = '|';
                                    }
                                }

                                if (grid.TryGetValue(right, out char rSplit))
                                {
                                    if (rSplit == '.')
                                    {
                                        grid[right] = '|';
                                    }
                                }

                                grid[under] = 'A';
                                ret++;
                            }
                            else if (there == '.')
                            {
                                grid[under] = '|';
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

            DynamicPlane<char> grid = Load();
            Queue<Point> queue = new();
            Dictionary<Point, long> scores = new();

            for (int row = 0; row < grid.Height; row++)
            {
                for (int col = 0; col < grid.Width; col++)
                {
                    Point here = new Point(col, row);

                    if (grid[here] == 'S')
                    {
                        queue.Enqueue(here);
                        ret = Score(grid, here, scores);
                    }
                }
            }

            Console.WriteLine(ret);

            static long Score(DynamicPlane<char> grid, Point here, Dictionary<Point, long> scores)
            {
                if (scores.TryGetValue(here, out long score))
                {
                    return score;
                }

                if (!grid.ContainsPoint(here))
                {
                    return 0;
                }

                Point test = here;

                while (grid.TryGetValue(test, out char value))
                {
                    if (value is 'S' or '.')
                    {
                        test = test.South();
                    }
                    else if (value == '^')
                    {
                        score = Score(grid, test.West(), scores);
                        score += Score(grid, test.East(), scores);

                        scores[here] = score;
                        return score;
                    }
                }

                return 1;
            }
        }
    }
}

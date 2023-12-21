using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day21
    {
        internal static void Problem1()
        {
            (DynamicPlane<State> plane, Point start) = Load();

            HashSet<Point> visitable = new() { start };
            HashSet<Point> prev = new();

            const int Steps =
#if SAMPLE
                6
#else
                64
#endif
                ;

            for (int i = 0; i < Steps; i++)
            {
                (prev, visitable) = (visitable, prev);
                visitable.Clear();

                foreach (Point from in prev)
                {
                    foreach (Point next in Neighbors(plane, from))
                    {
                        visitable.Add(next);
                    }

                    static IEnumerable<Point> Neighbors(DynamicPlane<State> dynamicPlane, Point point)
                    {
                        Point[] points = new[] { point.North(), point.South(), point.West(), point.East() };

                        foreach (Point test in points)
                        {
                            if (dynamicPlane.TryGetValue(test, out State state) && state.Char != '#')
                            {
                                yield return test;
                            }
                        }
                    }
                }
            }

            Console.WriteLine(visitable.Count);
        }

        internal static void Problem2()
        {
            (DynamicPlane<State> plane, Point start) = Load();
            int gridSize = plane.Width;
            Debug.Assert(gridSize == plane.Height);

#if SAMPLE
            // This underreports the sample, but worked for the real puzzle.
            // Given it wasn't an interesting problem to solve, meh.
            const int TotalSteps = 5000;
#else
            const int TotalSteps = 26501365;
#endif

            (int fullGrids, int remainingSteps) = int.DivRem(TotalSteps, gridSize);

            Console.WriteLine($"{TotalSteps} is {fullGrids} full grids and {remainingSteps} extra steps.");

            HashSet<Point> visitable = new() { start };
            HashSet<Point> prev = new();
            List<int> sequencePoints = new(3);
            int step = 0;

            for (int gridsToWalk = 0; gridsToWalk < sequencePoints.Capacity; gridsToWalk++)
            {
                int stepCount = remainingSteps + gridsToWalk * gridSize;

                // Don't reset the step count to 0 since we have a rolling interval and visitable
                // is already the points at (step-1)
                for (; step < stepCount; step++)
                {
                    (prev, visitable) = (visitable, prev);
                    visitable.Clear();

                    foreach (Point from in prev)
                    {
                        foreach (Point next in Neighbors(plane, from))
                        {
                            visitable.Add(next);
                        }

                        static IEnumerable<Point> Neighbors(DynamicPlane<State> dynamicPlane, Point point)
                        {
                            Point[] points = new[] { point.North(), point.South(), point.West(), point.East() };

                            foreach (Point test in points)
                            {
                                Point mapped = Map(test, dynamicPlane.Width);
                                State state = dynamicPlane[mapped];

                                if (state.Char != '#')
                                {
                                    yield return test;
                                }
                            }
                        }

                        static Point Map(Point p, int gridSize)
                        {
                            int x = p.X % gridSize;
                            int y = p.Y % gridSize;

                            if (x < 0)
                            {
                                x += gridSize;
                            }

                            if (y < 0)
                            {
                                y += gridSize;
                            }

                            return new Point(x, y);
                        }
                    }
                }

                Console.WriteLine($"After {stepCount} steps, {visitable.Count} visitable spaces.");
                sequencePoints.Add(visitable.Count);
            }

            // ax^2 + bx + c, where x is the number of grids to walk.
            // sequence[0] is x == 0, so a*0^2 + b*0 + c = sequence[0] => c = sequence[0]
            // sequence[1] is x == 1, so a*1^2 + b*1 + c = seq1 => a + b + c = seq1, a + b = seq1 - c.
            // sequence[2] is x == 2, so 4a + 2b + c = seq2, 4a + 2b = seq2 - c

            int c = sequencePoints[0];
            int aPlusB = sequencePoints[1] - c;
            int a4PlusB2 = sequencePoints[2] - c;
            int a2 = a4PlusB2 - 2 * aPlusB;
            double a = a2 / 2.0;
            double b = aPlusB - a;

            Console.WriteLine($"F(x) = {a}x^2 + {b}x + {c}");

            double ret = fullGrids;
            ret *= fullGrids;
            ret *= a;
            ret += (long)b * fullGrids;
            ret += c;

            Console.WriteLine($"F({fullGrids}) = {ret}");
            Console.WriteLine(ret);
        }

        private static (DynamicPlane<State> plane, Point start) Load()
        {
            DynamicPlane<State> plane = null;
            int x = 0;
            int y = 0;
            int curY = 0;

            foreach (string s in Data.Enumerate())
            {
                State[] array = s.Select(c => new State { Char = c, }).ToArray();
                int startIdx = s.IndexOf('S');

                if (startIdx >= 0)
                {
                    x = startIdx;
                    y = curY;
                }

                if (plane is null)
                {
                    plane = new DynamicPlane<State>(array);
                }
                else
                {
                    plane.PushY(array);
                }

                curY++;
            }

            return (plane, new Point(x, y));
        }

        private class State
        {
            public char Char;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day13
    {
        private struct Scenario
        {
            internal LongPoint A;
            internal LongPoint B;
            internal LongPoint Win;
        }

        private static IEnumerable<Scenario> Load()
        {
            Regex regexA = new Regex(@"^Button A: X\+(\d+), Y\+(\d+)$");
            Regex regexB = new Regex(@"^Button B: X\+(\d+), Y\+(\d+)$");
            Regex regexPrize = new Regex(@"^Prize: X=(\d+), Y=(\d+)$");
            LongPoint a = default;
            LongPoint b = default;
            LongPoint win = default;
            
            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrEmpty(s))
                {
                    yield return new Scenario
                    {
                        A = a,
                        B = b,
                        Win = win,
                    };

                    continue;
                }

                Match match = regexA.Match(s);

                if (match.Success)
                {
                    a = new LongPoint(int.Parse(match.Groups[1].ValueSpan), int.Parse(match.Groups[2].ValueSpan));
                    continue;
                }

                match = regexB.Match(s);

                if (match.Success)
                {
                    b = new LongPoint(int.Parse(match.Groups[1].ValueSpan), int.Parse(match.Groups[2].ValueSpan));
                    continue;
                }

                match = regexPrize.Match(s);

                if (match.Success)
                {
                    win = new LongPoint(int.Parse(match.Groups[1].ValueSpan), int.Parse(match.Groups[2].ValueSpan));
                    continue;
                }

                throw new InvalidDataException();
            }

            yield return new Scenario
            {
                A = a,
                B = b,
                Win = win,
            };
        }

        internal static void Problem1()
        {
            long ret = 0;

            // This should just use the cost formula from below... but I started with A*, so I'll leave it.

            foreach (Scenario scenario in Load())
            {
                long localCost = Pathing.AStar(
                    scenario,
                    new LongPoint(0, 0),
                    scenario.Win,
                    Neighbors,
                    static (candidate, end, scenario) => candidate.ManhattanDistance(end));

                if (localCost < long.MaxValue)
                {
                    ret += localCost;
                }
            }

            Console.WriteLine(ret);

            static IEnumerable<(LongPoint, long)> Neighbors(LongPoint from, Scenario scenario)
            {
                LongPoint maybe = from + scenario.A;

                if (maybe.X <= scenario.Win.X && maybe.Y <= scenario.Win.Y)
                {
                    yield return (maybe, 3);
                }

                maybe = from + scenario.B;

                if (maybe.X <= scenario.Win.X && maybe.Y <= scenario.Win.Y)
                {
                    yield return (maybe, 1);
                }
            }
        }

        internal static void Problem2()
        {
            long ret = 0;
            LongPoint offset = new LongPoint(10000000000000, 10000000000000);

            foreach (Scenario scenario in Load())
            {
                ret += Cost(scenario.A, scenario.B, scenario.Win + offset);
            }

            Console.WriteLine(ret);
        }

        private static long Cost(LongPoint a, LongPoint b, LongPoint win)
        {
            // Treat it as an intersection between the line that contains the origin and A,
            // as well as one that contains the win state and (Win-B).
            //
            // This works so long as A and B aren't parallel. If they are, the answer is just
            // Min(Win/B, 3 * (Win/A)).

            // scalarA * A.X + scalarB * B.X = Win.X
            // scalarA * A.Y + scalarB * B.Y = Win.Y
            //
            // scalarA * A.X + scalarB * B.X - Win.X = 0
            // scalarA * A.Y + scalarB * B.Y - Win.Y = 0
            //
            // scalarA and scalarB are the two unknowns, so call them x and y.
            // A.X * x + B.X * y - Win.X = 0
            // A.Y * x + B.Y * y - Win.Y = 0
            //
            // That looks like https://www.cuemath.com/geometry/intersection-of-two-lines/
            // where (A.X, B.X, -Win.X) is (a1, b1, c1) and (A.Y, B.Y, -Win.Y) is (a2, b2, c2)
            long a1 = a.X;
            long b1 = b.X;
            long c1 = -win.X;
            long a2 = a.Y;
            long b2 = b.Y;
            long c2 = -win.Y;

            long denom = a1 * b2 - a2 * b1;

            // If we get a remainder, we don't have an integer solution, so for the stated problem
            // it has 0 cost.

            (long scalarA, long rem) = long.DivRem(b1 * c2 - b2 * c1, denom);

            if (rem != 0)
            {
                return 0;
            }

            (long scalarB, rem) = long.DivRem(c1 * a2 - c2 * a1, denom);

            if (rem != 0)
            {
                return 0;
            }

            return 3 * scalarA + scalarB;
        }
    }
}
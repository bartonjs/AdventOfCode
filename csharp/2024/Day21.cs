using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day21
    {
        private const char Right = '>';
        private const char Up = '^';
        private const char Down = 'v';
        private const char Left = '<';
        private const char Submit = 'A';
        private const char Error = '!';

        private static readonly Dictionary<(char, char), List<string>> s_keypadPaths = new();
        private static readonly Dictionary<(char, char), List<string>> s_dpadPaths = new();
        private static readonly Dictionary<(string, int), long> s_dpadCosts = new();

        private static FixedPlane<char> s_keypad = LoadKeypad();
        private static FixedPlane<char> s_dpad = LoadDPad();

        private static FixedPlane<char> LoadKeypad()
        {
            FixedPlane<char> plane = new FixedPlane<char>(3, 4);
            
            plane[0, 0] = '7';
            plane[1, 0] = '8';
            plane[2, 0] = '9';
            plane[0, 1] = '4';
            plane[1, 1] = '5';
            plane[2, 1] = '6';
            plane[0, 2] = '1';
            plane[1, 2] = '2';
            plane[2, 2] = '3';
            plane[0, 3] = Error;
            plane[1, 3] = '0';
            plane[2, 3] = Submit;

            return plane;
        }

        private static FixedPlane<char> LoadDPad()
        {
            FixedPlane<char> plane = new FixedPlane<char>(3, 2);

            plane[0, 0] = Error;
            plane[1, 0] = Up;
            plane[2, 0] = Submit;
            plane[0, 1] = Left;
            plane[1, 1] = Down;
            plane[2, 1] = Right;

            return plane;
        }

        private static List<string> Load()
        {
            return Data.Enumerate().ToList();
        }

        internal static void Problem1()
        {
            Run(2);
        }

        internal static void Problem2()
        {
            Run(25);
        }

        private static void Run(int depth)
        {
            long ret = 0;

            foreach (string code in Load())
            {
                ret += Complexity(code, depth);
            }

            Console.WriteLine(ret);
        }

        private static long Complexity(ReadOnlySpan<char> code, int depth)
        {
            long numericScore = long.Parse(code.Slice(0, code.Length - 1));
            long accum = 0;
            char pos = 'A';

            foreach (char c in code)
            {
                long min = long.MaxValue;

                Print.ForSample($"Need cost from {pos} to {c}:");

                foreach (string path in KeypadPaths(pos, c))
                {
                    Print.ForSample($"  Path {path}:");
                    long cur = DpadCost(path, depth);
                    Print.ForSample($"  ... cost {cur}:");
                    min = long.Min(cur, min);
                }

                pos = c;
                accum += min;
            }

            return accum * numericScore;
        }

        private static long DpadCost(string seq, int depth)
        {
            if (depth == 0)
            {
                return seq.Length;
            }

            var key = (seq, depth);

            if (s_dpadCosts.TryGetValue(key, out long cost))
            {
                return cost;
            }

            char prev = Submit;
            long accum = 0;

            foreach (char next in seq)
            {
                long min = long.MaxValue;
                

                foreach (string path in DpadPaths(prev, next))
                {
                    cost = DpadCost(path, depth - 1);
                    min = long.Min(min, cost);
                }

                accum += min;
                prev = next;
            }

            return s_dpadCosts[key] = accum;
        }

        private static IEnumerable<string> DpadPaths(char from, char to)
        {
            var key = (from, to);

            if (s_dpadPaths.TryGetValue(key, out List<string> ret))
            {
                return ret;
            }

            Point fromPoint = new Point(Col(from), Row(from));
            Point toPoint = new Point(Col(to), Row(to));

            return s_dpadPaths[key] = GetPaths(s_dpad, fromPoint, toPoint);

            static int Row(char c) => c switch
            {
                '^' or 'A' => 0,
                '<' or 'v' or '>' => 1,
            };

            static int Col(char c) => c switch
            {
                '<' or '4' or '1' => 0,
                '^' or 'v' => 1,
                'A' or '>' => 2,
            };
        }

        private static IEnumerable<string> KeypadPaths(char from, char to)
        {
            var key = (from, to);

            if (s_keypadPaths.TryGetValue(key, out List<string> ret))
            {
                return ret;
            }

            Point fromPoint = new Point(Col(from), Row(from));
            Point toPoint = new Point(Col(to), Row(to));

            return s_keypadPaths[key] = GetPaths(s_keypad, fromPoint, toPoint);

            static int Row(char c) => c switch
            {
                '7' or '8' or '9' => 0,
                '4' or '5' or '6' => 1,
                '1' or '2' or '3' => 2,
                '0' or 'A' => 3,
            };

            static int Col(char c) => c switch
            {
                '7' or '4' or '1' => 0,
                '8' or '5' or '2' or '0' => 1,
                '9' or '6' or '3' or 'A' => 2,
            };
        }

        private static List<string> GetPaths(FixedPlane<char> plane, Point fromPoint, Point toPoint)
        {
            Dictionary<Point, int> gScore = new();

            long localCost = Pathing.AStar(
                plane,
                fromPoint,
                toPoint,
                (candidate, plane) => candidate.GetCardinalNeighbors()
                    .Where(p => plane.TryGetValue(p, out char value) && value != Error).Select(p => (p, 1)),
                (candidate, end, world) => candidate.ManhattanDistance(end),
                gScore: gScore,
                allPaths: true);

            if (localCost == long.MaxValue)
            {
                throw new InvalidDataException();
            }

            return AllPaths(gScore, toPoint);
        }

        private static List<string> AllPaths(Dictionary<Point, int> scores, Point final)
        {
            List<string> allPaths = new();
            Stack<Point> curPathRev = new();

            curPathRev.Push(final);
            BuildPath(scores, curPathRev, allPaths);
            curPathRev.Pop();

            return allPaths;

            static void BuildPath(
                Dictionary<Point, int> scores,
                Stack<Point> curPathRev,
                List<string> allPaths)
            {
                Point cur = curPathRev.Peek();
                int depth = curPathRev.Count;
                Span<Point> nextStates = stackalloc Point[3];

                try
                {
                    int count = 0;

                    while (true)
                    {
                        long curCost = scores[cur];

                        if (curCost == 0)
                        {
                            StringBuilder builder = new StringBuilder(curPathRev.Count);
                            Point prev = default;
                            bool havePrev = false;

                            foreach (Point next in curPathRev)
                            {
                                if (havePrev)
                                {
                                    int dx = next.X - prev.X;
                                    int dy = next.Y - prev.Y;

                                    char direction = (dx, dy) switch
                                    {
                                        (-1, 0) => Left,
                                        (1, 0) => Right,
                                        (0, 1) => Down,
                                        (0, -1) => Up,
                                    };

                                    builder.Append(direction);
                                }

                                prev = next;
                                havePrev = true;
                            }

                            builder.Append(Submit);
                            allPaths.Add(builder.ToString());
                            return;
                        }

                        count = 0;

                        foreach (Point next in cur.GetCardinalNeighbors())
                        {
                            if (scores.TryGetValue(next, out int nextScore))
                            {
                                if (nextScore + 1 == curCost)
                                {
                                    nextStates[count] = next;
                                    count++;
                                }
                            }
                        }

                        if (count == 0)
                        {
                            return;
                        }

                        if (count > 1)
                        {
                            break;
                        }

                        cur = nextStates[0];
                        curPathRev.Push(cur);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        curPathRev.Push(nextStates[i]);
                        BuildPath(scores, curPathRev, allPaths);
                        curPathRev.Pop();
                    }
                }
                finally
                {
                    while (curPathRev.Count > depth)
                    {
                        curPathRev.Pop();
                    }
                }
            }
        }
    }
}
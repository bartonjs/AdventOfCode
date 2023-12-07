using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day24
    {
        private static (List<Blizzard> blizzards, int xMax, int yMax) Load()
        {
            List<Blizzard> blizzards = new List<Blizzard>();
            int y = 0;
            int xMax = 0;

            foreach (string s in Data.Enumerate().Skip(1).SkipLast(1))
            {
                ReadOnlySpan<char> text = s;
                xMax = text.Length - 2;

                for (int i = 1; i < text.Length - 1; i++)
                {
                    if (text[i] == '.')
                    {
                        continue;
                    }

                    Debug.Assert(text[i] != '#');

                    blizzards.Add(new Blizzard { Position = new Point(i - 1, y), Direction = text[i] });
                }

                y++;
            }

            return (blizzards, xMax, y);
        }

        private struct Blizzard
        {
            public Point Position;
            public char Direction;

            private Point NextSpace(int xMax, int yMax)
            {
                return Clamp(
                    xMax,
                    yMax,
                    Direction switch
                    {
                        '^' => Position.North(),
                        'v' => Position.South(),
                        '>' => Position.East(),
                        '<' => Position.West(),
                    });

                static Point Clamp(int xMax, int yMax, Point point)
                {
                    return new Point((point.X + xMax) % xMax, (point.Y + yMax) % yMax);
                }
            }

            public Blizzard BlizzardInNextSpace(int xMax, int yMax)
            {
                return this with { Position = NextSpace(xMax, yMax) };
            }
        }

        private record struct State(Point Position, int RoundNumber, int Phase)
        {
        }

        private static int s_stateMax;
        private static int s_xMax;
        private static int s_yMax;

        private static Point s_start = new Point(0, -1);
        private static Point s_end;

        internal static void Problem1()
        {
            (List<Blizzard> blizzards, int xMax, int yMax) = Load();
            s_xMax = xMax;
            s_yMax = yMax;
            s_stateMax = xMax * yMax;
            s_end = new Point(xMax - 1, s_yMax);
            List<Blizzard>[] allBlizzardStates = new List<Blizzard>[s_stateMax];

            allBlizzardStates[0] = blizzards;
            List<Blizzard> prev = blizzards;

            HashSet<Point>[] occupiedSpaces = new HashSet<Point>[s_stateMax];

            for (int i = 1; i < s_stateMax; i++)
            {
                List<Blizzard> cur = new List<Blizzard>(prev.Count);

                foreach (Blizzard b in prev)
                {
                    cur.Add(b.BlizzardInNextSpace(xMax, yMax));
                }

                allBlizzardStates[i] = cur;
                prev = cur;
            }

            for (int i = 0; i < s_stateMax; i++)
            {
                HashSet<Point> cur = new HashSet<Point>();

                foreach (Blizzard b in allBlizzardStates[i])
                {
                    cur.Add(b.Position);
                }

                occupiedSpaces[i] = cur;
            }

            Dictionary<State, State> parentTracker = null;
#if SAMPLE
            parentTracker = new Dictionary<State, State>();
#endif

            (bool Found, State Value) answer = Pathing.BreadthFirstSearch(
                occupiedSpaces,
                new State { Position = new Point(0, -1), RoundNumber = 0},
                s => s.Position == s_end,
                NextStates,
                parentTracker: parentTracker);

            if (!answer.Found)
            {
                throw new InvalidOperationException("No solution");
            }

#if SAMPLE
            Stack<State> unwind = new Stack<State>();
            State blah = answer.Value;
            unwind.Push(blah);

            while (parentTracker.TryGetValue(blah, out blah))
            {
                unwind.Push(blah);
            }

            List<Blizzard> debugBlizzards = new List<Blizzard>();

            while (unwind.Count > 0)
            {
                blah = unwind.Pop();

                Console.WriteLine($" Round {blah.RoundNumber} :");
                Console.WriteLine();

                Console.Write('#');
                Console.Write(blah.Position == s_start ? '@' : '.');

                for (int x = 0; x < xMax; x++)
                {
                    Console.Write('#');
                }

                Console.WriteLine();

                for (int y = 0; y < yMax; y++)
                {
                    Console.Write('#');

                    for (int x = 0; x < xMax; x++)
                    {
                        Point target = new Point(x, y);

                        debugBlizzards.Clear();
                        debugBlizzards.AddRange(allBlizzardStates[blah.RoundNumber % s_stateMax].Where(b => b.Position == target));

                        if (blah.Position == target)
                        {
                            Console.Write(debugBlizzards.Count > 0 ? '!' : '@');
                        }
                        else if (debugBlizzards.Count > 9)
                        {
                            Console.Write('*');
                        }
                        else if (debugBlizzards.Count > 1)
                        {
                            Console.Write(debugBlizzards.Count);
                        }
                        else if (debugBlizzards.Count == 1)
                        {
                            Console.Write(debugBlizzards[0].Direction);
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }

                    Console.WriteLine('#');
                }

                for (int x = 0; x < xMax; x++)
                {
                    Console.Write('#');
                }

                Console.Write(blah.Position == s_end ? '@' : '.');
                Console.Write('#');

                Console.WriteLine();
            }
#endif

            Console.WriteLine(answer.Value.RoundNumber);
        }

        internal static void Problem2()
        {
            (List<Blizzard> blizzards, int xMax, int yMax) = Load();
            s_xMax = xMax;
            s_yMax = yMax;
            s_stateMax = xMax * yMax;
            s_end = new Point(xMax - 1, s_yMax);
            List<Blizzard>[] allBlizzardStates = new List<Blizzard>[s_stateMax];

            allBlizzardStates[0] = blizzards;
            List<Blizzard> prev = blizzards;

            HashSet<Point>[] occupiedSpaces = new HashSet<Point>[s_stateMax];

            for (int i = 1; i < s_stateMax; i++)
            {
                List<Blizzard> cur = new List<Blizzard>(prev.Count);

                foreach (Blizzard b in prev)
                {
                    cur.Add(b.BlizzardInNextSpace(xMax, yMax));
                }

                allBlizzardStates[i] = cur;
                prev = cur;
            }

            for (int i = 0; i < s_stateMax; i++)
            {
                HashSet<Point> cur = new HashSet<Point>();

                foreach (Blizzard b in allBlizzardStates[i])
                {
                    cur.Add(b.Position);
                }

                occupiedSpaces[i] = cur;
            }


            Dictionary<State, State> parentTracker = null;
#if SAMPLE
            parentTracker = new Dictionary<State, State>();
#endif

            (bool Found, State Value) answer = Pathing.BreadthFirstSearch(
                occupiedSpaces,
                new State { Position = new Point(0, -1), RoundNumber = 0 },
                s => s.Position == s_end && s.Phase == 2,
                NextStates,
                parentTracker: parentTracker);


            if (!answer.Found)
            {
                throw new InvalidOperationException("No solution");
            }

#if SAMPLE
            Stack<State> unwind = new Stack<State>();
            State blah = answer.Value;
            unwind.Push(blah);

            while (parentTracker.TryGetValue(blah, out blah))
            {
                unwind.Push(blah);
            }

            List<Blizzard> debugBlizzards = new List<Blizzard>();
            List<State> forwardPath = new List<State>(unwind);

            while (unwind.Count > 0)
            {
                blah = unwind.Pop();

                Console.WriteLine();
                Console.WriteLine($"Round {blah.RoundNumber}:");

                Console.Write('#');
                Console.Write(blah.Position == s_start ? '@' : '.');

                for (int x = 0; x < xMax; x++)
                {
                    Console.Write('#');
                }

                Console.WriteLine();

                for (int y = 0; y < yMax; y++)
                {
                    Console.Write('#');

                    for (int x = 0; x < xMax; x++)
                    {
                        Point target = new Point(x, y);

                        debugBlizzards.Clear();
                        debugBlizzards.AddRange(allBlizzardStates[blah.RoundNumber % s_stateMax].Where(b => b.Position == target));

                        if (blah.Position == target)
                        {
                            Console.Write(debugBlizzards.Count > 0 ? '!' : '@');
                        }
                        else if (debugBlizzards.Count > 9)
                        {
                            Console.Write('*');
                        }
                        else if (debugBlizzards.Count > 1)
                        {
                            Console.Write(debugBlizzards.Count);
                        }
                        else if (debugBlizzards.Count == 1)
                        {
                            Console.Write(debugBlizzards[0].Direction);
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }

                    Console.WriteLine('#');
                }

                for (int x = 0; x < xMax; x++)
                {
                    Console.Write('#');
                }

                Console.Write(blah.Position == s_end ? '@' : '.');
                Console.Write('#');

                Console.WriteLine();
            }
#endif

            Console.WriteLine(answer.Value.RoundNumber);
        }

        private static IEnumerable<State> NextStates(State state, HashSet<Point>[] occupiedPoints)
        {
            Point a = state.Position;
            int phase = state.Phase;

            if (a == s_end)
            {
                if (state.Phase == 2)
                {
                    yield break;
                }

                if (state.Phase == 0)
                {
                    phase = 1;
                }
            }

            if (state.Phase == 1 && a == s_start)
            {
                phase = 2;
            }

            Point n = state.Position.North();
            Point s = state.Position.South();
            
            int nextRound = state.RoundNumber + 1;
            HashSet<Point> occupied = occupiedPoints[nextRound % s_stateMax];

            if (!occupied.Contains(a))
            {
                yield return new State { Position = a, RoundNumber = nextRound, Phase = phase };
            }

            if (!occupied.Contains(n))
            {
                if (n.Y >= 0 || n == s_start)
                {
                    yield return new State { Position = n, RoundNumber = nextRound, Phase = phase };
                }
            }

            if (!occupied.Contains(s))
            {
                if (s.Y < s_yMax || s == s_end)
                {
                    yield return new State { Position = s, RoundNumber = nextRound, Phase = phase };
                }
            }

            if (a.Y >= 0 && a.Y < s_yMax)
            {
                Point e = state.Position.East();
                Point w = state.Position.West();

                if (!occupied.Contains(e))
                {
                    if (e.X < s_xMax)
                    {
                        yield return new State { Position = e, RoundNumber = nextRound, Phase = phase };
                    }
                }

                if (!occupied.Contains(w))
                {
                    if (w.X >= 0)
                    {
                        yield return new State { Position = w, RoundNumber = nextRound, Phase = phase };
                    }
                }
            }
        }
    }
}
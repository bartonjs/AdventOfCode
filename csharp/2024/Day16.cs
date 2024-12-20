using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day16
    {
        private class World : RoutableWorld<State>
        {
            private readonly bool _combineTurnAndStep;
            private readonly Point _start;
            private readonly Point _end;

            public World(DynamicPlane<char> world, Point start, Point end, bool combineTurnAndStep)
                : base(world)
            {
                _combineTurnAndStep = combineTurnAndStep;
                _start = start;
                _end = end;

                SetCustomEquality(static (a, b) => a.Position.Equals(b.Position));
            }

            internal State InitialState => new State { Position = _start, Facing = Directions2D.East };
            internal State FinalState => new State { Position = _end, Facing = Point.AllDirections };

            protected override long EstimateCost(State candidate, State end, DynamicPlane<char> world)
            {
                return candidate.Position.ManhattanDistance(end.Position);
            }

            protected override IEnumerable<(State Neighbor, long Cost)> GetNeighbors(State from, DynamicPlane<char> world)
            {
                return from.Neighbors(world, _combineTurnAndStep);
            }

            public long FindPathCost() => FindPathCost(InitialState, FinalState);
        }

        private static (DynamicPlane<char> World, Point Start, Point End) Load()
        {
            return Plane.LoadCharPlane(Data.Enumerate(), 'S', 'E');
        }

        private struct State : IEquatable<State>
        {
            internal Point Position;
            internal Directions2D Facing;

            internal IEnumerable<(State, long)> Neighbors(DynamicPlane<char> world, bool combineTurnAndStep = false)
            {
                foreach ((State state, long cost) in MaybeNeighbors(combineTurnAndStep))
                {
                    if (world.TryGetValue(state.Position, out char value) && value != '#')
                    {
                        yield return (state, cost);
                    }
                }
            }

            private IEnumerable<(State, long)> MaybeNeighbors(bool combineTurnAndStep)
            {
                yield return (new State
                {
                    Position = Position.GetNeighbor(Facing),
                    Facing = Facing,
                }, 1);

                Directions2D next = Facing.TurnLeft();

                if (!combineTurnAndStep)
                {
                    yield return (new State
                    {
                        Position = Position,
                        Facing = next,
                    }, 1000);
                }
                else
                {
                    yield return (new State
                    {
                        Position = Position.GetNeighbor(next),
                        Facing = next,
                    }, 1001);
                }

                next = Facing.TurnRight();

                if (!combineTurnAndStep)
                {
                    yield return (new State
                    {
                        Position = Position,
                        Facing = next,
                    }, 1000);
                }
                else
                {
                    yield return (new State
                    {
                        Position = Position.GetNeighbor(next),
                        Facing = next,
                    }, 1001);
                }
            }
            
            public State FreeReverseDirection()
            {
                return new State
                {
                    Position = Position,
                    Facing = Facing.TurnLeft().TurnLeft(),
                };
            }

            public bool Equals(State other)
            {
                return Position.Equals(other.Position) && Facing == other.Facing;
            }

            public override bool Equals(object obj)
            {
                return obj is State other && Equals(other);
            }

            public override int GetHashCode()
            {
                return Position.GetHashCode();
            }
        }

        internal static void Problem1()
        {
            (DynamicPlane<char> plane, Point start, Point end) = Load();
            World world = new World(plane, start, end, true);

            Console.WriteLine(world.FindPathCost(world.InitialState, world.FinalState));
        }

        internal static void Problem2()
        {
            (DynamicPlane<char> plane, Point start, Point end) = Load();
            World world = new World(plane, start, end, false);

            List<State> path = new();
            HashSet<Point> allPoints = new HashSet<Point>();
            Dictionary<State, long> gScore = new();
            long minCost = 0;

            world.TryFindCoveringSpaces(world.InitialState, world.FinalState, gScore, out long cost, path);
            Console.WriteLine($"Best path had cost {cost} and length {path.Count}");

            Queue<State> queue = new Queue<State>();

            foreach (var kvp in gScore)
            {
                if (kvp.Value == cost)
                {
                    queue.Enqueue(kvp.Key);
                    allPoints.Add(kvp.Key.Position);
                }
            }

            while (queue.TryDequeue(out State cur))
            {
                long curCost = gScore[cur];

                foreach ((State next, long increment) in cur.FreeReverseDirection().Neighbors(plane))
                {
                    State aboutFace = next.FreeReverseDirection();

                    if (gScore.TryGetValue(aboutFace, out long nextScore))
                    {
                        if (nextScore + increment == curCost)
                        {
                            queue.Enqueue(aboutFace);
                            allPoints.Add(aboutFace.Position);
                        }
                    }
                }
            }

            HashSet<Point> exciting = new HashSet<Point>(allPoints);
            exciting.RemoveWhere(p => path.Any(s => s.Position == p));

            PrintBestPaths(plane, allPoints, exciting);

            List<List<State>> allPaths = AllPaths(gScore, plane, cost);

            Console.WriteLine($"Found {allPaths.Count} different path(s).");

#if SAMPLE
            if (allPaths.Count > 1)
            {
                for (int i = 0; i < allPaths.Count; i++)
                {
                    Console.WriteLine($"Path {i+1}:");
                    PrintPath(plane, allPaths[i]);
                }
            }
#endif

            Console.WriteLine(allPoints.Count);
        }

        private static List<List<State>> AllPaths(Dictionary<State, long> gScore, DynamicPlane<char> world, long cost)
        {
            List<List<State>> allPaths = new();

            Stack<State> curPathRev = new();

            foreach (var kvp in gScore)
            {
                if (kvp.Value == cost)
                {
                    curPathRev.Push(kvp.Key);
                    BuildPath(gScore, curPathRev, world, allPaths);
                    curPathRev.Pop();
                }
            }

            return allPaths;

            static void BuildPath(
                Dictionary<State, long> gScore,
                Stack<State> curPathRev,
                DynamicPlane<char> world,
                List<List<State>> allPaths)
            {
                State cur = curPathRev.Peek();
                int depth = curPathRev.Count;
                Span<State> nextStates = stackalloc State[3];

                try
                {
                    int count = 0;

                    while (true)
                    {
                        long curCost = gScore[cur];

                        if (curCost == 0)
                        {
                            allPaths.Add(curPathRev.ToList());
                            return;
                        }

                        count = 0;

                        foreach ((State next, long increment) in cur.FreeReverseDirection().Neighbors(world))
                        {
                            State aboutFace = next.FreeReverseDirection();

                            if (gScore.TryGetValue(aboutFace, out long nextScore))
                            {
                                if (nextScore + increment == curCost)
                                {
                                    nextStates[count] = aboutFace;
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
                        BuildPath(gScore, curPathRev, world, allPaths);
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

        //[Conditional("SAMPLE")]
        private static void PrintBestPaths(
            DynamicPlane<char> world,
            HashSet<Point> allPoints,
            HashSet<Point> excitingPoints)
        {
            const ConsoleColor DefaultColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = DefaultColor;

            for (int row = 0; row < world.Height; row++)
            {
                for (int col = 0; col < world.Width; col++)
                {
                    Point point = new Point(col, row);

                    if (excitingPoints.Contains(point))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('@');
                        Console.ForegroundColor = DefaultColor;
                    }
                    else if (allPoints.Contains(point))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('!');
                        Console.ForegroundColor = DefaultColor;
                    }
                    else
                    {
                        Console.Write(world[point]);
                    }
                }

                Console.WriteLine();
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        [Conditional("SAMPLE")]
        private static void PrintPath(DynamicPlane<char> world, IEnumerable<State> path)
        {
            HashSet<Point> allPoints = new(path.Select(p => p.Position));

            for (int row = 0; row < world.Height; row++)
            {
                for (int col = 0; col < world.Width; col++)
                {
                    Point point = new Point(col, row);

                    if (allPoints.Contains(point))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('!');
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(world[point]);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day16
    {
        private static (DynamicPlane<char> world, Point start, Point end) Load()
        {
            DynamicPlane<char> world = null;
            Point start = default;
            Point end = default;
            int row = 0;

            foreach (string s in Data.Enumerate())
            {
                char[] array = s.ToCharArray();

                int sIdx = array.AsSpan().IndexOf('S');
                int eIdx = array.AsSpan().IndexOf('E');

                if (sIdx >= 0)
                {
                    start = new Point(sIdx, row);
                }

                if (eIdx >= 0)
                {
                    end = new Point(eIdx, row);
                }

                if (world is null)
                {
                    world = new DynamicPlane<char>(array);
                }
                else
                {
                    world.PushY(array);
                }

                row++;
            }

            return (world, start, end);
        }

        private struct State : IEquatable<State>
        {
            internal Point Position;
            internal Directions2D Facing;

            internal IEnumerable<(State, long)> Neighbors(DynamicPlane<char> world, bool turnWithoutMoving)
            {
                foreach ((State state, long cost) in MaybeNeighbors(turnWithoutMoving))
                {
                    if (world.TryGetValue(state.Position, out char value) && value != '#')
                    {
                        yield return (state, cost);
                    }
                }
            }

            internal IEnumerable<(State, long)> Neighbors(DynamicPlane<char> world, List<State> forks)
            {
                int multi = 0;

                foreach ((State state, long cost) in MaybeNeighbors())
                {
                    if (world.TryGetValue(state.Position, out char value) && value != '#')
                    {
                        if (multi == 1)
                        {
                            forks.Add(this);
                        }

                        yield return (state, cost);
                        multi++;
                    }
                }
            }

            internal IEnumerable<(State, long)> Neighbors(DynamicPlane<char> world)
            {
                foreach ((State state, long cost) in MaybeNeighbors())
                {
                    if (world.TryGetValue(state.Position, out char value) && value != '#')
                    {
                        yield return (state, cost);
                    }
                }
            }

            private IEnumerable<(State, long)> MaybeNeighbors(bool turnWithoutMoving = false)
            {
                yield return (new State
                {
                    Position = Position.GetNeighbor(Facing),
                    Facing = Facing,
                }, 1);

                Directions2D next = TurnLeft(Facing);

                if (turnWithoutMoving)
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

                next = TurnRight(Facing);

                if (turnWithoutMoving)
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

            static Directions2D TurnRight(Directions2D direction)
            {
                return direction switch
                {
                    Directions2D.North => Directions2D.East,
                    Directions2D.East => Directions2D.South,
                    Directions2D.South => Directions2D.West,
                    Directions2D.West => Directions2D.North,
                };
            }

            static Directions2D TurnLeft(Directions2D direction)
            {
                return direction switch
                {
                    Directions2D.North => Directions2D.West,
                    Directions2D.East => Directions2D.North,
                    Directions2D.South => Directions2D.East,
                    Directions2D.West => Directions2D.South,
                };
            }

            public State FreeReverseDirection()
            {
                return new State
                {
                    Position = Position,
                    Facing = TurnLeft(TurnLeft(Facing)),
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
            (DynamicPlane<char> world, Point start, Point end) = Load();

            State initial = new State
            {
                Position = start,
                Facing = Directions2D.East,
            };

            State final = new State
            {
                Position = end,
                Facing = Point.AllDirections,
            };

            long cost = Pathing.AStar(
                world,
                initial,
                final,
                static (position, world) => position.Neighbors(world),
                static (candidate, position, world) => position.Position.ManhattanDistance(candidate.Position),
                customEquals: (s1, s2) => s1.Position.Equals(s2.Position));

            Console.WriteLine(cost);
        }

        internal static void Problem2()
        {
            (DynamicPlane<char> world, Point start, Point end) = Load();

            State initial = new State
            {
                Position = start,
                Facing = Directions2D.East,
            };

            State final = new State
            {
                Position = end,
                Facing = Point.AllDirections,
            };

            List<State> forks = new();
            List<State> path = new();
            HashSet<Point> allPoints = new HashSet<Point>();
            Dictionary<State, long> gScore = new();

            long minCost = 0;

            long cost = Pathing.AStar(
                (world, forks),
                initial,
                final,
                static (position, world) => position.Neighbors(world.world, true),
                static (candidate, position, world) => position.Position.ManhattanDistance(candidate.Position),
                path,
                gScore,
                customEquals: (s1, s2) => s1.Position.Equals(s2.Position),
                allPaths: true);

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

                foreach ((State next, long increment) in cur.FreeReverseDirection().Neighbors(world))
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

            PrintBestPaths(world, allPoints, exciting);

            List<List<State>> allPaths = AllPaths(gScore, world, cost);

            Console.WriteLine($"Found {allPaths.Count} different path(s).");

#if SAMPLE
            if (allPaths.Count > 1)
            {
                for (int i = 0; i < allPaths.Count; i++)
                {
                    Console.WriteLine($"Path {i+1}:");
                    PrintPath(world, allPaths[i]);
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

                        foreach ((State next, long increment) in cur.FreeReverseDirection().Neighbors(world, true))
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
            for (int row = 0; row < world.Height; row++)
            {
                for (int col = 0; col < world.Width; col++)
                {
                    Point point = new Point(col, row);

                    if (excitingPoints.Contains(point))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('@');
                        Console.ResetColor();
                    }
                    else if (allPoints.Contains(point))
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
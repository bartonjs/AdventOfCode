using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

            private IEnumerable<(State, long)> MaybeNeighbors()
            {
                yield return (new State
                {
                    Position = Position.GetNeighbor(Facing),
                    Facing = Facing,
                }, 1);

                Directions2D next = TurnLeft(Facing);

                yield return (new State
                {
                    Position = Position.GetNeighbor(next),
                    Facing = next,
                }, 1001);

                next = TurnRight(Facing);

                yield return (new State
                {
                    Position = Position.GetNeighbor(next),
                    Facing = next,
                }, 1001);
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

            public bool Equals(State other)
            {
                return Position.Equals(other.Position) && (Facing & other.Facing) != 0;
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
                static (candidate, position, world) => position.Position.ManhattanDistance(candidate.Position));

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

            long minCost = 0;

            long cost = Pathing.AStar(
                (world, forks),
                initial,
                final,
                static (position, world) => position.Neighbors(world.world, world.forks),
                static (candidate, position, world) => position.Position.ManhattanDistance(candidate.Position),
                path);

            Console.WriteLine($"Best path had cost {cost} and length {path.Count}");

            foreach (State s in path)
            {
                allPoints.Add(s.Position);
            }
            
            PrintBestPaths(world, allPoints, new HashSet<Point>());
            RemoveAndRetry(forks, path, world, initial, final, cost, allPoints);

            Console.WriteLine(allPoints.Count);
        }

        private static HashSet<string> s_knownStates;

        private static void RemoveAndRetry(
            List<State> forks,
            List<State> path,
            DynamicPlane<char> world,
            State initial,
            State final,
            long cost,
            HashSet<Point> allPoints,
            int recursion = 1)
        {
            if (s_knownStates is null)
            {
                s_knownStates = new HashSet<string>();
                s_knownStates = new HashSet<string>(new WorldComparer(world.Width));
            }

            if (!s_knownStates.GetAlternateLookup<DynamicPlane<char>>().Add(world))
            {
                return;
            }

            HashSet<Point> pathPoints = new HashSet<Point>(path.Select(p => p.Position));
            List<Point> interestingPoints = new List<Point>();

            for (int i = 0; i < forks.Count; i++)
            {
                foreach ((State s, _) in forks[i].Neighbors(world))
                {
                    if (pathPoints.Contains(s.Position))
                    {
                        interestingPoints.Add(s.Position);
                    }
                }
            }

            Utils.TraceForSample($"Found {interestingPoints.Count} interesting points.");

            for (var i = 0; i < interestingPoints.Count; i++)
            {
                var p = interestingPoints[i];
                Utils.TraceForSample($"Best path contained fork point {p}... blocking it.");

                char prev = world[p];
                world[p] = '#';

                List<State> newForks = new();
                List<State> newPath = new();

                long newCost = Pathing.AStar(
                    (world, newForks),
                    initial,
                    final,
                    static (position, world) => position.Neighbors(world.world, world.newForks),
                    static (candidate, position, world) => position.Position.ManhattanDistance(candidate.Position),
                    newPath);

                Utils.TraceForSample($"New cost was {newCost} and new path was {newPath.Count} long");

                if (newCost == cost)
                {
                    int before = allPoints.Count;
                    HashSet<Point> excitingPoints = null;
#if SAMPLE
                    excitingPoints = new();
#endif

                    foreach (State s2 in newPath)
                    {
                        if (allPoints.Add(s2.Position))
                        {
#if SAMPLE
                            excitingPoints.Add(s2.Position);
#endif
                        }
                    }

                    int after = allPoints.Count;

                    if (after != before)
                    {
                        Console.WriteLine(
                            $"{DateTime.Now:HH:mm:ss.fff}: Found {after - before} new point(s) at recursion={recursion} on {i}/{interestingPoints.Count} (total={after}), cost is {newCost}");

                        PrintPath(world, newPath);
                        PrintBestPaths(world, allPoints, excitingPoints);
                    }

                    RemoveAndRetry(newForks, newPath, world, initial, final, cost, allPoints, recursion: recursion + 1);
                }

                world[p] = prev;
            }
        }

        [Conditional("SAMPLE")]
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
                        Console.Write('@');
                    }
                    else if (allPoints.Contains(point))
                    {
                        Console.Write('!');
                    }
                    else
                    {
                        Console.Write(world[point]);
                    }
                }

                Console.WriteLine();
            }
        }

        [Conditional("SAMPLE")]
        private static void PrintPath(DynamicPlane<char> world, List<State> path)
        {
            HashSet<Point> allPoints = new(path.Select(p => p.Position));

            for (int row = 0; row < world.Height; row++)
            {
                for (int col = 0; col < world.Width; col++)
                {
                    Point point = new Point(col, row);

                    if (allPoints.Contains(point))
                    {
                        Console.Write('!');
                    }
                    else
                    {
                        Console.Write(world[point]);
                    }
                }

                Console.WriteLine();
            }
        }

        private sealed class WorldComparer : IEqualityComparer<string>,
            IAlternateEqualityComparer<DynamicPlane<char>, string>
        {
            private readonly int _width;

            public WorldComparer(int width)
            {
                _width = width;
            }

            public bool Equals(string x, string y)
            {
                return string.Equals(x, y);
            }

            public int GetHashCode(string obj)
            {
                HashCode hc = new HashCode();

                for (int i = 0; i < obj.Length; i += _width)
                {
                    hc.AddBytes(MemoryMarshal.AsBytes(obj.AsSpan(i, int.Min(_width, obj.Length - i))));
                }

                return hc.ToHashCode();
            }

            public bool Equals(DynamicPlane<char> alternate, string other)
            {
                if (other?.Length != alternate.Width * alternate.Height)
                {
                    return false;
                }

                ReadOnlySpan<char> otherSpan = other;

                foreach (char[] row in alternate.AllRows())
                {
                    if (!row.AsSpan().SequenceEqual(otherSpan.Slice(0, row.Length)))
                    {
                        return false;
                    }

                    otherSpan = otherSpan.Slice(row.Length);
                }

                return true;
            }

            public int GetHashCode(DynamicPlane<char> alternate)
            {
                HashCode hc = new HashCode();

                foreach (char[] row in alternate.AllRows())
                {
                    hc.AddBytes(MemoryMarshal.AsBytes(row.AsSpan()));
                }

                return hc.ToHashCode();
            }

            public string Create(DynamicPlane<char> alternate)
            {
                string ret = string.Create(
                    alternate.Height * alternate.Width,
                    alternate,
                    static (span, alt) =>
                    {
                        foreach (char[] row in alt.AllRows())
                        {
                            row.AsSpan().CopyTo(span);
                            span = span.Slice(row.Length);
                        }
                    });

                return ret;
            }
        }
    }
}
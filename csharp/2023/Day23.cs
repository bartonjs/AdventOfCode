using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day23
    {
        internal static void Problem1()
        {
            (DynamicPlane<char> plane, Point start, Point end) = Load();
            long ret = 0;

            Pathing.BreadthFirstSearch(
                plane,
                new State(start),
                state =>
                {
                    ret = long.Max(ret, state.Visited.Count);
                    return false;
                },
                Neighbors);

            Console.WriteLine(ret);
        }

        private static IEnumerable<State> Neighbors(State state, DynamicPlane<char> world)
        {
            Point cur = state.Point;

            Directions2D directions = world[cur] switch
            {
                '^' => Directions2D.North,
                'v' => Directions2D.South,
                '>' => Directions2D.East,
                '<' => Directions2D.West,
                '.' => Point.AllDirections,
            };
                    
            foreach (Point p in cur.GetNeighbors(directions))
            {
                if (!state.Visited.Contains(p) && world.TryGetValue(p, out char c) && c != '#')
                {
                    yield return state.MoveTo(p);
                }
            }
        }

        private static Directions2D Neighbors2(DynamicPlane<char> world, Point point)
        {
            Directions2D neighbors = Directions2D.None;

            Directions2D check = Directions2D.North;

            while (check < Point.AllDirections)
            {
                Point p = point.GetNeighbor(check);

                if (world.TryGetValue(p, out char c) && c != '#')
                {
                    neighbors |= check;
                }

                check = (Directions2D)((int)check << 1);
            }

            return neighbors;
        }

        internal static void Problem2()
        {
            (DynamicPlane<char> plane, Point start, Point end) = Load();
            Dictionary<Point, List<(Point Neighbor, int Cost)>> junctions = new();

            foreach (Point p in plane.AllPoints())
            {
                if (plane[p] != '#')
                {
                    if (int.PopCount((int)Neighbors2(plane, p)) != 2)
                    {
                        junctions[p] = new();
                    }
                }
            }

            HashSet<Point> visited = new();

            foreach ((Point jctPoint, var list) in junctions)
            {
                Directions2D dirs = Neighbors2(plane, jctPoint);

                foreach (Point neighbor in jctPoint.GetNeighbors(dirs))
                {
                    visited.Clear();
                    visited.Add(neighbor);
                    visited.Add(jctPoint);
                    list.Add(GetPathLength(neighbor));

                    (Point, int) GetPathLength(Point point)
                    {
                        Point next = OnlyNeighbor(point);

                        while (!junctions.ContainsKey(next))
                        {
                            next = OnlyNeighbor(next);
                        }

                        return (next, visited.Count - 1);
                    }

                    Point OnlyNeighbor(Point point)
                    {
                        foreach (Point neighbor in point.GetNeighbors(Point.AllDirections))
                        {
                            if (!plane.TryGetValue(neighbor, out char c) || c == '#')
                            {
                                continue;
                            }

                            if (visited.Add(neighbor))
                            {
                                return neighbor;
                            }
                        }

                        throw new InvalidOperationException();
                    }
                }
            }

            visited.Clear();
            visited.Add(start);

            Console.WriteLine(FindLongestPath(start, end, junctions, visited, 0));

            static int FindLongestPath(
                Point start,
                Point end,
                Dictionary<Point, List<(Point Neighbor, int Cost)>> map,
                HashSet<Point> visited,
                int costSoFar)
            {
                if (start == end)
                {
                    return costSoFar;
                }

                var list = map[start];
                int max = 0;

                foreach (var kvp in list)
                {
                    if (!visited.Add(kvp.Neighbor))
                    {
                        continue;
                    }

                    max = int.Max(max, FindLongestPath(kvp.Neighbor, end, map, visited, costSoFar + kvp.Cost));
                    visited.Remove(kvp.Neighbor);
                }

                return max;
            }
        }

        private struct State : IEquatable<State>
        {
            public Point Point;
            public ImmutableSortedSet<Point> Visited;

            public State(Point point)
            {
                Point = point;
                Visited = ImmutableSortedSet<Point>.Empty;
            }

            public bool Equals(State other)
            {
                return Point.Equals(other.Point) && Visited.SequenceEqual(other.Visited);
            }

            public override bool Equals(object obj)
            {
                return obj is State other && Equals(other);
            }

            public override int GetHashCode()
            {
                HashCode hashCode = new HashCode();
                hashCode.Add(Point);

                foreach (Point p in Visited)
                {
                    hashCode.Add(p);
                }

                return hashCode.ToHashCode();
            }

            public State MoveTo(Point point)
            {
                return new State(point)
                {
                    Visited = Visited.Add(Point),
                };
            }
        }

        private static (DynamicPlane<char> Plane, Point Start, Point Exit) Load()
        {
            DynamicPlane<char> plane = null;
            Point start = default;
            int end = -1;

            int y = 0;

            foreach (string s in Data.Enumerate())
            {
                char[] chars = s.ToCharArray();

                if (plane is null)
                {
                    plane = new DynamicPlane<char>(chars);
                    start = new Point(s.IndexOf('.'), y);
                }
                else
                {
                    plane.PushY(chars);
                    end = s.IndexOf('.');
                }

                y++;
            }

            Debug.Assert(end >= 0);
            return (plane, start, new Point(end, y - 1));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day17
    {
        internal static void Problem1()
        {
            DynamicPlane<int> plane = null;

            foreach (string s in Data.Enumerate())
            {
                int[] row = s.Select(c => (int)c - (int)'0').ToArray();

                if (plane is null)
                {
                    plane = new DynamicPlane<int>(row);
                }
                else
                {
                    plane.PushY(row);
                }
            }

            List<Position> path = null;

#if SAMPLE
            path = new List<Position>();
#endif

            int min = int.MaxValue;

            for (int i = 1; i < 4; i++)
            {
                int ret = FindCheapestPath(
                    plane,
                    new Position(new(), new()),
                    new Position(new Point(plane.Width - 1, plane.Height - 1), new(0, i)),
                    path);

                min = int.Min(min, ret);

                ret = FindCheapestPath(
                    plane,
                    new Position(new(), new()),
                    new Position(new Point(plane.Width - 1, plane.Height - 1), new(i, 0)),
                    path);

                min = int.Min(min, ret);
            }

            Console.WriteLine(min);

            //int ret2 = 0;

            //if (path is not null)
            //{
            //    foreach (Position position in path)
            //    {
            //        Console.WriteLine($"{position.At} -- {position.Vector} -- {plane[position.At]}");
            //        ret2 += plane[position.At];
            //    }

            //    Console.WriteLine($"Path sum: {ret2}");
            //}
        }

        private struct Position : IEquatable<Position>
        {
            public Point At;
            public Point Vector;

            public Position(Point at, Point vector)
            {
                At = at;
                Vector = vector;
            }

            public bool Equals(Position other)
            {
                return At.Equals(other.At) && Vector.Equals(other.Vector);
            }

            public override bool Equals(object obj)
            {
                return obj is Position other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(At, Vector);
            }
        }

        private static int FindCheapestPath(Plane<int> grid, Position start, Position end, List<Position> route = null)
        {
            return Pathing.AStar(
                grid,
                start,
                end,
                Neighbors,
                static (candidate, end, world) => candidate.At.ManhattanDistance(end.At),
                pathToFill: route);

            static IEnumerable<(Position, int Cost)> Neighbors(Position from, Plane<int> world)
            {
                const int Limit = 3;

                Point plusX = new Point(from.At.X + 1, from.At.Y);
                Point minusX = new Point(from.At.X - 1, from.At.Y);
                Point plusY = new Point(from.At.X, from.At.Y + 1);
                Point minusY = new Point(from.At.X, from.At.Y - 1);
                int heat;

                if (from.Vector.East().X <= Limit &&
                    from.Vector.X >= 0 &&
                    world.TryGetValue(plusX, out heat))
                {
                    yield return (new Position(plusX, Normalize(from.Vector, 1, 0)), heat);
                }

                if (from.Vector.South().Y <= Limit &&
                    from.Vector.Y >= 0 &&
                    world.TryGetValue(plusY, out heat))
                {
                    yield return (new Position(plusY, Normalize(from.Vector, 0, 1)), heat);
                }

                if (from.Vector.West().X >= -Limit &&
                    from.Vector.X <= 0 &&
                    world.TryGetValue(minusX, out heat))
                {
                    yield return (new Position(minusX, Normalize(from.Vector, -1, 0)), heat);
                }

                if (from.Vector.North().Y >= -Limit &&
                    from.Vector.Y <= 0 &&
                    world.TryGetValue(minusY, out heat))
                {
                    yield return (new Position(minusY, Normalize(from.Vector, 0, -1)), heat);
                }
            }

            static Point Normalize(Point current, int x, int y)
            {
                if (x != 0)
                {
                    if (int.Sign(current.X) == int.Sign(x))
                    {
                        return new Point(current.X + x, current.Y);
                    }

                    return new Point(x, 0);
                }

                if (int.Sign(current.Y) == int.Sign(y))
                {
                    return new Point(current.X, current.Y + y);
                }

                return new Point(0, y);
            }
        }

        internal static void Problem2()
        {
            DynamicPlane<int> plane = null;

            foreach (string s in Data.Enumerate())
            {
                int[] row = s.Select(c => (int)c - (int)'0').ToArray();

                if (plane is null)
                {
                    plane = new DynamicPlane<int>(row);
                }
                else
                {
                    plane.PushY(row);
                }
            }

            List<Position> path = null;

#if SAMPLE
            path = new List<Position>();
#endif

            int min = int.MaxValue;

            for (int i = 4; i < 10; i++)
            {
                int ret = FindCheapestPath2(
                    plane,
                    new Position(new(), new()),
                    new Position(new Point(plane.Width - 1, plane.Height - 1), new(0, i)),
                    path);

                min = int.Min(min, ret);

                ret = FindCheapestPath2(
                    plane,
                    new Position(new(), new()),
                    new Position(new Point(plane.Width - 1, plane.Height - 1), new(i, 0)),
                    path);

                min = int.Min(min, ret);
            }

            Console.WriteLine(min);
        }

        private static int FindCheapestPath2(Plane<int> grid, Position start, Position end, List<Position> route = null)
        {
            return Pathing.AStar(
                grid,
                start,
                end,
                Neighbors,
                static (candidate, end, world) => candidate.At.ManhattanDistance(end.At),
                pathToFill: route);

            static IEnumerable<(Position, int Cost)> Neighbors(Position from, Plane<int> world)
            {
                const int Limit = 10;
                const int TurnLimit = 4;

                Point plusX = new Point(from.At.X + 1, from.At.Y);
                Point minusX = new Point(from.At.X - 1, from.At.Y);
                Point plusY = new Point(from.At.X, from.At.Y + 1);
                Point minusY = new Point(from.At.X, from.At.Y - 1);
                int heat;

                if (from.Vector == default)
                {
                    yield return (new Position(plusX, new Point(1, 0)), world[plusX]);
                    yield return (new Position(plusY, new Point(0, 1)), world[plusY]);
                    yield break;
                }

                if (from.Vector.X >= 0 &&
                    from.Vector.X < Limit &&
                    int.Abs(from.Vector.Y) is 0 or >= TurnLimit &&
                    world.TryGetValue(plusX, out heat))
                {
                    yield return (new Position(plusX, Normalize(from.Vector, 1, 0)), heat);
                }

                if (from.Vector.Y >= 0 &&
                    from.Vector.Y < Limit &&
                    int.Abs(from.Vector.X) is 0 or >= TurnLimit &&
                    world.TryGetValue(plusY, out heat))
                {
                    yield return (new Position(plusY, Normalize(from.Vector, 0, 1)), heat);
                }

                if (from.Vector.X <= 0 &&
                    from.Vector.X > -Limit &&
                    int.Abs(from.Vector.Y) is 0 or >= TurnLimit &&
                    world.TryGetValue(minusX, out heat))
                {
                    yield return (new Position(minusX, Normalize(from.Vector, -1, 0)), heat);
                }

                if (from.Vector.Y <= 0 &&
                    from.Vector.Y > -Limit &&
                    int.Abs(from.Vector.X) is 0 or >= TurnLimit &&
                    world.TryGetValue(minusY, out heat))
                {
                    yield return (new Position(minusY, Normalize(from.Vector, 0, -1)), heat);
                }
            }

            static Point Normalize(Point current, int x, int y)
            {
                if (x != 0)
                {
                    if (int.Sign(current.X) == int.Sign(x))
                    {
                        return new Point(current.X + x, current.Y);
                    }

                    return new Point(x, 0);
                }

                if (int.Sign(current.Y) == int.Sign(y))
                {
                    return new Point(current.X, current.Y + y);
                }

                return new Point(0, y);
            }
        }
    }
}
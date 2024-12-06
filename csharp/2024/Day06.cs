using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day06
    {
        internal static (Plane<char>, Point) Load()
        {
            DynamicPlane<char> plane = null;
            Point start = default;
            int row = 0;

            foreach (string s in Data.Enumerate())
            {
                if (plane is null)
                {
                    plane = new DynamicPlane<char>(s.Length);
                }

                int caret = s.IndexOf('^');

                if (caret >= 0)
                {
                    start = new Point(caret, row);
                }

                plane.PushY(s.ToCharArray());
                row++;
            }

            return (plane, start);
        }

        internal static void Problem1()
        {
            (Plane<char> world, Point pos) = Load();
            Directions2D facing = Directions2D.North;
            HashSet<Point> covered = new HashSet<Point> { pos };

            while (true)
            {
                Point next = pos.GetNeighbor(facing);

                if (!world.TryGetValue(next, out char value))
                {
                    break;
                }

                if (value == '#')
                {
                    facing = facing switch
                    {
                        Directions2D.North => Directions2D.East,
                        Directions2D.East => Directions2D.South,
                        Directions2D.South => Directions2D.West,
                        Directions2D.West => Directions2D.North,
                    };

                    continue;
                }

                pos = next;
                covered.Add(pos);
                Utils.TraceForSample($"Pos: {pos}, Facing: {facing}");
            }

            Console.WriteLine(covered.Count);
        }

        internal static void Problem2x()
        {
            long ret = 0;
            (Plane<char> world, Point start) = Load();
            Point pos = start;
            Directions2D facing = Directions2D.North;
            HashSet<Point> covered = new HashSet<Point> { pos };

            while (true)
            {
                Point next = pos.GetNeighbor(facing);

                if (!world.TryGetValue(next, out char value))
                {
                    break;
                }

                if (value == '#')
                {
                    facing = facing switch
                    {
                        Directions2D.North => Directions2D.East,
                        Directions2D.East => Directions2D.South,
                        Directions2D.South => Directions2D.West,
                        Directions2D.West => Directions2D.North,
                    };

                    continue;
                }

                pos = next;
                covered.Add(pos);
                Utils.TraceForSample($"Pos: {pos}, Facing: {facing}");
            }

            foreach (Point candidate in covered)
            {
                if (candidate == start)
                {
                    continue;
                }

                char existing = world[candidate];
                world[candidate] = '#';
                
                if (InfiniteCycle(world, start, Directions2D.North))
                {
                    ret++;
                }

                world[candidate] = existing;

            }

            Console.WriteLine(ret);
        }

        private static bool InfiniteCycle(Plane<char> world, Point pos, Directions2D facing)
        {
            HashSet<(Point, Directions2D)> states = new();
            states.Add((pos, facing));

            while (true)
            {
                Point next = pos.GetNeighbor(facing);

                if (!world.TryGetValue(next, out char value))
                {
                    return false;
                }

                if (value == '#')
                {
                    facing = facing switch
                    {
                        Directions2D.North => Directions2D.East,
                        Directions2D.East => Directions2D.South,
                        Directions2D.South => Directions2D.West,
                        Directions2D.West => Directions2D.North,
                    };

                    continue;
                }

                pos = next;

                if (!states.Add((pos, facing)))
                {
                    return true;
                }
            }
        }
    }
}
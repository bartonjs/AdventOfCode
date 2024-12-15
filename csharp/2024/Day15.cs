using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdventOfCode.Util;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2024
{
    internal class Day15
    {
        private static (DynamicPlane<char> World, List<char> Moves, Point RobotLoc) Load(bool part2 = false)
        {
            DynamicPlane<char> world = null;
            List<char> moves = null;
            Point robotLoc = default;
            int row = 0;

            foreach (string s in Data.Enumerate())
            {
                if (world is null)
                {
                    if (part2)
                    {
                        world = new DynamicPlane<char>((s + s).ToCharArray());
                    }
                    else
                    {
                        world = new DynamicPlane<char>(s.ToCharArray());
                    }
                }
                else if (moves is not null)
                {
                    moves.AddRange(s);
                }
                else if (string.IsNullOrEmpty(s))
                {
                    moves = new List<char>();
                }
                else
                {
                    row++;
                    char[] charArray;

                    if (part2)
                    {
                        charArray = new char[2 * s.Length];

                        for (int i = 0; i < s.Length; i++)
                        {
                            char real = s[i];

                            if (real is '#' or '.')
                            {
                                charArray[2 * i] = charArray[2 * i + 1] = real;
                            }
                            else if (real is 'O')
                            {
                                charArray[2 * i] = '[';
                                charArray[2 * i + 1] = ']';
                            }
                            else
                            {
                                Debug.Assert(real == '@');
                                charArray[2 * i] = '@';
                                charArray[2 * i + 1] = '.';
                            }
                        }
                    }
                    else
                    {
                        charArray = s.ToCharArray();
                    }

                    if (robotLoc.Y == 0)
                    {
                        int idx = charArray.AsSpan().IndexOf('@');

                        if (idx >= 0)
                        {
                            robotLoc = new Point(idx, row);
                        }
                    }

                    world.PushY(charArray);
                }
            }

            return (world, moves, robotLoc);
        }

        internal static void Problem1()
        {
            (DynamicPlane<char> world, List<char> moves, Point robotLoc) = Load();

            foreach (char move in moves)
            {
                Directions2D direction = move switch
                {
                    '^' => Directions2D.North,
                    '>' => Directions2D.East,
                    '<' => Directions2D.West,
                    'v' => Directions2D.South,
                };

                Point next = robotLoc.GetNeighbor(direction);

                if (world[next] == '.')
                {
                    world[next] = '@';
                    world[robotLoc] = '.';
                    robotLoc = next;
                    continue;
                }

                if (world[next] == '#')
                {
                    continue;
                }

                Debug.Assert(world[next] == 'O');
                Point free = next;

                while (world[free] == 'O')
                {
                    free = free.GetNeighbor(direction);
                }

                if (world[free] == '.')
                {
                    world[free] = 'O';
                    world[next] = '@';
                    world[robotLoc] = '.';
                    robotLoc = next;
                }
            }

            long ret = 0;

            foreach (Point p in world.AllPoints())
            {
                if (world[p] == 'O')
                {
                    ret += p.Y * 100 + p.X;
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            (DynamicPlane<char> world, List<char> moves, Point robotLoc) = Load(true);

            foreach (char move in moves)
            {
                Directions2D direction = move switch
                {
                    '^' => Directions2D.North,
                    '>' => Directions2D.East,
                    '<' => Directions2D.West,
                    'v' => Directions2D.South,
                };

                Point next = robotLoc.GetNeighbor(direction);

                if (world[next] == '.')
                {
                    world[next] = '@';
                    world[robotLoc] = '.';
                    robotLoc = next;
                    continue;
                }

                if (world[next] == '#')
                {
                    continue;
                }

                if (CanMove(next, direction, world))
                {
                    Move(next, direction, world);
                    world[next] = '@';
                    world[robotLoc] = '.';
                    robotLoc = next;
                }
            }

            long ret = 0;

            foreach (Point p in world.AllPoints())
            {
                if (world[p] == '[')
                {
                    ret += p.Y * 100 + p.X;
                }
            }

            Console.WriteLine(ret);

            static bool CanMove(Point test, Directions2D direction, DynamicPlane<char> world, bool fromPair = false)
            {
                char here = world[test];

                if (here == '.')
                {
                    return true;
                }

                if (here == '#')
                {
                    return false;
                }

                if (here == ']')
                {
                    if (fromPair || CanMove(test.West(), direction, world, true))
                    {
                        if (direction == Directions2D.West)
                        {
                            return true;
                        }

                        return CanMove(test.GetNeighbor(direction), direction, world);
                    }

                    return false;
                }

                Debug.Assert(here == '[');

                if (fromPair || CanMove(test.East(), direction, world, true))
                {
                    if (direction == Directions2D.East)
                    {
                        return true;
                    }

                    return CanMove(test.GetNeighbor(direction), direction, world);
                }

                return false;
            }

            static void Move(Point loc, Directions2D direction, DynamicPlane<char> world, bool fromPair = false)
            {
                char here = world[loc];

                if (here == '.')
                {
                    return;
                }

                if (here == '#')
                {
                    throw new InvalidOperationException();
                }

                Point next = loc.GetNeighbor(direction);

                if (here == ']')
                {
                    if (!fromPair)
                    {
                        Move(loc.West(), direction, world, true);
                    }
                }
                else
                {
                    Debug.Assert(here == '[');

                    if (!fromPair)
                    {
                        Move(loc.East(), direction, world, true);
                    }
                }

                Move(next, direction, world);
                world[next] = here;
                world[loc] = '.';
            }
        }

        private static void PrintWorld(DynamicPlane<char> world) => Console.WriteLine(world.Print(c => c));
    }
}
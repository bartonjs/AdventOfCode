using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day16
    {
        internal static void Problem1()
        {
            Console.WriteLine(MetaProblem(new Beam()));
        }

        private static DynamicPlane<State> Load()
        {
            DynamicPlane<State> plane = null;

            foreach (string s in Data.Enumerate())
            {
                if (plane is null)
                {
                    plane = new DynamicPlane<State>(s.Select(c => new State(c)).ToArray());
                }
                else
                {
                    plane.PushY(s.Select(c => new State(c)).ToArray());
                }
            }

            return plane;
        }

        internal static void Problem2()
        {
            DynamicPlane<State> size = Load();
            int max = 0;

            for (int x = 0; x < size.Width; x++)
            {
                max = int.Max(max, MetaProblem(new Beam { Point = new Point(x, 0), Direction = Direction.South, }));
                max = int.Max(max,MetaProblem(new Beam { Point = new Point(x, size.Height), Direction = Direction.North, }));
            }

            for (int y = 0; y < size.Height; y++)
            {
                max = int.Max(max, MetaProblem(new Beam { Point = new Point(0, y), Direction = Direction.East, }));
                max = int.Max(max, MetaProblem(new Beam { Point = new Point(size.Width, y), Direction = Direction.West, }));
            }

            Console.WriteLine(max);
        }

        private static int MetaProblem(Beam startingBeam)
        {
            DynamicPlane<State> plane = Load();
            Queue<Beam> beams = new();
            beams.Enqueue(startingBeam);

            while (beams.Count > 0)
            {
                Beam beam = beams.Dequeue();

                while (plane.ContainsPoint(beam.Point))
                {
                    State state = plane[beam.Point];

                    if (state.EnergizeFrom(beam))
                    {
                        break;
                    }

                    switch (state.Char)
                    {
                        case '\\':
                        case '/':
                            beam.Rotate(state.Char);
                            break;
                        case '|':
                            if (beam.Direction is Direction.East or Direction.West)
                            {
                                beams.Enqueue(beam.Split());
                            }

                            break;
                        case '-':
                            if (beam.Direction is Direction.North or Direction.South)
                            {
                                beams.Enqueue(beam.Split());
                            }

                            break;
                        case '.':
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    beam.Move();
                }
            }

            int ret = 0;

            foreach (Point point in plane.AllPoints())
            {
                if (plane[point].Energized)
                {
                    ret++;
                }
            }

            return ret;
        }

        private class Beam
        {
            public Point Point;
            public Direction Direction;

            public Beam Split()
            {
                Direction = (Direction)(((int)Direction + 1) % 4);

                return new Beam
                {
                    Point = Point,
                    Direction = (Direction)(((int)Direction + 2) % 4),
                };
            }

            public void Move()
            {
                switch (Direction)
                {
                    case Direction.North:
                        Point = Point.North();
                        break;
                    case Direction.East:
                        Point = Point.East();
                        break;
                    case Direction.South:
                        Point = Point.South();
                        break;
                    case Direction.West:
                        Point = Point.West();
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public void Rotate(char type)
            {
                switch (type)
                {
                    case '\\':
                        Direction = Direction switch
                        {
                            Direction.North => Direction.West,
                            Direction.East => Direction.South,
                            Direction.South => Direction.East,
                            Direction.West => Direction.North,
                            _ => throw new InvalidOperationException(),
                        };

                        break;
                    case '/':
                        Direction = Direction switch
                        {
                            Direction.North => Direction.East,
                            Direction.East => Direction.North,
                            Direction.South => Direction.West,
                            Direction.West => Direction.South,
                            _ => throw new InvalidOperationException(),
                        };

                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private enum Direction
        {
            East,
            South,
            West,
            North,
        }

        private class State
        {
            public char Char;
            private int _energizedFrom;

            public State(char c)
            {
                Char = c;
            }

            public bool EnergizeFrom(Beam beam)
            {
                int test = 1 << (int)beam.Direction;

                if ((_energizedFrom & test) == 0)
                {
                    _energizedFrom |= test;
                    return false;
                }

                return true;
            }

            public bool Energized => _energizedFrom != 0;
        }
    }
}
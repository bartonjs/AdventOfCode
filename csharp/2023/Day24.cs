using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day24
    {
        internal static void Problem1()
        {
#if SAMPLE
            const double MinBound = 7;
            const double MaxBound = 27;
#else
            const double MinBound = 200000000000000;
            const double MaxBound = 400000000000000;
#endif

            List<Hailstone> hailstones = Load();
            int smash = 0;

            for (int i = 0; i < hailstones.Count; i++)
            {
                for (int j = i + 1; j < hailstones.Count; j++)
                {
                    (double x, double y) = hailstones[i].Intersect(hailstones[j]);

                    Utils.TraceForSample($"{hailstones[i]} intersects {hailstones[j]} at ({x}, {y})");

                    if (x >= MinBound && x <= MaxBound && y >= MinBound && y <= MaxBound)
                    {
                        Utils.TraceForSample("*** HIT ***");
                        smash++;
                    }
                }
            }

            Console.WriteLine(smash);
        }

        internal static void Problem2()
        {
            List<Hailstone3> hailstones = Load3();

            Console.Write("Solve[{");
            char[] tvars = { 't', 'u', 'v' };

            for (int i = 0; i < 3; i++)
            {
                Hailstone3 stone = hailstones[i];

                if (i > 0)
                {
                    Console.Write(", ");
                }

                Console.Write($"{tvars[i]} >= 0, ");

                if (stone.Velocity.X < 0)
                {
                    Console.Write($"{stone.Position.X} - {-stone.Velocity.X}{tvars[i]} == x + a * {tvars[i]}, ");
                }
                else
                {
                    Console.Write($"{stone.Position.X} + {stone.Velocity.X}{tvars[i]} == x + a * {tvars[i]}, ");
                }

                if (stone.Velocity.X < 0)
                {
                    Console.Write($"{stone.Position.Y} - {-stone.Velocity.Y}{tvars[i]} == y + b * {tvars[i]}, ");
                }
                else
                {
                    Console.Write($"{stone.Position.Y} + {stone.Velocity.Y}{tvars[i]} == y + b * {tvars[i]}, ");
                }

                if (stone.Velocity.X < 0)
                {
                    Console.Write($"{stone.Position.Z} - {-stone.Velocity.Z}{tvars[i]} == z + c * {tvars[i]}");
                }
                else
                {
                    Console.Write($"{stone.Position.Z} + {stone.Velocity.Z}{tvars[i]} == z + c * {tvars[i]}");
                }
            }

            Console.WriteLine("}, {x,y,z,a,b,c,t,u,v}]");
        }

        private static List<Hailstone> Load()
        {
            List<Hailstone> ret = new();

            foreach (string s in Data.Enumerate())
            {
                string[] posVel = s.Split('@');
                long[] pos = posVel[0].Split(',').Select(s => s.Trim()).Select(long.Parse).ToArray();
                long[] vel = posVel[1].Split(',').Select(s => s.Trim()).Select(long.Parse).ToArray();

                ret.Add(
                    new Hailstone
                    {
                        Position = new LongPoint(pos[0], pos[1]),
                        Velocity = new LongPoint(vel[0], vel[1]),
                    });
            }

            return ret;
        }

        private static List<Hailstone3> Load3()
        {
            List<Hailstone3> ret = new();

            foreach (string s in Data.Enumerate())
            {
                string[] posVel = s.Split('@');
                long[] pos = posVel[0].Split(',').Select(s => s.Trim()).Select(long.Parse).ToArray();
                long[] vel = posVel[1].Split(',').Select(s => s.Trim()).Select(long.Parse).ToArray();

                ret.Add(
                    new Hailstone3
                    {
                        Position = new LongPoint3(pos[0], pos[1], pos[2]),
                        Velocity = new LongPoint3(vel[0], vel[1], vel[2]),
                    });
            }

            return ret;
        }

        private class Hailstone3
        {
            private static int s_index;

            public int Id { get; }
            public LongPoint3 Position;
            public LongPoint3 Velocity;

            public Hailstone3()
            {
                Id = s_index;
                s_index++;
            }

            public override string ToString()
            {
                return $"ID:{Id} P:{Position} V:{Velocity}";
            }
        }

        private class Hailstone
        {
            private static int s_index;

            public int Id { get; }
            public LongPoint Position;
            public LongPoint Velocity;

            public Hailstone()
            {
                Id = s_index;
                s_index++;
            }

            public override string ToString()
            {
                return $"ID:{Id} P:{Position} V:{Velocity}";
            }

            public (double X, double Y) Intersect(Hailstone other)
            {
                double a1 = Velocity.Y;
                double a2 = other.Velocity.Y;
                double b1 = -Velocity.X;
                double b2 = -other.Velocity.X;
                
                double det = a1 * b2 - a2 * b1;

                if (det == 0)
                {
                    if (Position == other.Position)
                    {
                        return (Position.X, Position.Y);
                    }

                    return (double.PositiveInfinity, double.PositiveInfinity);
                }

                double c1 = a1 * Position.X + b1 * Position.Y;
                double c2 = a2 * other.Position.X + b2 * other.Position.Y;

                double x = (b2 * c1 - b1 * c2) / det;
                double y = (a1 * c2 - a2 * c1) / det;

                if (double.Sign(y - Position.Y) != long.Sign(Velocity.Y) ||
                    double.Sign(y - other.Position.Y) != long.Sign(other.Velocity.Y) ||
                    double.Sign(x - Position.X) != long.Sign(Velocity.X) ||
                    double.Sign(x - other.Position.X) != long.Sign(other.Velocity.X))
                {
                    return (double.NegativeInfinity, double.NegativeInfinity);
                }

                return (x, y);
            }
        }
    }
}
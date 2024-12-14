using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day14
    {
#if SAMPLE
        private const int WorldX = 11;
        private const int WorldY = 7;
#else
        private const int WorldX = 101;
        private const int WorldY = 103;
#endif

        private static IEnumerable<(Point Position, Point Velocity)> Load()
        {
            Regex regex = new Regex("^p=(-?\\d+),(-?\\d+) v=(-?\\d+),(-?\\d+)$");

            foreach (string s in Data.Enumerate())
            {
                Match m = regex.Match(s);

                if (!m.Success)
                {
                    throw new InvalidDataException();
                }

                yield return (
                    new Point(int.Parse(m.Groups[1].ValueSpan), int.Parse(m.Groups[2].ValueSpan)),
                    new Point(int.Parse(m.Groups[3].ValueSpan), int.Parse(m.Groups[4].ValueSpan))
                );
            }
        }
        internal static void Problem1()
        {
            long[] quadrants = new long[4];
            const int QuadrantX = WorldX / 2;
            const int QuadrantY = WorldY / 2;
            const int TopX = WorldX - QuadrantX;
            const int TopY = WorldY - QuadrantY;

            Utils.TraceForSample($"Quadrant 0: X=0..{QuadrantX-1}, Y=0..{QuadrantY-1}");
            Utils.TraceForSample($"Quadrant 1: X=0..{QuadrantX-1}, Y={TopY}..{WorldY-1}");
            Utils.TraceForSample($"Quadrant 2: X={TopX}..{WorldX-1}, Y=0..{QuadrantY-1}");
            Utils.TraceForSample($"Quadrant 3: X={TopX}..{WorldX-1}, Y={TopY}..{WorldY-1}");

            foreach ((Point position, Point velocity) in Load())
            {
                Point laterPos = GetRobotPosition(position, velocity, 100);
                Utils.TraceForSample($"Final position: {laterPos}");

                if (laterPos.X >= TopX)
                {
                    if (laterPos.Y >= TopY)
                    {
                        Utils.TraceForSample("  Quadrant 3");
                        quadrants[3]++;
                    }
                    else if (laterPos.Y < QuadrantY)
                    {
                        Utils.TraceForSample("  Quadrant 2");
                        quadrants[2]++;
                    }
                    else
                    {
                        Utils.TraceForSample("  No quadrant (midpoint Y)");
                    }
                }
                else if (laterPos.X < QuadrantX)
                {
                    if (laterPos.Y >= TopY)
                    {
                        Utils.TraceForSample("  Quadrant 1");
                        quadrants[1]++;
                    }
                    else if (laterPos.Y < QuadrantY)
                    {
                        Utils.TraceForSample("  Quadrant 0");
                        quadrants[0]++;
                    }
                    else
                    {
                        Utils.TraceForSample("  No quadrant (midpoint Y)");
                    }
                }
                else
                {
                    Utils.TraceForSample("  No quadrant (midpoint X)");
                }
            }

            long ret = 1;

            foreach (long val in quadrants)
            {
                ret *= val;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            List<Point> positions = new List<Point>();
            List<Point> velocities = new List<Point>();

            foreach ((Point position, Point velocity) in Load())
            {
                positions.Add(position);
                velocities.Add(velocity);
            }

            int i = 0;
            Point borderStart = new Point(24, 31);
            Point borderStartRight = new Point(54, 31);
            Point borderStop = new Point(24, 63);
            Point borderStopRight = new Point(54, 63);

            StringBuilder line = new StringBuilder();

            //using StreamWriter writer = new StreamWriter(File.Open("output.txt", FileMode.Truncate, FileAccess.Write, FileShare.Read);
            TextWriter writer = Console.Out;
            
            while (i < WorldY * WorldX)
            {
                if (positions.Any(p => p == borderStart) && positions.Any(p => p == borderStartRight) &&
                    positions.Any(p => p == borderStop) && positions.Any(p => p == borderStopRight))
                {
                    writer.WriteLine(i);

                    bool[,] world = new bool[WorldX, WorldY];

                    foreach (Point position in positions)
                    {
                        world[position.X, position.Y] = true;
                    }

                    for (int row = 0; row < WorldY; row++)
                    {
                        line.Clear();

                        for (int col = 0; col < WorldX; col++)
                        {
                            line.Append(world[col, row] ? '@' : ' ');
                        }

                        writer.WriteLine(line);
                    }

                    writer.WriteLine();
                }

                MoveRobots(ref i);
            }

            void MoveRobots(ref int i)
            {
                for (int robot = 0; robot < positions.Count; robot++)
                {
                    Point nowPos = positions[robot];
                    Point velocity = velocities[robot];

                    int newX = (nowPos.X + velocity.X + WorldX) % WorldX;
                    int newY = (nowPos.Y + velocity.Y + WorldY) % WorldY;
                    positions[robot] = new Point(newX, newY);
                }

                i++;
            }
        }

        private static Point GetRobotPosition(Point position, Point velocity, long time)
        {
            long lcmX = Utils.LeastCommonMultiple(velocity.X, (long)WorldX);
            long lcmY = Utils.LeastCommonMultiple(velocity.Y, (long)WorldY);
            long lcm = Utils.LeastCommonMultiple(lcmX, lcmY);

            long iterTime = time % lcm;
            Point nowPos = position;

            for (long i = 0; i < iterTime; i++)
            {
                int newX = (nowPos.X + velocity.X + WorldX) % WorldX;
                int newY = (nowPos.Y + velocity.Y + WorldY) % WorldY;
                nowPos = new Point(newX, newY);
            }

            return nowPos;
        }
    }
}
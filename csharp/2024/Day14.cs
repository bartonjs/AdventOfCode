using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            StringBuilder line = new StringBuilder();

            Span<bool> world = new bool[WorldX * WorldY];
            ReadOnlySpan<bool> borderStart = [false, true, true, true, true, true, true, true];
            ReadOnlySpan<bool> FT = [false, true];
            ReadOnlySpan<bool> TF = [true, false];

            while (i < WorldY * WorldX)
            {
                world.Clear();

                foreach (Point position in positions)
                {
                    world[position.Y * WorldX + position.X] = true;
                }

                for (int row = 0; row < WorldY; row++)
                {
                    ReadOnlySpan<bool> rowSpan = world.Slice(row * WorldX, WorldX);
                    int idx = rowSpan.IndexOf(borderStart);

                    if (idx >= 0)
                    {
                        int before = idx;
                        idx++;
                        int len = rowSpan.Slice(idx).IndexOf(false);
                        int last = len - 1;

                        int checkRow = row + 1;
                        ReadOnlySpan<bool> checkSlice = world.Slice(checkRow * WorldX, WorldX);
                        ReadOnlySpan<bool> leftCheck = checkSlice.Slice(before, 2);
                        ReadOnlySpan<bool> rightCheck = checkSlice.Slice(idx + last, 2);

                        while (leftCheck.SequenceEqual(FT) && rightCheck.SequenceEqual(TF))
                        {
                            if (checkSlice.Slice(idx, len).IndexOf(false) == -1)
                            {
                                PrintWorld(world, line);
                                Console.WriteLine(i);
                                return;
                            }

                            checkRow++;
                            checkSlice = world.Slice(checkRow * WorldX, WorldX);
                            leftCheck = checkSlice.Slice(before, 2);
                            rightCheck = checkSlice.Slice(idx + last, 2);
                        }
                    }
                }

                MoveRobots(positions, velocities, ref i);
            }

            static void MoveRobots(List<Point> positions, List<Point> velocities, ref int i)
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

        private static void PrintWorld(ReadOnlySpan<bool> world, StringBuilder builder)
        {
            builder.Clear();

            for (int i = 0, col = 0; i < world.Length; i++, col++)
            {
                builder.Append(world[i] ? '@' : ' ');

                if (col == WorldX - 1)
                {
                    Console.WriteLine(builder);
                    builder.Clear();
                    col = -1;
                }
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
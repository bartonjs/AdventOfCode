using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day15
    {
        [GeneratedRegex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")]
        private static partial Regex MatchRegex();

        private record struct Sensor(Point Point, int Distance)
        {
            internal int X => Point.X;
            internal int Y => Point.Y;

            internal Sensor(int x, int y, int distance)
                : this(new Point(x, y), distance)
            {
            }
        }

        private static (List<Sensor> sensors, HashSet<Point> beacons) LoadGrid()
        {
            Regex matcher = MatchRegex();
            List<Sensor> sensors = new List<Sensor>();
            HashSet<Point> beacons = new HashSet<Point>();

            foreach (string s in Data.Enumerate())
            {
                Match m = matcher.Match(s);

                int sensorX = int.Parse(m.Groups[1].ValueSpan);
                int sensorY = int.Parse(m.Groups[2].ValueSpan);
                int beaconX = int.Parse(m.Groups[3].ValueSpan);
                int beaconY = int.Parse(m.Groups[4].ValueSpan);

                int sensorDistance = Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY);

                sensors.Add(new Sensor(sensorX, sensorY, sensorDistance));
                beacons.Add(new Point(beaconX, beaconY));
            }

            return (sensors, beacons);
        }

        internal static void Problem1()
        {
            HashSet<int> points = new HashSet<int>();
            (List<Sensor> sensors, HashSet<Point> beacons) = LoadGrid();

#if SAMPLE
            const int TargetY = 10;
#else
            const int TargetY = 2000000;
#endif

            foreach (var sensor in sensors)
            {
                for (int i = 0; i < sensor.Distance; i++)
                {
                    int yPlus = sensor.Y + i;
                    int yMinus = sensor.Y - i;

                    if (yPlus == TargetY || yMinus == TargetY)
                    {
                        int delta = sensor.Distance - i;

                        for (int j = 0; j <= delta; j++)
                        {
                            points.Add(sensor.X + j);
                            points.Add(sensor.X - j);
                        }
                    }
                }
            }

            foreach (var beacon in beacons)
            {
                if (beacon.Y == TargetY)
                {
                    points.Remove(beacon.X);
                }
            }

            Console.WriteLine(points.Count);
#if SAMPLE
            Console.WriteLine(string.Join(", ", points.OrderBy(x => x)));
#endif
        }

        internal static void Problem2()
        {
            const int Scale = 4000000;

            (List<Sensor> sensors, HashSet<Point> beacons) = LoadGrid();

            for (int outer = 0; outer < sensors.Count; outer++)
            {
                Sensor reference = sensors[outer];

                int x = reference.X;
                int y = reference.Y - reference.Distance - 1;

                while (y < reference.Y)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x++;
                    y++;
                }


                while (x > reference.X)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x--;
                    y++;
                }

                while (y > reference.Y)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x--;
                    y--;
                }

                while (x < reference.X)
                {
                    if (Check(reference, x, y, sensors))
                    {
                        return;
                    }

                    x++;
                    y--;
                }

                Debug.Assert(x == reference.X);
            }

            static bool Check(Sensor sensor, int x, int y, List<Sensor> sensors)
            {
#if SAMPLE
                const int Size = 20;
#else
                const int Size = 4000000;
#endif
                if (x >= 0 && x <= Size && y >= 0 && y <= Size)
                {
                    if (!sensors.Any(s => Touches(s, x, y)))
                    {
                        Console.WriteLine(
                            $"Empty spot at ({x}, {y})  => {(long)x * Scale + y}");
                        return true;
                    }
                }

                return false;
            }

            static bool Touches(Sensor sensor, int x, int y)
            {
                int distance = Math.Abs(sensor.X - x) + Math.Abs(sensor.Y - y);
                return distance <= sensor.Distance;
            }
        }

        internal static void Problem2BruteForce()
        {
#if SAMPLE
            const int Size = 20;
#else
            const int Size = 4000000;
#endif
            const int Scale = 4000000;

            (List<Sensor> sensors, HashSet<Point> beacons) = LoadGrid();
            List<Thread> threads = new List<Thread>();

            int sliceSize = Size / Math.Max(1, Environment.ProcessorCount - 1);

            for (int cpu = 0; cpu < Environment.ProcessorCount; cpu++)
            {
                Thread t = new Thread(
                    id =>
                    {
                        int tid = (int)id;
                        int start = sliceSize * tid;
                        int stop = Math.Min(start + sliceSize, Size);
                        bool[] row = new bool[Size + 1];

                        Console.WriteLine($"Thread {tid} scanning {start}-{stop}");

                        for (int targetY = start; targetY <= stop; targetY++)
                        {
                            Array.Clear(row);

                            foreach (var sensor in sensors)
                            {
                                int yDiff = Math.Abs(sensor.Y - targetY);

                                int delta = sensor.Distance - yDiff;

                                if (delta >= 0)
                                {
                                    int xMin = Math.Max(sensor.X - delta, 0);
                                    int xMax = Math.Min(sensor.X + delta, Size);
                                    row.AsSpan(xMin, xMax - xMin + 1).Fill(true);
                                }
                            }

                            int firstFalse = row.AsSpan().IndexOf(false);

                            if (firstFalse >= 0)
                            {
                                Console.WriteLine(
                                    $"Empty spot at ({firstFalse}, {targetY}) => {(long)firstFalse * Scale + targetY}");
                            }

                            if ((targetY % 10000) == 0)
                            {
                                Console.WriteLine($"{tid}:{targetY}");
                            }

                        }

                        Console.WriteLine($"Thread {tid} is done.");
                    });

                t.Start(cpu);
                threads.Add(t);
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }
        }
    }
}
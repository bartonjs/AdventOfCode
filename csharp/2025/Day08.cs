using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day08
    {
        private static List<Point3WithId> Load()
        {
            List<Point3WithId> points = new();
            int nextId = 0;

            foreach (string s in Data.Enumerate())
            {
                int comma1 = s.IndexOf(',');
                int comma2 = s.IndexOf(',', comma1 + 1);

                points.Add(
                    new Point3WithId(
                        int.Parse(s.AsSpan(0, comma1)),
                        int.Parse(s.AsSpan(comma1 + 1, comma2 - comma1 - 1)),
                        int.Parse(s.AsSpan(comma2 + 1)),
                        nextId));

                nextId++;
            }

            return points;
        }

        internal static void Problem1()
        {
            const int Iters =
#if SAMPLE
                10;
#else
                1000;
#endif

            List<Point3WithId> points = Load();
            List<(Point3WithId A, Point3WithId B, double Distance)> pairs = new(points.Count * points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                Point3WithId a = points[i];

                for (int j = i + 1; j < points.Count; j++)
                {
                    Point3WithId b = points[j];

                    pairs.Add((a, b, a.Point.EuclidianDistance(b.Point)));
                }
            }

            pairs.Sort((x, y) => double.Sign(x.Distance - y.Distance));

            int[] circuits = new int[points.Count];
            int nextCircuit = 1;

            for (int i = 0; i < Iters; i++)
            {
                (Point3WithId a, Point3WithId b, _) = pairs[i];
                int circuitA = circuits[a.Id];
                int circuitB = circuits[b.Id];

                if (circuitA != 0)
                {
                    if (circuitB != 0)
                    {
                        if (circuitB != circuitA)
                        {
                            Utils.TraceForSample($"Merge {circuitB} => {circuitA}");
                            Reassign(circuits, circuitB, circuitA);
                        }
                    }
                    else
                    {
                        Utils.TraceForSample($"Assigning {b.Id} to existing circuit {circuitA}");
                        circuits[b.Id] = circuitA;
                    }
                }
                else if (circuitB != 0)
                {
                    Utils.TraceForSample($"Assigning {a.Id} to existing circuit {circuitB}");
                    circuits[a.Id] = circuitB;
                }
                else
                {
                    Utils.TraceForSample($"Forming circuit {nextCircuit} from {a.Id} and {b.Id}");
                    circuits[a.Id] = nextCircuit;
                    circuits[b.Id] = nextCircuit;
                    nextCircuit++;
                }
            }

            long ret = 1;

            foreach (var kvp in circuits.Where(x => x != 0).CountBy(x => x).OrderByDescending(kvp => kvp.Value).Take(3))
            {
                //Console.WriteLine($"Found a circuit with {kvp.Value} members");
                ret *= kvp.Value;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            List<Point3> points = Load().Select(p => p.Point).ToList();
            List<(Point3 A, Point3 B, double Distance)> pairs = new(points.Count * points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                Point3 a = points[i];

                for (int j = i + 1; j < points.Count; j++)
                {
                    Point3 b = points[j];

                    pairs.Add((a, b, a.EuclidianDistance(b)));
                }
            }

            pairs.Sort((x, y) => double.Sign(x.Distance - y.Distance));

            Dictionary<Point3, CircuitAssignment> circuits = new();
            int nextCircuit = 0;

            foreach (Point3 point in points)
            {
                circuits[point] = new CircuitAssignment(nextCircuit);
                nextCircuit++;
            }

            int circuitCount = nextCircuit;

            foreach ((Point3 a, Point3 b, _) in pairs)
            {
                if (circuits.TryGetValue(a, out CircuitAssignment circuitA))
                {
                    if (circuits.TryGetValue(b, out CircuitAssignment circuitB))
                    {
                        if (circuitA.Num != circuitB.Num)
                        {
                            if (circuitCount == 2)
                            {
                                long ret = a.X;
                                ret *= b.X;
                                Console.WriteLine(ret);
                                return;
                            }

                            int from = circuitB.Num;
                            int to = circuitA.Num;

                            if (from < to)
                            {
                                (from, to) = (to, from);
                            }

                            foreach (var kvp in circuits)
                            {
                                if (kvp.Value.Num == from)
                                {
                                    kvp.Value.Num = to;
                                }
                            }

                            circuitCount--;
                        }
                    }
                    else
                    {
                        circuits[b] = circuitA;
                    }
                }
                else if (circuits.TryGetValue(b, out CircuitAssignment circuitB))
                {
                    circuits[a] = circuitB;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            Console.WriteLine("No solution");
        }

        private static void Reassign(int[] circuits, int from, int to)
        {
            for (int i = circuits.Length - 1; i >= 0; i--)
            {
                if (circuits[i] == from)
                {
                    circuits[i] = to;
                }
            }
        }

        private class CircuitAssignment
        {
            internal int Num;

            internal CircuitAssignment(int num)
            {
                Num = num;
            }
        }

        private struct Point3WithId
        {
            public readonly Point3 Point;
            public readonly int Id;

            public Point3WithId(int x, int y, int z, int id)
            {
                Point = new Point3(x, y, z);
                Id = id;
            }
        }
    }
}

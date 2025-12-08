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
            List<DistancePair> pairs = new(points.Count * points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                Point3WithId a = points[i];

                for (int j = i + 1; j < points.Count; j++)
                {
                    Point3WithId b = points[j];

                    pairs.Add(new DistancePair(a, b));
                }
            }

            pairs.Sort();

            int[] circuits = new int[points.Count];
            int nextCircuit = 1;

            for (int i = 0; i < Iters; i++)
            {
                (Point3WithId a, Point3WithId b) = pairs[i];
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
            List<Point3WithId> points = Load();
            List<DistancePair> pairs = new(points.Count * points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                Point3WithId a = points[i];

                for (int j = i + 1; j < points.Count; j++)
                {
                    Point3WithId b = points[j];

                    pairs.Add(new DistancePair(a, b));
                }
            }

            pairs.Sort();

            int[] circuits = new int[points.Count];

            for (int i = circuits.Length - 1; i >= 0; i--)
            {
                circuits[i] = i;
            }

            int circuitCount = circuits.Length;

            foreach ((Point3WithId a, Point3WithId b) in pairs)
            {
                int circuitA = circuits[a.Id];
                int circuitB = circuits[b.Id];

                if (circuitB != circuitA)
                {
                    Utils.TraceForSample($"Merge {circuitB} => {circuitA} ({circuitCount} total)");

                    if (circuitCount == 2)
                    {
                        long ret = a.Point.X;
                        ret *= b.Point.X;
                        Console.WriteLine(ret);
                        return;
                    }

                    Reassign(circuits, circuitB, circuitA);
                    circuitCount--;
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

        private readonly struct DistancePair : IComparable<DistancePair>
        {
            private readonly Point3WithId A;
            private readonly Point3WithId B;
            private readonly double Dist;

            internal DistancePair(Point3WithId a, Point3WithId b)
            {
                A = a;
                B = b;
                Dist = a.Point.EuclideanDistance(b.Point);
            }

            internal void Deconstruct(out Point3WithId a, out Point3WithId b)
            {
                a = A;
                b = B;
            }

            public int CompareTo(DistancePair other)
            {
                return Dist.CompareTo(other.Dist);
            }
        }
    }
}

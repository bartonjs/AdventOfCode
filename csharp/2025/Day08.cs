using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day08
    {
        internal static List<Point3> Load()
        {
            List<Point3> points = new();

            foreach (string s in Data.Enumerate())
            {
                int comma1 = s.IndexOf(',');
                int comma2 = s.IndexOf(',', comma1 + 1);

                points.Add(
                    new Point3(
                        int.Parse(s.AsSpan(0, comma1)),
                        int.Parse(s.AsSpan(comma1 + 1, comma2 - comma1 - 1)),
                        int.Parse(s.AsSpan(comma2 + 1))));
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

            List<Point3> points = Load();
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

            for (int i = 0; i < Iters; i++)
            {
                (Point3 a, Point3 b, _) = pairs[i];

                if (circuits.TryGetValue(a, out CircuitAssignment circuitA))
                {
                    if (circuits.TryGetValue(b, out CircuitAssignment circuitB))
                    {
                        if (circuitA.Num != circuitB.Num)
                        {
                            int from = circuitB.Num;
                            int to = circuitA.Num;

                            foreach (var kvp in circuits)
                            {
                                if (kvp.Value.Num == from)
                                {
                                    kvp.Value.Num = to;
                                }
                            }

                            circuitB.Num = circuitA.Num;
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
                    CircuitAssignment assign = new CircuitAssignment(nextCircuit);
                    nextCircuit++;
                    circuits[a] = assign;
                    circuits[b] = assign;
                }
            }

            var orderedPairs = circuits.GroupBy(kvp => kvp.Value.Num).Select(gr => (gr, gr.Count())).OrderByDescending(p => p.Item2);
            long ret = 1;

            foreach (var pair in orderedPairs.Take(3))
            {
                long count = pair.Item2;
                ret *= count;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            List<Point3> points = Load();
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

        private class CircuitAssignment
        {
            internal int Num;

            internal CircuitAssignment(int num)
            {
                Num = num;
            }
        }
    }
}

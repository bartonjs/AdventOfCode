using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day19Abandoned
    {
        internal class Coord3
        {
            public int X { get; }
            public int Y { get; }
            public int Z { get; }

            public Coord3(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static Coord3 operator -(Coord3 left, Coord3 right)
            {
                return new Coord3(
                    left.X - right.X,
                    left.Y - right.Y,
                    left.Z - right.Z);
            }

            public override string ToString()
            {
                return $"(X={X}, Y={Y}, Z={Z})";
            }

            public double Magnitude => Math.Sqrt((double)X * X + (double)Y * Y + (double)Z * Z);
        }

        internal static void Problem1()
        {
            List<List<Coord3>> scannerData = new List<List<Coord3>>();

            {
                List<Coord3> cur = null;

                foreach (string line in Data.Enumerate())
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        cur = null;
                        continue;
                    }

                    if (line[0] == '-' && line[1] == '-')
                    {
                        cur = new List<Coord3>();
                        scannerData.Add(cur);
                        continue;
                    }

                    int comma1 = line.IndexOf(',');
                    int comma2 = line.IndexOf(',', comma1 + 1);

                    Coord3 coord = new Coord3(
                        int.Parse(line.AsSpan(0, comma1)),
                        int.Parse(line.AsSpan(comma1 + 1, comma2 - comma1 - 1)),
                        int.Parse(line.AsSpan(comma2 + 1)));

                    cur.Add(coord);
                }
            }

            var matches = FindMatches(scannerData[0], scannerData[1]);

            foreach (var match in matches)
            {
                Console.WriteLine($"A {match.Item1}->{match.Item2}={match.Item3} // B {match.Item4}->{match.Item5}={match.Item6}");
            }

            List<(int ScannerA, int ProbeA, int ScannerB, int ProbeB)> matchedProbes = new();
            HashSet<(int, int)> matchedOut = new HashSet<(int, int)>();

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];

                if (!matchedOut.Contains((0, match.Item1)))
                {
                    int countA = matches.Skip(i + 1).Count(m => (m.Item1 == match.Item1 || m.Item2 == match.Item1) && (m.Item4 == match.Item4 || m.Item5 == match.Item4));
                    int countB = matches.Skip(i + 1).Count(m => (m.Item1 == match.Item1 || m.Item2 == match.Item1) && (m.Item4 == match.Item5 || m.Item5 == match.Item5));

                    if (countA > countB)
                    {
                        matchedProbes.Add((0, match.Item1, 1, match.Item4));
                        matchedOut.Add((0, match.Item1));
                        matchedOut.Add((1, match.Item4));
                    }
                    else
                    {
                        matchedProbes.Add((0, match.Item1, 1, match.Item5));
                        matchedOut.Add((0, match.Item1));
                        matchedOut.Add((1, match.Item5));
                    }
                }

                if (!matchedOut.Contains((0, match.Item2)))
                {
                    int countA = matches.Skip(i + 1).Count(m => (m.Item1 == match.Item2 || m.Item2 == match.Item2) && (m.Item4 == match.Item4 || m.Item5 == match.Item4));
                    int countB = matches.Skip(i + 1).Count(m => (m.Item1 == match.Item2 || m.Item2 == match.Item2) && (m.Item4 == match.Item5 || m.Item5 == match.Item5));

                    if (countA > countB)
                    {
                        matchedProbes.Add((0, match.Item2, 1, match.Item4));
                        matchedOut.Add((0, match.Item2));
                        matchedOut.Add((1, match.Item4));
                    }
                    else
                    {
                        matchedProbes.Add((0, match.Item2, 1, match.Item5));
                        matchedOut.Add((0, match.Item2));
                        matchedOut.Add((1, match.Item5));
                    }
                }
            }

            foreach (var match in matchedProbes)
            {
                Console.WriteLine($"Scanner {match.ScannerA} Probe {match.ProbeA} is the same as Scanner {match.ScannerB} Probe {match.ProbeB}");
            }
        }

        private static List<(int, int, Coord3, int, int, Coord3)> FindMatches(
            List<Coord3> scannerA,
            List<Coord3> scannerB)
        {
            List<(int, int, Coord3)> s0r = new List<(int, int, Coord3)>();
            List<(int, int, Coord3)> s1r = new List<(int, int, Coord3)>();

            for (int i = 0; i < scannerA.Count; i++)
            {
                for (int j = i + 1; j < scannerA.Count; j++)
                {
                    Coord3 rel = scannerA[i] - scannerA[j];

                    //if (!rel.AnyOver1000)
                    {
                        s0r.Add((i, j, rel));
                    }
                }
            }

            for (int i = 0; i < scannerB.Count; i++)
            {
                for (int j = i + 1; j < scannerB.Count; j++)
                {
                    Coord3 rel = scannerB[i] - scannerB[j];

                    //if (!rel.AnyOver1000)
                    {
                        s1r.Add((i, j, rel));
                    }
                }
            }

            var s0re = s0r.OrderBy(x => x.Item3.Magnitude).GetEnumerator();
            var s1re = s1r.OrderBy(x => x.Item3.Magnitude).GetEnumerator();

            s0re.MoveNext();
            s1re.MoveNext();

            List<(int, int, Coord3, int, int, Coord3)> matches = new List<(int, int, Coord3, int, int, Coord3)>();

            while (s0re.MoveNext() && s1re.MoveNext())
            {
                bool moved = false;

                do
                {
                    moved = false;

                    while (s1re.Current.Item3.Magnitude - s0re.Current.Item3.Magnitude > 0.0001 )
                    {
                        moved = true;

                        if (!s0re.MoveNext())
                        {
                            return matches;
                        }
                    }

                    while (s0re.Current.Item3.Magnitude - s1re.Current.Item3.Magnitude  > 0.0001)
                    {
                        moved = true;

                        if (!s1re.MoveNext())
                        {
                            return matches;
                        }
                    }
                } while (moved);


                var s0 = s0re.Current;
                var s1 = s1re.Current;

                matches.Add((s0.Item1, s0.Item2, s0.Item3, s1.Item1, s1.Item2, s1.Item3));
            }

            return matches;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day19
    {
        internal static void Problem1()
        {
            List<List<Scanner>> scannerData = LoadData();
            List<Scanner> scanners = OrientScanners(scannerData);

            HashSet<Coord3> coords = new HashSet<Coord3>();

            foreach (Scanner scanner in scanners)
            {
                coords.UnionWith(scanner.AbsoluteBeacons);
                Console.WriteLine(
                    $"After merging in {scanner.RelativeBeacons.Count} points from scanner {scanner.Id} there are {coords.Count} points known.");
            }
        }

        internal static void Problem2()
        {
            List<List<Scanner>> scannerData = LoadData();
            List<Scanner> scanners = OrientScanners(scannerData);
            scanners[0] = scanners[0] with { Position = new Coord3(0, 0, 0), };

            int maxDistance = int.MinValue;

            for (int i = 0; i < scanners.Count; i++)
            {
                for (int j = i + 1; j < scanners.Count; j++)
                {
                    int manhattan = scanners[i].Position.ManhattanDistance(scanners[j].Position);

                    maxDistance = Math.Max(manhattan, maxDistance);
                }
            }

            Console.WriteLine(maxDistance);
        }

        private static List<List<Scanner>> LoadData()
        {
            List<List<Scanner>> scannerData = new();
            List<Coord3> cur = null;
            int scannerId = 0;

            foreach (string line in Data.Enumerate())
            {
                if (string.IsNullOrEmpty(line))
                {
                    scannerData.Add(Scanner.CreateOrientations(scannerId, cur).ToList());
                    scannerId++;
                    cur = null;
                    continue;
                }

                if (line[0] == '-' && line[1] == '-')
                {
                    cur = new List<Coord3>();
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

            scannerData.Add(Scanner.CreateOrientations(scannerId, cur).ToList());
            return scannerData;
        }

        private static List<Scanner> OrientScanners(List<List<Scanner>> scannerData)
        {
            List<Scanner> scanners = new List<Scanner>(scannerData.Count);

            scanners.Add(scannerData[0][0]);

            for (int i = 1; i < scannerData.Count; i++)
            {
                Scanner matched = AlignAnyOrientation(scanners[0], scannerData[i]).SingleOrDefault();
                scanners.Add(matched);

                if (matched is not null)
                {
                    Console.WriteLine($"Merged in scanner {i}.");
                }
            }

            bool needWork = false;

            do
            {
                bool didWork = false;
                needWork = false;

                for (int i = 1; i < scannerData.Count; i++)
                {
                    if (scanners[i] is null)
                    {
                        needWork = true;

                        for (int j = 0; j < scannerData.Count; j++)
                        {
                            if (j == i)
                            {
                                continue;
                            }

                            Scanner target = scanners[j];

                            if (target is not null)
                            {
                                Scanner matched = AlignAnyOrientation(target, scannerData[i]).SingleOrDefault();

                                if (matched is not null)
                                {
                                    didWork = true;
                                    Console.WriteLine($"Merged in scanner {i}.  ({scanners.Count(x => x is not null)}/{scanners.Count})");
                                    scanners[i] = matched;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (needWork && !didWork)
                {
                    throw new InvalidOperationException();
                }
            } while (needWork);

            return scanners;
        }

        // Code from here down is based on https://github.com/schovanec/AdventOfCode/blob/master/2021/Day19/Program.cs

        private static IEnumerable<Scanner> AlignAnyOrientation(Scanner target, IEnumerable<Scanner> scanners)
        {
            return scanners.SelectMany(x => AlignSingleOrientation(target, x)).Take(1);
        }

        private static IEnumerable<Scanner> AlignSingleOrientation(Scanner target, Scanner scanner)
        {
            foreach (Coord3 targetBeacon in target.AbsoluteBeacons)
            {
                foreach (Coord3 scannerBeacon in scanner.RelativeBeacons)
                {
                    Scanner moved = scanner with { Position = targetBeacon.Subtract(scannerBeacon) };

                    if (target.AbsoluteBeacons.Intersect(moved.AbsoluteBeacons).Skip(11).Any())
                    {
                        yield return moved;
                        yield break;
                    }
                }
            }
        }

        record Coord3(int X, int Y, int Z)
        {
            public Coord3 Subtract(Coord3 other)
                => new(X - other.X, Y - other.Y, Z - other.Z);

            public Coord3 Add(Coord3 other)
                => new(X + other.X, Y + other.Y, Z + other.Z);

            public int ManhattanDistance(Coord3 other)
                => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);

            public IEnumerable<Coord3> EnumFacingDirections()
            {
                var current = this;
                for (var i = 0; i < 3; ++i)
                {
                    yield return current;
                    yield return new(-current.X, -current.Y, current.Z);

                    current = new(current.Y, current.Z, current.X);
                }
            }

            public IEnumerable<Coord3> EnumRotations()
            {
                var current = this;
                for (var i = 0; i < 4; ++i)
                {
                    yield return current;
                    current = new(current.X, -current.Z, current.Y);
                }
            }

            public IEnumerable<Coord3> EnumOrientations()
                => EnumFacingDirections().SelectMany(v => v.EnumRotations());
        };

        record Scanner(int Id, ImmutableHashSet<Coord3> RelativeBeacons, Coord3 Position = default)
        {
            public IEnumerable<Coord3> AbsoluteBeacons
            {
                get
                {
                    if (Position == null)
                    {
                        return RelativeBeacons;
                    }

                    return RelativeBeacons.Select(v => v.Add(Position));
                }
            }

            public static IEnumerable<Scanner> CreateOrientations(int id, IEnumerable<Coord3> beacons)
            {
                return beacons.SelectMany(b => b.EnumOrientations().Select((v, i) => (index: i, vector: v)))
                    .GroupBy(v => v.index, g => g.vector)
                    .Select(g => new Scanner(id, g.ToImmutableHashSet()));
            }
        };
    }
}

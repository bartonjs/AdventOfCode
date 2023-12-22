using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day22
    {
        internal static void Problem1()
        {
            List<Brick> bricks = Load();
            bricks.Sort();

            List<Brick> done = new List<Brick>(bricks.Count);
            HashSet<Point3> currentPoints = new();
            Queue<Brick> toMove = new Queue<Brick>(bricks);

            while (toMove.Count > 0)
            {
                Brick brick = toMove.Dequeue();

                while (brick.BottomZ > 1 && !brick.Underneath().Any(currentPoints.Contains))
                {
                    brick.MoveDown();
                }

                Utils.TraceForSample($"Brick {brick.Id} stopped at Z={brick.BottomZ}");
                done.Add(brick);

                foreach (Point3 point in brick.AllPoints())
                {
                    bool added = currentPoints.Add(point);
                    Debug.Assert(added);
                }
            }

            HashSet<Brick> destroyable = new HashSet<Brick>(bricks);

            foreach (Brick brick in bricks)
            {
                List<Brick> supportsMe =
                    new(bricks.Where(b => b != brick && b.AllPoints().Any(brick.Underneath().Contains)));

                Utils.TraceForSample($"Brick {brick.Id} is supported by {string.Join(",", supportsMe.Select(b => b.Id))}");

                if (supportsMe.Count == 1)
                {
                    destroyable.Remove(supportsMe[0]);
                }
            }

            Console.WriteLine(destroyable.Count);
        }

        internal static void Problem2()
        {
            List<Brick> bricks = Load();
            bricks.Sort();

            List<Brick> done = new List<Brick>(bricks.Count);
            HashSet<Point3> currentPoints = new();
            Queue<Brick> toMove = new Queue<Brick>(bricks);

            while (toMove.Count > 0)
            {
                Brick brick = toMove.Dequeue();

                while (brick.BottomZ > 1 && !brick.Underneath().Any(currentPoints.Contains))
                {
                    brick.MoveDown();
                }

                Utils.TraceForSample($"Brick {brick.Id} stopped at Z={brick.BottomZ}");
                done.Add(brick);

                foreach (Point3 point in brick.AllPoints())
                {
                    bool added = currentPoints.Add(point);
                    Debug.Assert(added);
                }
            }

            currentPoints.Clear();
            int ret = 0;

            // There's probably some clever algorithm with memoization and dependency flows that does
            // this better... but 800ms is good enough for me.
            for (int i = 0; i < bricks.Count; i++)
            {
                int movedHere = 0;

                for (int j = 0; j < bricks.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    toMove.Enqueue(bricks[j].Clone());

                    while (toMove.Count > 0)
                    {
                        Brick brick = toMove.Dequeue();
                        bool moved = false;

                        while (brick.BottomZ > 1 && !brick.Underneath().Any(currentPoints.Contains))
                        {
                            moved = true;
                            brick.MoveDown();
                        }

                        done.Add(brick);

                        if (moved)
                        {
                            movedHere++;
                        }

                        foreach (Point3 point in brick.AllPoints())
                        {
                            bool added = currentPoints.Add(point);
                            Debug.Assert(added);
                        }
                    }
                }

                currentPoints.Clear();
                Utils.TraceForSample($"Removing brick {bricks[i].Id} (re)moved {movedHere} other bricks");
                ret += movedHere;
            }

            Console.WriteLine(ret);
        }

        private static List<Brick> Load()
        {
            List<Brick> ret = new();

            foreach (string s in Data.Enumerate())
            {
                string[] ends = s.Split('~');
                int[] parts = ends[0].Split(',').Select(int.Parse).ToArray();
                Point3 endA = new Point3(parts[0], parts[1], parts[2]);
                parts = ends[1].Split(',').Select(int.Parse).ToArray();
                Point3 endB = new Point3(parts[0], parts[1], parts[2]);

                ret.Add(new Brick(endA, endB));
            }

            return ret;
        }

        private class Brick : IComparable<Brick>
        {
            private static int s_count;

            private Point3 _endA;
            private Point3 _endB;

            public int Id { get; }
            public Point3 EndA => _endA;
            public Point3 EndB => _endB;

            public int BottomZ { get; private set; }

            public Brick(Point3 endA, Point3 endB)
            {
                _endA = endA;
                _endB = endB;
                
                int z = int.Min(endA.Z, endB.Z);
                BottomZ = z;

                Id = s_count;
                s_count++;
            }

            public IEnumerable<Point3> AllPoints()
            {
                int xMin = int.Min(EndA.X, EndB.X);
                int xMax = int.Max(EndA.X, EndB.X);
                int yMin = int.Min(EndA.Y, EndB.Y);
                int yMax = int.Max(EndA.Y, EndB.Y);
                int zMin = int.Min(EndA.Z, EndB.Z);
                int zMax = int.Max(EndA.Z, EndB.Z);

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        for (int z = zMin; z <= zMax; z++)
                        {
                            yield return new Point3(x, y, z);
                        }
                    }
                }
            }

            public IEnumerable<Point3> Underneath()
            {
                int xMin = int.Min(EndA.X, EndB.X);
                int xMax = int.Max(EndA.X, EndB.X);
                int yMin = int.Min(EndA.Y, EndB.Y);
                int yMax = int.Max(EndA.Y, EndB.Y);
                int z = BottomZ - 1;

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        yield return new Point3(x, y, z);
                    }
                }
            }

            public IEnumerable<Point3> Above()
            {
                int xMin = int.Min(EndA.X, EndB.X);
                int xMax = int.Max(EndA.X, EndB.X);
                int yMin = int.Min(EndA.Y, EndB.Y);
                int yMax = int.Max(EndA.Y, EndB.Y);
                int z = BottomZ + 1;

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        yield return new Point3(x, y, z);
                    }
                }
            }

            public int CompareTo(Brick other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;

                int compare = BottomZ.CompareTo(other.BottomZ);

                if (compare == 0)
                {
                    compare = EndA.X.CompareTo(other.EndA.X);
                }

                if (compare == 0)
                {
                    compare = EndA.Y.CompareTo(other.EndA.Y);
                }

                return compare;
            }

            public void MoveDown()
            {
                _endA.Z--;
                _endB.Z--;
                BottomZ--;
            }

            public Brick Clone()
            {
                return new Brick(_endA, _endB);
            }
        }
    }
}
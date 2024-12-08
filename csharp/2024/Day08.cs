using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day08
    {
        private static (DynamicPlane<char>, List<(char, Point)>) Load()
        {
            DynamicPlane<char> world = null;
            List<(char, Point)> antennae = new List<(char, Point)>();
            int row = 0;

            foreach (string s in Data.Enumerate())
            {
                char[] line = s.ToCharArray();

                for (int i = 0; i < line.Length; i++)
                {
                    if (s[i] != '.')
                    {
                        antennae.Add((s[i], new Point(i, row)));
                    }
                }

                if (world is null)
                {
                    world = new DynamicPlane<char>(line);
                }
                else
                {
                    world.PushY(line);
                }

                row++;
            }

            return (world, antennae);
        }

        internal static void Problem1()
        {
            (var world, var antennae) = Load();

            HashSet<Point> antinodes = new();

            for (var i = 0; i < antennae.Count; i++)
            {
                (char id, Point point) = antennae[i];

                for (int j = i + 1; j < antennae.Count; j++)
                {
                    if (antennae[j].Item1 == id)
                    {
                        Point jPoint = antennae[j].Item2;
                        int dx = jPoint.X - point.X;
                        int dy = jPoint.Y - point.Y;

                        Point candidate1 = new Point(point.X - dx, point.Y - dy);
                        Point candidate2 = new Point(jPoint.X + dx, jPoint.Y + dy);

                        if (world.ContainsPoint(candidate1))
                        {
                            antinodes.Add(candidate1);
                        }

                        if (world.ContainsPoint(candidate2))
                        {
                            antinodes.Add(candidate2);
                        }
                    }
                }
            }

            Console.WriteLine(antinodes.Count);
        }

        internal static void Problem2()
        {
            (var world, var antennae) = Load();

            HashSet<Point> antinodes = new();

            for (var i = 0; i < antennae.Count; i++)
            {
                (char id, Point point) = antennae[i];

                for (int j = i + 1; j < antennae.Count; j++)
                {
                    if (antennae[j].Item1 == id)
                    {
                        Point jPoint = antennae[j].Item2;
                        int dx = jPoint.X - point.X;
                        int dy = jPoint.Y - point.Y;

                        antinodes.Add(point);
                        antinodes.Add(jPoint);

                        Point candidate = new Point(point.X - dx, point.Y - dy);

                        while (world.ContainsPoint(candidate))
                        {
                            antinodes.Add(candidate);
                            candidate = new Point(candidate.X - dx, candidate.Y - dy);
                        }

                        candidate = new Point(jPoint.X + dx, jPoint.Y + dy);

                        while (world.ContainsPoint(candidate))
                        {
                            antinodes.Add(candidate);
                            candidate = new Point(candidate.X + dx, candidate.Y + dy);
                        }
                    }
                }
            }

            Console.WriteLine(antinodes.Count);
        }
    }
}
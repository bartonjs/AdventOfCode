using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;
using Microsoft.VisualBasic.CompilerServices;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day23
    {
        private static List<Elf> Load()
        {
            List<Elf> elves = new List<Elf>();
            int y = 0;

            foreach (string s in Data.Enumerate())
            {
                ReadOnlySpan<char> text = s;

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '#')
                    {
                        elves.Add(new Elf { Position = new Point(i, y) });
                    }
                }

                y++;
            }

            return elves;
        }

        private class Elf
        {
            public Point Position;
            public Point MoveTo;
            public bool CanMove;
        }

        private static Elf FindElf(List<Elf> elves, Point point)
        {
            return elves.FirstOrDefault(elf => elf.Position == point);
        }

        private static Elf FindAnyElf(List<Elf> elves, Point reference, int deltaX, int deltaY)
        {
            Point one;
            Point two;
            Point three;

            if (deltaX == 0)
            {
                one = new Point(reference.X - 1, reference.Y + deltaY);
                two = new Point(reference.X, reference.Y + deltaY);
                three = new Point(reference.X + 1, reference.Y + deltaY);
            }
            else
            {
                one = new Point(reference.X + deltaX, reference.Y - 1);
                two = new Point(reference.X + deltaX, reference.Y);
                three = new Point(reference.X + deltaX, reference.Y + 1);
            }

            return elves.FirstOrDefault(elf => elf.Position == one || elf.Position == two || elf.Position == three);
        }

        private static void FindAdjacentElves(List<Elf> elves, Point reference, List<Elf> neighbors)
        {
            Point n = reference.North();
            Point e = reference.East();
            Point s = reference.South();
            Point w = reference.West();
            Point ne = n.East();
            Point nw = n.West();
            Point se = s.East();
            Point sw = s.West();

            neighbors.AddRange(elves.Where(elf =>
                elf.Position == n || elf.Position == e || elf.Position == s || elf.Position == w ||
                elf.Position == ne || elf.Position == se || elf.Position == sw || elf.Position == nw));
        }

        internal static void Problem1()
        {
            List<Elf> elves = Load();

            HashSet<Point> candidates = new HashSet<Point>();
            HashSet<Point> rejected = new HashSet<Point>();

            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

#if SAMPLE
            foreach (Elf elf in elves)
            {
                minX = Math.Min(minX, elf.Position.X);
                maxX = Math.Max(maxX, elf.Position.X);
                minY = Math.Min(minY, elf.Position.Y);
                maxY = Math.Max(maxY, elf.Position.Y);
            }

            Console.WriteLine($"({maxX} - {minX}) * ({maxY} - {minY})");

            
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Console.Write(FindElf(elves, new Point(x, y)) switch { null => '.', _ => '#' });
                }

                Console.WriteLine();
            }
#endif
            List<Elf> neighbors = new List<Elf>();

            for (int round = 0; round < 10; round++)
            {
#if SAMPLE
                Console.WriteLine($"Begin round {round + 1}");
#endif

                int choiceOffset = round % 4;
                candidates.Clear();
                rejected.Clear();

                foreach (Elf elf in elves)
                {
                    elf.CanMove = false;
                    Point cur = elf.Position;

                    neighbors.Clear();
                    FindAdjacentElves(elves, cur, neighbors);

                    if (neighbors.Count == 0)
                    {
                        continue;
                    }

                    for (int relChoice = 0; relChoice < 4; relChoice++)
                    {
                        int choice = (relChoice + choiceOffset) % 4;

                        if (choice == 0)
                        {
                            if (FindAnyElf(neighbors, cur, 0, -1) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.North();
                                break;
                            }
                        }
                        else if (choice == 1)
                        {
                            if (FindAnyElf(neighbors, cur, 0, 1) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.South();
                                break;
                            }
                        }
                        else if (choice == 2)
                        {
                            if (FindAnyElf(neighbors, cur, -1, 0) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.West();
                                break;
                            }
                        }
                        else
                        {
                            Debug.Assert(choice == 3);

                            if (FindAnyElf(neighbors, cur, 1, 0) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.East();
                                break;
                            }
                        }
                    }

                    if (elf.CanMove)
                    {
                        if (!candidates.Add(elf.MoveTo))
                        {
                            rejected.Add(elf.MoveTo);
                        }
                    }
                }

                foreach (Elf elf in elves)
                {
                    if (elf.CanMove && !rejected.Contains(elf.MoveTo))
                    {
                        elf.Position = elf.MoveTo;
                    }
                }

#if SAMPLE
                minX = int.MaxValue;
                minY = int.MaxValue;
                maxX = int.MinValue;
                maxY = int.MinValue;

                foreach (Elf elf in elves)
                {
                    minX = Math.Min(minX, elf.Position.X);
                    maxX = Math.Max(maxX, elf.Position.X);
                    minY = Math.Min(minY, elf.Position.Y);
                    maxY = Math.Max(maxY, elf.Position.Y);
                }

                Console.WriteLine($"({maxX} - {minX}) * ({maxY} - {minY})");

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        Console.Write(FindElf(elves, new Point(x, y)) switch { null => '.', _ => '#' });
                    }

                    Console.WriteLine();
                }
#endif
            }


            foreach (Elf elf in elves)
            {
                minX = Math.Min(minX, elf.Position.X);
                maxX = Math.Max(maxX, elf.Position.X);
                minY = Math.Min(minY, elf.Position.Y);
                maxY = Math.Max(maxY, elf.Position.Y);
            }

            long rect = (maxX - minX + 1) * (maxY - minY + 1);
            Console.WriteLine($"({maxX} - {minX} + 1) * ({maxY} - {minY} + 1)");
            Console.WriteLine(rect);
            Console.WriteLine(rect - elves.Count);
        }

        internal static void Problem2()
        {
            List<Elf> elves = Load();

            HashSet<Point> candidates = new HashSet<Point>();
            HashSet<Point> rejected = new HashSet<Point>();

            int round = 0;
            int moved = 1;
            List<Elf> neighbors = new List<Elf>();

            while (moved > 0)
            {
                moved = 0;
                round++;

#if !SAMPLE
                if (round % 10 == 0)
#endif
                {
                    Console.WriteLine($"Starting round {round}");
                }

                int choiceOffset = (round - 1) % 4;
                candidates.Clear();
                rejected.Clear();

                foreach (Elf elf in elves)
                {
                    elf.CanMove = false;
                    Point cur = elf.Position;
                    neighbors.Clear();

                    FindAdjacentElves(elves, cur, neighbors);

                    if (neighbors.Count == 0)
                    {
                        continue;
                    }

                    for (int relChoice = 0; relChoice < 4; relChoice++)
                    {
                        int choice = (relChoice + choiceOffset) % 4;

                        if (choice == 0)
                        {
                            if (FindAnyElf(neighbors, cur, 0, -1) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.North();
                                break;
                            }
                        }
                        else if (choice == 1)
                        {
                            if (FindAnyElf(neighbors, cur, 0, 1) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.South();
                                break;
                            }
                        }
                        else if (choice == 2)
                        {
                            if (FindAnyElf(neighbors, cur, -1, 0) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.West();
                                break;
                            }
                        }
                        else
                        {
                            Debug.Assert(choice == 3);

                            if (FindAnyElf(neighbors, cur, 1, 0) == null)
                            {
                                elf.CanMove = true;
                                elf.MoveTo = cur.East();
                                break;
                            }
                        }
                    }

                    if (elf.CanMove)
                    {
                        if (!candidates.Add(elf.MoveTo))
                        {
                            rejected.Add(elf.MoveTo);
                        }
                    }
                }

                foreach (Elf elf in elves)
                {
                    if (elf.CanMove && !rejected.Contains(elf.MoveTo))
                    {
                        elf.Position = elf.MoveTo;
                        moved++;
                    }
                }

#if !SAMPLE
                if (round % 10 == 0)
#endif
                {
                    Console.WriteLine($"  {moved} elves moved.");
                }
            }

            Console.WriteLine(round);
        }
    }
}
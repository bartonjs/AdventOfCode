using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day18
    {
        private struct Datum
        {
            public bool ExplicitDig;
            public bool Exterior;

            public bool Dug => ExplicitDig || !Exterior;
        }

        internal static void Problem1()
        {
            int maxX = 0;
            int curX = 0;
            int minX = 0;
            int curY = 0;
            int minY = 0;
            int maxY = 0;

            List<LongPoint> points = new();
            points.Add(default);
            long perimeter = 0;

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int value = int.Parse(parts[1]);

                if (parts[0] == "R")
                {
                    curX += value;
                    maxX = int.Max(curX, maxX);
                }
                else if (parts[0] == "L")
                {
                    curX -= value;
                    minX = int.Min(minX, curX);
                }
                else if (parts[0] == "U")
                {
                    curY -= value;
                    minY = int.Min(minY, curY);
                }
                else
                {
                    Debug.Assert(parts[0] == "D");
                    curY += value;
                    maxY = int.Max(maxY, curY);
                }

                points.Add(new LongPoint(curX, curY));
                perimeter += value;
            }

            Console.WriteLine($"X: {{{minX}, {maxX}}}");
            Console.WriteLine($"Y: {{{minY}, {maxY}}}");

            int height = maxY - minY + 10;
            int width = maxX - minX + 10;
            DynamicPlane<Datum> plane = new DynamicPlane<Datum>(width, height);

            for (int i = 0; i < height; i++)
            {
                plane.PushY(new Datum[width]);
            }

            Point cur = new Point(-minX + 5, -minY + 5);

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int val = int.Parse(parts[1]);

                switch (parts[0])
                {
                    case "R":
                    {
                        int stop = cur.X + val;

                        while (cur.X < stop)
                        {
                            cur = cur.East();
                            plane[cur].ExplicitDig = true;
                        }

                        break;
                    }
                    case "L":
                    {
                        int stop = cur.X - val;

                        while (cur.X > stop)
                        {
                            cur = cur.West();
                            plane[cur].ExplicitDig = true;
                        }

                        break;
                    }
                    case "U":
                    {
                        int stop = cur.Y - val;

                        while (cur.Y > stop)
                        {
                            cur = cur.North();
                            plane[cur].ExplicitDig = true;
                        }

                        break;
                    }
                    default:
                    {
                        Debug.Assert(parts[0] == "D");

                        int stop = cur.Y + val;

                        while (cur.Y < stop)
                        {
                            cur = cur.South();
                            plane[cur].ExplicitDig = true;
                        }

                        break;
                    }
                }
            }

            for (int i = 0; i < plane.Height; i++)
            {
                Point start = new Point(0, i);

                if (!plane[start].ExplicitDig && !plane[start].Exterior)
                {
                    Pathing.BreadthFirstSearch(
                        plane,
                        start,
                        node =>
                        {
                            plane[node].Exterior = true;
                            return false;
                        },
                        ExteriorPeers);
                }
            }

            long ret = 0;

            foreach (Point point in plane.AllPoints())
            {
                if (plane[point].Dug)
                {
                    ret++;
                }
            }

            Console.WriteLine($"Initial: {ret}");
            Console.WriteLine($"Shoelace: {ShoelaceArea(points, perimeter)}");
        }

        private static IEnumerable<Point> ExteriorPeers(Point from, DynamicPlane<Datum> plane)
        {
            foreach (var point in new[] { from.North(), from.East(), from.South(), from.West() })
            {
                if (plane.TryGetValue(point, out Datum value))
                {
                    if (value.ExplicitDig || value.Exterior)
                    {
                        continue;
                    }

                    yield return point;
                }
            }
        }

        internal static void Problem2()
        {
            long maxX = 0;
            long curX = 0;
            long minX = 0;
            long curY = 0;
            long minY = 0;
            long maxY = 0;

            List<LongPoint> points = new();
            points.Add(default);
            long perimeter = 0;

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                (string part, long value) = Parse(parts[2]);

                if (part == "R")
                {
                    curX += value;
                    maxX = long.Max(curX, maxX);
                }
                else if (part == "L")
                {
                    curX -= value;
                    minX = long.Min(minX, curX);
                }
                else if (part == "U")
                {
                    curY -= value;
                    minY = long.Min(minY, curY);
                }
                else
                {
                    Debug.Assert(part == "D");
                    curY += value;
                    maxY = long.Max(maxY, curY);
                }

                points.Add(new LongPoint(curX, curY));
                perimeter += value;
            }

            Console.WriteLine($"X: {{{minX}, {maxX}}}");
            Console.WriteLine($"Y: {{{minY}, {maxY}}}");

            Console.WriteLine(ShoelaceArea(points, perimeter));
        }

        private static long ShoelaceArea(List<LongPoint> points, long perimeter)
        {
            int n = points.Count;
            long area = 0;

            for (int i = 0; i < n - 1; i++)
            {
                area += points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y;
            }

            return (area + points[n - 1].X * points[0].Y - points[0].X * points[n - 1].Y) / 2 + perimeter / 2 + 1;
        }

        private static (string Direction, long Value) Parse(string part3)
        {
            string direction = part3[7] switch
            {
                '0' => "R",
                '1' => "D",
                '2' => "L",
                '3' => "U",
            };

            long value = long.Parse(part3.AsSpan(2, 5), NumberStyles.HexNumber);

            return (direction, value);
        }
    }
}
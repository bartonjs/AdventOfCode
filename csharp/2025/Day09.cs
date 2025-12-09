using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day09
    {
        private static List<Point> Load()
        {
            List<Point> points = new List<Point>();

            foreach (string s in Data.Enumerate())
            {
                int comma = s.IndexOf(',');
                points.Add(new Point(int.Parse(s.AsSpan(0, comma)), int.Parse(s.AsSpan(comma + 1))));
            }

            return points;
        }

        internal static void Problem1()
        {
            long ret = 0;
            List<Point> points = Load();

            for (int i = 0; i < points.Count; i++)
            {
                Point a = points[i];

                for (int j = i + 1; j < points.Count; j++)
                {
                    Point b = points[j];

                    long area = Area(a, b);

                    if (area > ret)
                    {
                        ret = area;
                    }
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            List<Point> points = Load();

            for (int i = 0; i < points.Count; i++)
            {
                Point a = points[i];

                for (int j = i + 1; j < points.Count; j++)
                {
                    Point b = points[j];

                    long area = Area(a, b);

                    if (area > ret)
                    {
                        if (IsContained(points, a, b))
                        {
                            Utils.TraceForSample($"{a} and {b} have area {area} and are contained.");
                            ret = area;
                        }
                    }
                }
            }

            Console.WriteLine(ret);
        }

        private static long Area(Point a, Point b)
        {
            long dx = long.Abs(a.X - b.X) + 1;
            long dy = long.Abs(a.Y - b.Y) + 1;

            return dx * dy;
        }

        private static bool IsContained(List<Point> shape, Point rectA, Point rectB)
        {
            int boxLeft = int.Min(rectA.X, rectB.X);
            int boxRight = int.Max(rectA.X, rectB.X);
            int boxTop = int.Min(rectA.Y, rectB.Y);
            int boxBot = int.Max(rectA.Y, rectB.Y);

            bool trRight = false;
            bool trUp = false;
            bool brRight = false;
            bool brDown = false;
            bool blLeft = false;
            bool blDown = false;
            bool tlLeft = false;
            bool tlUp = false;

            for (int i = 0; i < shape.Count; i++)
            {
                int lineMate = (i + 1) % shape.Count;
                Point shapeTest = shape[i];
                Point shapeTestMate = shape[lineMate];

                if (shapeTest.X == shapeTestMate.X)
                {
                    int lineTop = int.Min(shapeTest.Y, shapeTestMate.Y);
                    int lineBot = int.Max(shapeTest.Y, shapeTestMate.Y);

                    if (boxLeft < shapeTest.X && boxRight > shapeTest.X)
                    {
                        if (boxTop > lineTop && boxTop < lineBot)
                        {
                            return false;
                        }

                        if (boxBot > lineTop && boxBot < lineBot)
                        {
                            return false;
                        }
                    }

                    if (boxTop >= lineTop && boxTop <= lineBot)
                    {
                        if (boxRight <= shapeTest.X)
                        {
                            trRight = true;
                        }

                        if (boxLeft >= shapeTest.X)
                        {
                            tlLeft = true;
                        }
                    }

                    if (boxBot >= lineTop && boxBot <= lineBot)
                    {
                        if (boxRight <= shapeTest.X)
                        {
                            brRight = true;
                        }

                        if (boxLeft >= shapeTest.X)
                        {
                            blLeft = true;
                        }
                    }
                }
                else if (shapeTest.Y == shapeTestMate.Y)
                {
                    int lineLeft = int.Min(shapeTest.X, shapeTestMate.X);
                    int lineRight = int.Max(shapeTest.X, shapeTestMate.X);

                    if (boxTop < shapeTest.Y && boxBot > shapeTest.Y)
                    {
                        if (boxLeft > lineLeft && boxLeft < lineRight)
                        {
                            return false;
                        }

                        if (boxRight > lineLeft && boxRight < lineRight)
                        {
                            return false;
                        }
                    }

                    if (boxLeft >= lineLeft && boxLeft <= lineRight)
                    {
                        if (boxTop >= shapeTest.Y)
                        {
                            tlUp = true;
                        }

                        if (boxBot <= shapeTest.Y)
                        {
                            blDown = true;
                        }
                    }

                    if (boxRight >= lineLeft && boxRight <= lineRight)
                    {
                        if (boxTop >= shapeTest.Y)
                        {
                            trUp = true;
                        }

                        if (boxBot <= shapeTest.Y)
                        {
                            brDown = true;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return tlLeft && tlUp && trRight && trUp && brRight && brDown && blDown && blLeft;
        }
    }
}

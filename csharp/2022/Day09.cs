using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day09
    {
        private static IEnumerable<(char,int)> LoadData()
        {
            Regex regex = Regex();

            foreach (string s in Data.Enumerate())
            {
                Match match = regex.Match(s);
                yield return
                (
                    match.Groups[1].ValueSpan[0],
                    int.Parse(match.Groups[2].ValueSpan));
            }
        }

        [GeneratedRegex(@"^(.) (\d+)")]
        private static partial Regex Regex();

        internal static void Problem1()
        {
            HashSet<Point> tailPositions = new HashSet<Point>();

            int tailX, tailY, headX, headY;
            tailX = tailY = headX = headY = 0;

            foreach ((char dir, int count) in LoadData())
            {
#if SAMPLE
                Console.WriteLine($"{dir}: {count}");
#endif

                for (int i = 0; i < count; i++)
                {
                    switch (dir)
                    {
                        case 'U':
                            headY -= 1;
                            break;
                        case 'D':
                            headY += 1;
                            break;
                        case 'L':
                            headX -= 1;
                            break;
                        case 'R':
                            headX += 1;
                            break;
                        default:
                            throw new NotFiniteNumberException();
                    }

                    int xDiff = headX - tailX;
                    int yDiff = headY - tailY;
                    bool xGap = Math.Abs(xDiff) > 1;
                    bool yGap = Math.Abs(yDiff) > 1;
                    
                    if (xGap || yGap)
                    {
                        tailX += Math.Sign(xDiff);
                        tailY += Math.Sign(yDiff);
                    }

                    tailPositions.Add(new Point(tailX, tailY));

#if SAMPLE
                    Console.WriteLine($"head: ({headX}, {headY}) tail: ({tailX}, {tailY})");
#endif
                }
            }

            Console.WriteLine(tailPositions.Count);
        }

        internal static void Problem2()
        {
            HashSet<Point> tailPositions = new HashSet<Point>();

            Point[] points = new Point[10];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point();
            }

            foreach ((char dir, int count) in LoadData())
            {
#if SAMPLE
                Console.WriteLine($"{dir}: {count}");
#endif
                for (int i = 0; i < count; i++)
                {
                    switch (dir)
                    {
                        case 'U':
                            points[0].Y -= 1;
                            break;
                        case 'D':
                            points[0].Y += 1;
                            break;
                        case 'L':
                            points[0].X -= 1;
                            break;
                        case 'R':
                            points[0].X += 1;
                            break;
                        default:
                            throw new NotFiniteNumberException();
                    }

#if SAMPLE
                    Console.WriteLine($"H: {points[0]}");
#endif

                    for (int p = 1; p < points.Length; p++)
                    {
                        Point pP = points[p - 1];
                        Point pC = points[p];
                        int xDiff = pP.X - pC.X;
                        int yDiff = pP.Y - pC.Y;
                        bool xGap = Math.Abs(xDiff) > 1;
                        bool yGap = Math.Abs(yDiff) > 1;

                        if (xGap || yGap)
                        {
                            pC.X += Math.Sign(xDiff);
                            pC.Y += Math.Sign(yDiff);
                        }

#if SAMPLE
                        Console.WriteLine($"  P{p}: {pC}");
#endif
                    }

                    tailPositions.Add(new Point(points[9].X, points[9].Y));
                }
            }

            Console.WriteLine(tailPositions.Count);
        }
    }
}
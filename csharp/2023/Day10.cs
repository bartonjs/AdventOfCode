using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day10
    {
        private class Datum
        {
            public char Shape;
            public Point Loc;
            public int Value = -1;

            public IEnumerable<Point> GetNeighbors()
            {
                switch (Shape)
                {
                    case '|':
                        yield return Loc.North();
                        yield return Loc.South();
                        break;
                    case '-':
                        yield return Loc.West();
                        yield return Loc.East();
                        break;
                    case 'L':
                        yield return Loc.North();
                        yield return Loc.East();
                        break;
                    case 'J':
                        yield return Loc.North();
                        yield return Loc.West();
                        break;
                    case '7':
                        yield return Loc.West();
                        yield return Loc.South();
                        break;
                    case 'F':
                        yield return Loc.East();
                        yield return Loc.South();
                        break;
                    case '.':
                    case '\0':
                        break;
                    case 'S':
                        yield return Loc.North();
                        yield return Loc.East();
                        yield return Loc.West();
                        yield return Loc.South();
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        internal static void Problem1()
        {
            long ret = 0;
            DynamicPlane<Datum> plane = null;
            int row = 0;
            int startY = -1;
            int startX = -1;

            foreach (string s in Data.Enumerate())
            {
                Datum[] datums = s.Select((c, idx) => new Datum { Shape = c, Loc = new Point(idx, row)}).ToArray();

                if (plane is null)
                {
                    plane = new(datums);
                }
                else
                {
                    plane.PushY(datums);
                }

                if (startX < 0)
                {
                    startX = s.IndexOf('S');

                    if (startX >= 0)
                    {
                        startY = row;
                    }
                }

                row++;
            }

            Point startPoint = new Point(startX, startY);
            plane[startPoint].Value = 0;

            Queue<Datum> datumQueue = new();

            Console.WriteLine($"Start point is {startPoint}");

            foreach (Point neighbor in plane[startPoint].GetNeighbors())
            {
                if (plane.TryGetValue(neighbor, out Datum actualNeighbor))
                {
                    if (actualNeighbor.GetNeighbors().Contains(startPoint))
                    {
                        actualNeighbor.Value = 1;
                        datumQueue.Enqueue(actualNeighbor);

                        Console.WriteLine($"Neighbor found at {neighbor}");
                    }
                }
            }

            while (datumQueue.Count > 0)
            {
                Datum next = datumQueue.Dequeue();
                ret = long.Max(ret, next.Value);

                foreach (Point neighbor in next.GetNeighbors())
                {
                    Datum actualNeighbor = plane[neighbor];

                    if (actualNeighbor.Value < 0)
                    {
                        actualNeighbor.Value = next.Value + 1;
                        datumQueue.Enqueue(actualNeighbor);
                    }
                }
            }

            Utils.TraceForSample(plane.Print(d => d.Shape));
            Console.WriteLine(ret);
        }

        internal static void Problem2x()
        {
            long ret = 0;
            DynamicPlane<Datum> plane = null;
            int row = 0;
            int startY = -1;
            int startX = -1;

            foreach (string s in Data.Enumerate())
            {
                Datum[] datums = s.Select((c, idx) => new Datum { Shape = c, Loc = new Point(idx, row) }).ToArray();

                if (plane is null)
                {
                    plane = new(datums);
                }
                else
                {
                    plane.PushY(datums);
                }

                if (startX < 0)
                {
                    startX = s.IndexOf('S');

                    if (startX >= 0)
                    {
                        startY = row;
                    }
                }

                row++;
            }

            Point startPoint = new Point(startX, startY);
            plane[startPoint].Value = 0;

            // I should generalize this...
#if SAMPLE
            plane[startPoint].Shape = '7';
#else
            plane[startPoint].Shape = '|';
#endif

            Queue<Datum> datumQueue = new();

            Console.WriteLine($"Start point is {startPoint}");

            foreach (Point neighbor in plane[startPoint].GetNeighbors())
            {
                if (plane.TryGetValue(neighbor, out Datum actualNeighbor))
                {
                    if (actualNeighbor.GetNeighbors().Contains(startPoint))
                    {
                        actualNeighbor.Value = 1;
                        datumQueue.Enqueue(actualNeighbor);

                        Console.WriteLine($"Neighbor found at {neighbor}");
                    }
                }
            }

            while (datumQueue.Count > 0)
            {
                Datum next = datumQueue.Dequeue();
                ret = long.Max(ret, next.Value);

                foreach (Point neighbor in next.GetNeighbors())
                {
                    Datum actualNeighbor = plane[neighbor];

                    if (actualNeighbor.Value < 0)
                    {
                        actualNeighbor.Value = next.Value + 1;
                        datumQueue.Enqueue(actualNeighbor);
                    }
                }
            }

            int inside = 0;

            foreach (Point candidate in plane.AllPoints())
            {
                if (plane[candidate].Value < 0)
                {
                    plane[candidate].Shape = '.';
                }

                Debug.Assert(plane[candidate].Loc == candidate);
            }

            Utils.TraceForSample("");
            Utils.TraceForSample("");
            Utils.TraceForSample("");
            Utils.TraceForSample(plane.Print(d => d.Shape));

            foreach (Point candidate in plane.AllPoints())
            {
                int northCount = 0;
                int southCount = 0;
                int westCount = 0;
                int eastCount = 0;

                if (plane[candidate].Value < 0)
                {
                    Datum lineStart = null;
                    Point work = candidate.North();

                    while (plane.TryGetValue(work, out Datum there))
                    {
                        if (there.Value >= 0)
                        {
                            if (there.Shape == 'F')
                            {
                                Debug.Assert(lineStart is not null);
                                
                                if (lineStart.Shape == 'J')
                                {
                                    northCount++;
                                }

                                lineStart = null;
                            }
                            else if (there.Shape == '7')
                            {
                                Debug.Assert(lineStart is not null);

                                if (lineStart.Shape == 'L')
                                {
                                    northCount++;
                                }

                                lineStart = null;
                            }
                            else if (there.Shape == '-')
                            {
                                lineStart = null;
                                northCount++;
                            }
                            else if (there.Shape is 'J' or 'L')
                            {
                                Debug.Assert(lineStart is null);
                                lineStart = there;
                            }
                            else
                            {
                                Debug.Assert(there.Shape == '|');
                            }
                        }
                        else
                        {
                            lineStart = null;
                        }

                        work = work.North();
                    }

                    lineStart = null;
                    work = candidate.West();

                    while (plane.TryGetValue(work, out Datum there))
                    {
                        if (there.Value >= 0)
                        {
                            if (there.Shape == 'F')
                            {
                                Debug.Assert(lineStart is not null);

                                if (lineStart.Shape == 'J')
                                {
                                    westCount++;
                                }

                                lineStart = null;
                            }
                            else if (there.Shape == 'L')
                            {
                                Debug.Assert(lineStart is not null);

                                if (lineStart.Shape == '7')
                                {
                                    westCount++;
                                }

                                lineStart = null;
                            }
                            else if (there.Shape == '|')
                            {
                                lineStart = null;
                                westCount++;
                            }
                            else if (there.Shape is 'J' or '7')
                            {
                                Debug.Assert(lineStart is null);
                                lineStart = there;
                            }
                            else
                            {
                                Debug.Assert(there.Shape == '-');
                            }
                        }
                        else
                        {
                            lineStart = null;
                        }

                        work = work.West();
                    }

                    if (northCount % 2 == 0 || westCount % 2 == 0)
                    {
                        plane[candidate].Shape = 'o';
                    }
                    else
                    {
                        inside++;
                        plane[candidate].Shape = 'i';

                    }

                    Utils.TraceForSample($"({candidate.X}, {candidate.Y}): N:{northCount}, E:{eastCount}, S:{southCount}, W:{westCount} -- {plane[candidate].Shape}");
                }
            }

            Utils.TraceForSample("");
            Utils.TraceForSample("");
            Utils.TraceForSample("");
            Utils.TraceForSample(plane.Print(d => d.Shape));
            Console.WriteLine(inside);
        }
    }
}
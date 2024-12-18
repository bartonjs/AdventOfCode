using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day12
    {
        internal static DynamicPlane<char> Load()
        {
            DynamicPlane<char> world = null;

            foreach (string s in Data.Enumerate())
            {
                char[] arr = s.ToCharArray();

                if (world is null)
                {
                    world = new DynamicPlane<char>(arr);
                }
                else
                {
                    world.PushY(arr);
                }
            }

            return world;
        }

        internal static void Problem1()
        {
            List<HashSet<Point>> regions = new();
            DynamicPlane<char> world = Load();

            foreach (Point p in world.AllPoints())
            {
                if (regions.Any(r => r.Contains(p)))
                {
                    continue;
                }

                HashSet<Point> region = new() { p };
                regions.Add(region);

                Pathing.BreadthFirstSearch(
                    world,
                    p,
                    point =>
                    {
                        region.Add(point);
                        return false;
                    },
                    static (point, world) => point.GetNeighbors(Point.AllDirections)
                        .Where(p => world.TryGetValue(p, out char value) && value == world[point]));
            }

            long ret = 0;

            foreach (HashSet<Point> region in regions)
            {
                long area = region.Count;
                long perimeter = area * 4;

                foreach (Point p in region)
                {
                    foreach (Point n in p.GetNeighbors(Point.AllDirections))
                    {
                        if (region.Contains(n))
                        {
                            perimeter--;
                        }
                    }
                }

                Utils.TraceForSample(
                    $"A region of '{world[region.First()]}' plants with price {area} * {perimeter} = {area * perimeter}");

                ret += area * perimeter;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            List<HashSet<Point>> regions = new();
            DynamicPlane<char> world = Load();

            Stopwatch sw = Stopwatch.StartNew();

            foreach (Point p in world.AllPoints())
            {
                if (regions.Any(r => r.Contains(p)))
                {
                    continue;
                }

                HashSet<Point> region = new() { p };
                regions.Add(region);

                Pathing.BreadthFirstSearch(
                    world,
                    p,
                    point =>
                    {
                        region.Add(point);
                        return false;
                    },
                    static (point, world) => point.GetNeighbors(Point.AllDirections)
                        .Where(p => world.TryGetValue(p, out char value) && value == world[point]));
            }

            sw.Stop();
            Console.WriteLine($"Flood fill took {sw.Elapsed.TotalMilliseconds:F4}ms");

            Dictionary<Point, HashSet<Point>> regionMap = new();

            foreach (HashSet<Point> region in regions)
            {
                foreach (Point p in region)
                {
                    regionMap[p] = region;
                }
            }

            long ret = 0;

            Dictionary<HashSet<Point>, long> sideDict = new();
            Dictionary<HashSet<Point>, HashSet<Point>> enclaves = new();

            foreach (HashSet<Point> region in regions)
            {
                long area = region.Count;
                long sides = 0;

                Point start = region.OrderBy(p => p.Y).ThenBy(p => p.X).First();
                start = start.GetNeighbor(Directions2D.West);
                Point p = start;

                regionMap.TryGetValue(start, out HashSet<Point> parent);
                bool enclave = parent is not null;

                Utils.TraceForSample($"Investigating region '{world[p.GetNeighbor(Directions2D.East)]}' starting at {p}");
                Directions2D walking = Directions2D.South;
                Directions2D looking = Directions2D.East;
                bool advanced = false;

                do
                {
                    while (region.Contains(p.GetNeighbor(looking)) && !region.Contains(p.GetNeighbor(walking)))
                    {
                        if (enclave)
                        {
                            if (!regionMap.TryGetValue(p, out HashSet<Point> localParent) || localParent != parent)
                            {
                                enclave = false;
                            }
                        }

                        advanced = true;
                        p = p.GetNeighbor(walking);
                    }

                    sides++;

                    if (region.Contains(p.GetNeighbor(looking)))
                    {
                        Utils.TraceForSample($"  Walking {walking} is blocked from {p}, turning right.");
                        walking = walking.TurnRight();
                    }
                    else
                    {
                        Utils.TraceForSample($"  Walking {walking} passed a corner at {p}, turning left and advancing.");
                        walking = walking.TurnLeft();
                        p = p.GetNeighbor(walking);
                        advanced = true;
                    }

                    looking = walking.TurnLeft();
                } while (p != start || !advanced);

                Utils.TraceForSample(
                    $"A region of '{world[region.First()]}' plants with price {area} * {sides} = {area * sides}");

                sideDict[region] = sides;

                if (enclave)
                {
                    Utils.TraceForSample($"This region is an enclave, contained in {world[parent.First()]}");
                    enclaves[region] = parent;
                }
            }

            foreach (var kvp in enclaves)
            {
                Utils.TraceForSample($"Because {world[kvp.Key.First()]} was an enclave, {world[kvp.Value.First()]} needs {sideDict[kvp.Key]} more sides.");
                sideDict[kvp.Value] += sideDict[kvp.Key];
            }

            foreach (var kvp in sideDict)
            {
                ret += kvp.Key.Count * kvp.Value;
            }

            Console.WriteLine(ret);
        }
    }
}
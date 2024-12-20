using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day20
    {
        private static (DynamicPlane<char> World, Point Start, Point End) Load()
        {
            return Plane.LoadCharPlane(Data.Enumerate(), 'S', 'E');
        }

        internal static void Problem1()
        {
            Console.WriteLine(Run(2, 1));
        }

        internal static void Problem2()
        {
            Console.WriteLine(Run(20, 50));
        }

        private static long Run(int maxWormholeLength, int sampleThreshold)
        {
            const int InclusionThreshold = 100;

            (DynamicPlane<char> world, Point start, Point end) = Load();

            static IEnumerable<(Point, int)> Neighbors(Point from, DynamicPlane<char> world)
            {
                foreach (Point neighbor in from.GetCardinalNeighbors())
                {
                    if (world.TryGetValue(neighbor, out char value) && value != '#')
                    {
                        yield return (neighbor, 1);
                    }
                }
            }

            CostPlane backwardCosts = new CostPlane(world.Width, world.Height);
            Pathing.DijkstraCosts(world, end, Neighbors, backwardCosts);

            Print.Verbose($"Normal cost is {backwardCosts[start]}");
            long ret = 0;

#if SAMPLE
            Dictionary<long, long> savingsCounts = new();
#endif

            foreach (Point cheatStart in world.AllPoints())
            {
                if (!backwardCosts.TryGetValue(cheatStart, out int entranceCost))
                {
                    continue;
                }

                // If it costs less than 100 to get from the candidate wormhole entrance
                // to the end, then we can't save 100 or more by taking the wormhole.
                if (entranceCost < InclusionThreshold
#if SAMPLE
                    && entranceCost < sampleThreshold
#endif
                   )
                {
                    continue;
                }

                int xMin = cheatStart.X - maxWormholeLength;
                int yMin = cheatStart.Y - maxWormholeLength;

                for (int x = cheatStart.X + maxWormholeLength; x >= xMin; x--)
                {
                    for (int y = cheatStart.Y + maxWormholeLength; y >= yMin; y--)
                    {
                        Point cheatEnd = new Point(x, y);
                        int wormholeDistance = cheatStart.ManhattanDistance(cheatEnd);

                        if (wormholeDistance > maxWormholeLength)
                        {
                            continue;
                        }

                        if (!backwardCosts.TryGetValue(cheatEnd, out int exitCost))
                        {
                            continue;
                        }

                        int alternateCost = wormholeDistance + exitCost;
                        int delta = entranceCost - alternateCost;

#if SAMPLE
                        if (delta >= sampleThreshold)
                        {
                            savingsCounts.Increment(delta);
                        }
#endif

                        if (delta >= InclusionThreshold)
                        {
                            ret++;
                        }
                    }
                }
            }

#if SAMPLE
            foreach (var kvp in savingsCounts.OrderBy(kvp => kvp.Key))
            {
                Console.WriteLine($"{kvp.Value} saved {kvp.Key}");
            }
#endif

            return ret;
        }

        internal static void Problem1Original()
        {
            (DynamicPlane<char> world, Point start, Point end) = Load();

            List<Point> idealPath = new();
            long normalCost = Pathing.AStar(
                world,
                start,
                end,
                static (from, world) =>
                    from.GetNeighbors(Point.AllDirections)
                        .Where(p => world.TryGetValue(p, out char value) && value != '#').Select(p => (p, 1)),
                static (candidate, end, world) => candidate.ManhattanDistance(end),
                idealPath);

            long ret = 0;

            Console.WriteLine($"Normal cost is {normalCost}");
#if SAMPLE
            Dictionary<long, long> savingsCounts = new();
#endif
            HashSet<Point> cheatStarts = new HashSet<Point>();

            foreach (Point pathPoint in idealPath)
            {
                foreach (Point neighbor in pathPoint.GetNeighbors(Point.AllDirections))
                {
                    if (world[neighbor] == '#')
                    {
                        cheatStarts.Add(neighbor);
                    }
                }
            }

            foreach (Point cheatStart in cheatStarts)
            {
                if (world[cheatStart] != '#')
                {
                    continue;
                }

                long hereCost = Pathing.AStar(
                    (world, cheatStart),
                    start,
                    end,
                    static (from, world) =>
                    {
                        IEnumerable<Point> candidates = from.GetNeighbors(Point.AllDirections);

                        if (from == world.cheatStart)
                        {
                        }
                        else if (!world.world.TryGetValue(from, out char here) || here == '#')
                        {
                            return [];
                        }
                        else
                        {
                            candidates = candidates.Where(
                                p => p == world.cheatStart ||
                                     world.world.TryGetValue(p, out char value) && value != '#');
                        }

                        return candidates.Select(p => (p, 1));
                    },
                    static (candidate, end, world) => candidate.ManhattanDistance(end));

                long savings = normalCost - hereCost;

#if SAMPLE
                Utils.TraceForSample($"Cheating at t={cheatStart} results in {hereCost} ({savings})");
                savingsCounts.Increment(savings);
#endif

                if (savings >= 100)
                {
                    ret++;
                }
            }

#if SAMPLE
            foreach (var kvp in savingsCounts.OrderBy(kvp => kvp.Key))
            {
                Console.WriteLine($"{kvp.Value} saved {kvp.Key}");
            }
#endif

            Console.WriteLine(ret);
        }

        private sealed class CostPlane : IDictionary<Point, int>
        {
            private readonly FixedPlane<int> _plane;

            internal CostPlane(int width, int height)
            {
                _plane = new FixedPlane<int>(width, height);
                _plane.Fill(int.MaxValue);
            }

            IEnumerator<KeyValuePair<Point, int>> IEnumerable<KeyValuePair<Point, int>>.GetEnumerator()
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotSupportedException();
            }

            void ICollection<KeyValuePair<Point, int>>.Add(KeyValuePair<Point, int> item)
            {
                throw new NotSupportedException();
            }

            void ICollection<KeyValuePair<Point, int>>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<KeyValuePair<Point, int>>.Contains(KeyValuePair<Point, int> item)
            {
                throw new NotSupportedException();
            }

            void ICollection<KeyValuePair<Point, int>>.CopyTo(KeyValuePair<Point, int>[] array, int arrayIndex)
            {
                throw new NotSupportedException();
            }

            bool ICollection<KeyValuePair<Point, int>>.Remove(KeyValuePair<Point, int> item)
            {
                throw new NotSupportedException();
            }

            int ICollection<KeyValuePair<Point, int>>.Count => throw new NotSupportedException();

            bool ICollection<KeyValuePair<Point, int>>.IsReadOnly => false;

            void IDictionary<Point, int>.Add(Point key, int value)
            {
                throw new NotSupportedException();
            }

            bool IDictionary<Point, int>.ContainsKey(Point key)
            {
                throw new NotSupportedException();
            }

            bool IDictionary<Point, int>.Remove(Point key)
            {
                throw new NotSupportedException();
            }

            public bool TryGetValue(Point key, out int value)
            {
                if (!_plane.TryGetValue(key, out int val) || val == int.MaxValue)
                {
                    value = 0;
                    return false;
                }

                value = val;
                return true;
            }

            public ref int this[Point key] => ref _plane[key];

            int IDictionary<Point, int>.this[Point key]
            {
                get => _plane[key];
                set => _plane[key] = value;
            }

            ICollection<Point> IDictionary<Point, int>.Keys => throw new NotSupportedException();

            ICollection<int> IDictionary<Point, int>.Values => throw new NotSupportedException();
        }
    }
}
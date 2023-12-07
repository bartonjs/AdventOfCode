using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day17
    {
        private static IEnumerable<int> EnumerateWind()
        {
            string data = Data.Enumerate().First();

            while (true)
            {
                foreach (char c in data)
                {
                    yield return c switch { '<' => -1, '>' => 1 };
                }
            }
        }

        private static IEnumerator<int> s_windIter = EnumerateWind().GetEnumerator();

        private static int NextWind()
        {
            s_windIter.MoveNext();
            return s_windIter.Current;
        }

        internal static void Problem1()
        {
            const int BottomOfWorld = 400000;
            const int RightEdge = 7;
            byte[,] world = new byte[RightEdge, BottomOfWorld];

            int curTop = BottomOfWorld;
            int spawned = 0;

            while (spawned < 2022)
            {
                int spawnShape = spawned % 5;
                spawned++;

                (int width, int height) = spawnShape switch
                {
                    0 => (4, 1),
                    1 => (3, 3),
                    2 => (3, 3),
                    3 => (1, 4),
                    4 => (2, 2),
                };

                int bottom = curTop - 4;
                int left = 2;
                int right = left + width - 1;
                int top = bottom - height + 1;

                while (true)
                {
                    int dir = NextWind();

                    if (bottom < curTop - 1)
                    {
                        if (right + dir < RightEdge && left + dir >= 0)
                        {
                            //Console.WriteLine($"Moved direction {dir}");
                            right += dir;
                            left += dir;
                        }
                        //else
                        //{
                        //    Console.WriteLine($"Movement {dir} blocked");
                        //}

                        //Console.WriteLine("Moved down");
                        bottom++;
                        top++;
                        continue;
                    }

                    // check for collisions

                    int maybeLeft = left + dir;
                    int maybeRight = right + dir;

                    if (maybeRight < RightEdge &&
                        maybeLeft >= 0 &&
                        !Collision(world, maybeLeft, bottom - 1, spawnShape, freeze: false))
                    {
                        //Console.WriteLine($"Moved direction {dir}");
                        left = maybeLeft;
                        right = maybeRight;
                    }
                    //else
                    //{
                    //    Console.WriteLine($"Movement {dir} blocked");
                    //}

                    if (Collision(world, left, bottom, spawnShape, freeze: true))
                    {
                        //Console.WriteLine($"Spawn {spawned - 1} stopped at y={bottom}");
                        if (top < curTop)
                        {
                            curTop = top;
                        }

                        break;
                    }

                    //Console.WriteLine("Moved down");
                    bottom++;
                    top++;
                }

#if SAMPLE
                if (spawned < 15)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();

                    for (int y = curTop; y < BottomOfWorld; y++)
                    {
                        Console.Write('|');

                        for (int x = 0; x < RightEdge; x++)
                        {
                            Console.Write(world[x, y] switch { 0 => '.', _ => '#' });
                        }

                        Console.WriteLine('|');
                    }

                    Console.WriteLine("+-------+");
                }
#endif
            }

            Console.WriteLine(BottomOfWorld - curTop);

            static bool Collision(byte[,] world, int left, int bottom, int shape, bool freeze)
            {
                int bp1 = bottom + 1;

                // @@@@
                if (shape == 0)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left, bp1] != 0 ||
                        world[left + 1, bp1] != 0 ||
                        world[left + 2, bp1] != 0 ||
                        world[left + 3, bp1] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom] =
                                world[left + 1, bottom] =
                                    world[left + 2, bottom] =
                                        world[left + 3, bottom] = 1;
                        }

                        return true;
                    }

                    return false;
                }
                
                // .@.
                // @@@
                // .@.
                if (shape == 1)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left + 1, bp1] != 0 ||
                        world[left, bottom] != 0 ||
                        world[left + 2, bottom] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom - 1] =
                                world[left + 1, bottom - 2] =
                                    world[left + 1, bottom - 1] =
                                        world[left + 1, bottom] =
                                            world[left + 2, bottom - 1] = 1;
                        }

                        return true;
                    }

                    return false;
                }

                // ..@
                // ..@
                // @@@
                if (shape == 2)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left, bp1] != 0 ||
                        world[left + 1, bp1] != 0 ||
                        world[left + 2, bp1] != 0 ||
                        world[left + 2, bottom] != 0 ||
                        world[left + 2, bottom - 1] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom] =
                                world[left + 1, bottom] =
                                    world[left + 2, bottom] =
                                        world[left + 2, bottom - 1] =
                                            world[left + 2, bottom - 2] = 1;
                        }

                        return true;
                    }

                    return false;
                }

                // @
                // @
                // @
                // @
                if (shape == 3)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left, bp1] != 0 ||
                        world[left, bottom] != 0 ||
                        world[left, bottom - 1] != 0 ||
                        world[left, bottom - 2] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom] =
                                world[left, bottom - 1] =
                                    world[left, bottom - 2] =
                                        world[left, bottom - 3] = 1;
                        }

                        return true;
                    }

                    return false;
                }

                // @@
                // @@
                Debug.Assert(shape == 4);

                if (bp1 == BottomOfWorld ||
                    world[left, bp1] != 0 ||
                    world[left + 1, bp1] != 0 ||
                    world[left, bottom] != 0 ||
                    world[left + 1, bottom] != 0)
                {
                    if (freeze)
                    {
                        world[left, bottom] =
                            world[left + 1, bottom] =
                                world[left, bottom - 1] =
                                    world[left + 1, bottom - 1] = 1;
                    }

                    return true;
                }

                return false;
            }
        }

        private struct WorldState : IEquatable<WorldState>
        {
            private readonly long _profile;
            internal readonly int _windPos;
            internal readonly int _shape;

            public WorldState(byte[,] world, int curTop, int windPos, int shape)
            {
                _profile =
                    (long)world[0, curTop] << 63 |
                    (long)world[1, curTop] << 62 |
                    (long)world[2, curTop] << 61 |
                    (long)world[3, curTop] << 60 |
                    (long)world[4, curTop] << 59 |
                    (long)world[5, curTop] << 58 |
                    (long)world[6, curTop] << 57 |
                    (long)world[0, curTop + 1] << 56 |
                    (long)world[1, curTop + 1] << 55 |
                    (long)world[2, curTop + 1] << 54 |
                    (long)world[3, curTop + 1] << 53 |
                    (long)world[4, curTop + 1] << 52 |
                    (long)world[5, curTop + 1] << 51 |
                    (long)world[6, curTop + 1] << 50 |
                    (long)world[0, curTop + 2] << 49 |
                    (long)world[1, curTop + 2] << 48 |
                    (long)world[2, curTop + 2] << 47 |
                    (long)world[3, curTop + 2] << 46 |
                    (long)world[4, curTop + 2] << 45 |
                    (long)world[5, curTop + 2] << 44 |
                    (long)world[6, curTop + 2] << 43 |
                    (long)world[0, curTop + 3] << 42 |
                    (long)world[1, curTop + 3] << 41 |
                    (long)world[2, curTop + 3] << 40 |
                    (long)world[3, curTop + 3] << 39 |
                    (long)world[4, curTop + 3] << 38 |
                    (long)world[5, curTop + 3] << 37 |
                    (long)world[6, curTop + 3] << 36 |
                    (long)world[0, curTop + 4] << 35 |
                    (long)world[1, curTop + 4] << 34 |
                    (long)world[2, curTop + 4] << 33 |
                    (long)world[3, curTop + 4] << 32 |
                    (long)world[4, curTop + 4] << 31 |
                    (long)world[5, curTop + 4] << 30 |
                    (long)world[6, curTop + 4] << 29 |
                    (long)world[0, curTop + 5] << 28 |
                    (long)world[1, curTop + 5] << 27 |
                    (long)world[2, curTop + 5] << 26 |
                    (long)world[3, curTop + 5] << 25 |
                    (long)world[4, curTop + 5] << 24 |
                    (long)world[5, curTop + 5] << 23 |
                    (long)world[6, curTop + 5] << 22 |
                    (long)world[0, curTop + 6] << 21 |
                    (long)world[1, curTop + 6] << 20 |
                    (long)world[2, curTop + 6] << 19 |
                    (long)world[3, curTop + 6] << 18 |
                    (long)world[4, curTop + 6] << 17 |
                    (long)world[5, curTop + 6] << 16 |
                    (long)world[6, curTop + 6] << 15;

                _windPos = windPos;
                _shape = shape;
            }

            public bool Equals(WorldState other)
            {
                return _profile == other._profile && _windPos == other._windPos && _shape == other._shape;
            }

            public override bool Equals(object obj)
            {
                return obj is WorldState other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_profile, _windPos, _shape);
            }
        }

        internal static void Problem2()
        {
            const int BottomOfWorld = 400000;
            const int RightEdge = 7;
            byte[,] world = new byte[RightEdge, BottomOfWorld];

            int curTop = BottomOfWorld;
            int spawned = 0;

            int windCycle = Data.Enumerate().Single().Length;
            int windCount = 0;

            Dictionary<WorldState, (int,int,int)> profile = new Dictionary<WorldState, (int,int,int)>();
            int prevWindCount = 0;
            int prevSpawned = 0;
            int prevTop = 0;

            while (true)
            {
                int spawnShape = spawned % 5;

                if (spawned >= 5)
                {
                    WorldState state = new WorldState(world, curTop, windCount % windCycle, spawnShape);

                    if (spawnShape == 0)
                    {
                        if (profile.TryGetValue(state, out var known))
                        {
                            (prevSpawned, prevWindCount, prevTop) = known;
                            break;
                        }
                    }

                    profile[state] = (spawned, windCount, curTop);
                }

                spawned++;

                (int width, int height) = spawnShape switch
                {
                    0 => (4, 1),
                    1 => (3, 3),
                    2 => (3, 3),
                    3 => (1, 4),
                    4 => (2, 2),
                };

                int bottom = curTop - 4;
                int left = 2;
                int right = left + width - 1;
                int top = bottom - height + 1;

                while (true)
                {
                    int dir = NextWind();

                    if (bottom < curTop - 1)
                    {
                        if (right + dir < RightEdge && left + dir >= 0)
                        {
                            right += dir;
                            left += dir;
                        }

                        windCount++;
                        bottom++;
                        top++;
                        continue;
                    }

                    // check for collisions

                    int maybeLeft = left + dir;
                    int maybeRight = right + dir;

                    windCount++;
                    if (maybeRight < RightEdge &&
                        maybeLeft >= 0 &&
                        !Collision(world, maybeLeft, bottom - 1, spawnShape, freeze: false))
                    {
                        left = maybeLeft;
                        right = maybeRight;
                    }
                   
                    if (Collision(world, left, bottom, spawnShape, freeze: true))
                    {
                        if (top < curTop)
                        {
                            curTop = top;
                        }

                        break;
                    }

                    bottom++;
                    top++;
                }
            }

            int nowHeight = BottomOfWorld - curTop;
            int deltaH = prevTop - curTop;
            int thenH = BottomOfWorld - prevTop;

            int spawnDelta = spawned - prevSpawned;
            Console.WriteLine($"A similar state was detected on spawn {spawned} (from {prevSpawned}), which gained {deltaH} height over the base of {thenH}");

            const long TotalRocks = 1000000000000;
            long rocksAfterStartup = TotalRocks - prevSpawned;
            (long totalCycles, long extra) = long.DivRem(rocksAfterStartup, spawnDelta);

            Console.WriteLine($"{totalCycles} full cycles are required, plus {extra} rock(s).");

            long expectedHeight = (long)thenH + totalCycles * deltaH;

            if (extra > 0)
            {
                KeyValuePair<WorldState, (int, int, int)> match = profile.Single(kvp => kvp.Value.Item1 == prevSpawned + extra);
                int prevExtra = prevTop - match.Value.Item3;
                Console.WriteLine($"Between spawn {prevSpawned} and {prevSpawned + extra} an extra {prevExtra} rows were added.");
                expectedHeight += prevExtra;
            }

            Console.WriteLine(expectedHeight);

            static bool Collision(byte[,] world, int left, int bottom, int shape, bool freeze)
            {
                int bp1 = bottom + 1;

                // @@@@
                if (shape == 0)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left, bp1] != 0 ||
                        world[left + 1, bp1] != 0 ||
                        world[left + 2, bp1] != 0 ||
                        world[left + 3, bp1] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom] =
                                world[left + 1, bottom] =
                                    world[left + 2, bottom] =
                                        world[left + 3, bottom] = 1;
                        }

                        return true;
                    }

                    return false;
                }

                // .@.
                // @@@
                // .@.
                if (shape == 1)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left + 1, bp1] != 0 ||
                        world[left, bottom] != 0 ||
                        world[left + 2, bottom] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom - 1] =
                                world[left + 1, bottom - 2] =
                                    world[left + 1, bottom - 1] =
                                        world[left + 1, bottom] =
                                            world[left + 2, bottom - 1] = 1;
                        }

                        return true;
                    }

                    return false;
                }

                // ..@
                // ..@
                // @@@
                if (shape == 2)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left, bp1] != 0 ||
                        world[left + 1, bp1] != 0 ||
                        world[left + 2, bp1] != 0 ||
                        world[left + 2, bottom] != 0 ||
                        world[left + 2, bottom - 1] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom] =
                                world[left + 1, bottom] =
                                    world[left + 2, bottom] =
                                        world[left + 2, bottom - 1] =
                                            world[left + 2, bottom - 2] = 1;
                        }

                        return true;
                    }

                    return false;
                }

                // @
                // @
                // @
                // @
                if (shape == 3)
                {
                    if (bp1 == BottomOfWorld ||
                        world[left, bp1] != 0 ||
                        world[left, bottom] != 0 ||
                        world[left, bottom - 1] != 0 ||
                        world[left, bottom - 2] != 0)
                    {
                        if (freeze)
                        {
                            world[left, bottom] =
                                world[left, bottom - 1] =
                                    world[left, bottom - 2] =
                                        world[left, bottom - 3] = 1;
                        }

                        return true;
                    }

                    return false;
                }

                // @@
                // @@
                Debug.Assert(shape == 4);

                if (bp1 == BottomOfWorld ||
                    world[left, bp1] != 0 ||
                    world[left + 1, bp1] != 0 ||
                    world[left, bottom] != 0 ||
                    world[left + 1, bottom] != 0)
                {
                    if (freeze)
                    {
                        world[left, bottom] =
                            world[left + 1, bottom] =
                                world[left, bottom - 1] =
                                    world[left + 1, bottom - 1] = 1;
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
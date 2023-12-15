using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day14
    {
        internal static void Problem1()
        {
            long ret = 0;
            DynamicPlane<char> plane = null;

            foreach (string s in Data.Enumerate())
            {
                if (plane is null)
                {
                    plane = new DynamicPlane<char>(s.ToCharArray());
                }
                else
                {
                    plane.PushY(s.ToCharArray());
                }
            }

            for (int y = 1; y < plane.Height; y++)
            {
                for (int x = 0; x < plane.Width; x++)
                {
                    if (plane[new Point(x, y)] == 'O')
                    {
                        int realNewY = y;

                        for (int newY = y - 1; newY >= 0; newY--)
                        {
                            if (plane[new Point(x, newY)] == '.')
                            {
                                realNewY = newY;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (realNewY < y)
                        {
                            plane[new Point(x, realNewY)] = 'O';
                            plane[new Point(x, y)] = '.';
                        }
                    }
                }
            }

            for (int y = 0; y < plane.Height; y++)
            {
                for (int x = 0; x < plane.Width; x++)
                {
                    if (plane[new Point(x, y)] == 'O')
                    {
                        ret += plane.Height - y;
                    }
                }
            }


            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            DynamicPlane<char> plane = null;

            foreach (string s in Data.Enumerate())
            {
                if (plane is null)
                {
                    plane = new DynamicPlane<char>(s.ToCharArray());
                }
                else
                {
                    plane.PushY(s.ToCharArray());
                }
            }

            Dictionary<string, int> storage = new();
            int stopCycle = int.MaxValue;

            for (int cycle = 1; cycle <= stopCycle; cycle++)
            {
                // North
                for (int y = 1; y < plane.Height; y++)
                {
                    for (int x = 0; x < plane.Width; x++)
                    {
                        if (plane[new Point(x, y)] == 'O')
                        {
                            int realNewY = y;

                            for (int newY = y - 1; newY >= 0; newY--)
                            {
                                if (plane[new Point(x, newY)] == '.')
                                {
                                    realNewY = newY;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (realNewY < y)
                            {
                                plane[new Point(x, realNewY)] = 'O';
                                plane[new Point(x, y)] = '.';
                            }
                        }
                    }
                }

                // West
                for (int x = 1; x < plane.Width; x++)
                {
                    for (int y = 0; y < plane.Height; y++)
                    {
                        if (plane[new Point(x, y)] == 'O')
                        {
                            int realNewX = x;

                            for (int newX = x - 1; newX >= 0; newX--)
                            {
                                if (plane[new Point(newX, y)] == '.')
                                {
                                    realNewX = newX;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (realNewX != x)
                            {
                                plane[new Point(realNewX, y)] = 'O';
                                plane[new Point(x, y)] = '.';
                            }
                        }
                    }
                }

                // South
                for (int y = plane.Height - 2; y >= 0; y--)
                {
                    for (int x = 0; x < plane.Width; x++)
                    {
                        if (plane[new Point(x, y)] == 'O')
                        {
                            int realNewY = y;

                            for (int newY = y + 1; newY < plane.Height; newY++)
                            {
                                if (plane[new Point(x, newY)] == '.')
                                {
                                    realNewY = newY;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (realNewY != y)
                            {
                                plane[new Point(x, realNewY)] = 'O';
                                plane[new Point(x, y)] = '.';
                            }
                        }
                    }
                }

                // East
                for (int x = plane.Width - 2; x >= 0; x--)
                {
                    for (int y = 0; y < plane.Height; y++)
                    {
                        if (plane[new Point(x, y)] == 'O')
                        {
                            int realNewX = x;

                            for (int newX = x + 1; newX < plane.Width; newX++)
                            {
                                if (plane[new Point(newX, y)] == '.')
                                {
                                    realNewX = newX;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (realNewX != x)
                            {
                                plane[new Point(realNewX, y)] = 'O';
                                plane[new Point(x, y)] = '.';
                            }
                        }
                    }
                }


                if (stopCycle == int.MaxValue)
                {
                    string state = plane.Print(c => c);

                    if (storage.TryGetValue(state, out int existing))
                    {
                        int remaining = 1000000000 - cycle;
                        (int fullCycles, int remainder) = int.DivRem(remaining, cycle - existing);
                        stopCycle = cycle + remainder;
                        Console.WriteLine($"Cycle {cycle} is the same as cycle {existing}, so there are {fullCycles} more full loops and {remainder} incremental steps remaining.");
                    }
                    else
                    {
                        storage[state] = cycle;

                        if (cycle < 3)
                        {
                            Utils.TraceForSample($"After cycle {cycle}");
                            Utils.TraceForSample(state);
                        }
                    }
                }
            }

            for (int y = 0; y < plane.Height; y++)
            {
                for (int x = 0; x < plane.Width; x++)
                {
                    if (plane[new Point(x, y)] == 'O')
                    {
                        ret += plane.Height - y;
                    }
                }
            }

            Utils.TraceForSample(plane.Print());

            Console.WriteLine(ret);
        }
    }
}
using System;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day01
    {
        internal static void Problem1()
        {
            long ret = 0;
            int pos = 50;

            foreach (string s in Data.Enumerate())
            {
                char dir = s[0];
                int val = int.Parse(s[1..]);

                if (dir == 'L')
                {
                    pos -= val;
                }
                else if (dir == 'R')
                {
                    pos += val;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                pos %= 100;

                if (pos == 0)
                {
                    ret++;
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            int pos = 50;

            foreach (string s in Data.Enumerate())
            {
                char dir = s[0];
                int val = int.Parse(s[1..]);

                (int fullRot, val) = int.DivRem(val, 100);
                ret += fullRot;


                if (dir == 'L')
                {
                    if (pos == 0)
                        ret--;

                    pos -= val;

                    if (pos <= 0)
                    {
                        ret++;
                    }

                    pos = (pos + 100) % 100;
                }
                else if (dir == 'R')
                {
                    pos += val;

                    if (pos >= 100)
                    {
                        ret++;
                        pos -= 100;
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                Utils.TraceForSample($"{s} is {fullRot} full and stops at {pos} => {ret}");
            }

            Console.WriteLine(ret);
        }
    }
}

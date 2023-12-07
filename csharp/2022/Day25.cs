using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day25
    {
        private static long SnafuToInt(string snafu)
        {
            ReadOnlySpan<char> text = snafu;

            long ret = 0;

            while (!text.IsEmpty)
            {
                int local = text[0] switch
                {
                    '2' => 2,
                    '1' => 1,
                    '0' => 0,
                    '-' => -1,
                    '=' => -2,
                };

                ret *= 5;
                ret += local;

                text = text[1..];
            }

            return ret;
        }

        private static string IntToSnafu(long val)
        {
            long left = val;
            Stack<char> stack = new Stack<char>();

            while (left != 0)
            {
                long pos = left % 5;

                if (pos <= 2)
                {
                    stack.Push((char)(pos + '0'));
                }
                else
                {
                    stack.Push(pos switch { 4 => '-', 3 => '=' });
                    left += (5 - pos);
                }

                left /= 5;
            }

            return new string(stack.ToArray());
        }

        internal static void Problem1()
        {
            long sum = 0;

            foreach (string snafu in Data.Enumerate())
            {
                if (!string.IsNullOrEmpty(snafu))
                {
                    long decoded = SnafuToInt(snafu);
                    Console.WriteLine($"{snafu} => {decoded}");
                    sum += decoded;
                }
            }

            Console.WriteLine(sum);
            Console.WriteLine(IntToSnafu(sum));
        }

        internal static void Problem2x()
        {
        }
    }
}
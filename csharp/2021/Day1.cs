using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public static class Day1
    {
        internal static void Problem1()
        {
            int last = int.MaxValue;
            int increased = 0;

            foreach (string line in Data.Enumerate())
            {
                int cur = int.Parse(line);

                if (cur > last)
                {
                    increased++;
                }

                last = cur;
            }

            Console.WriteLine(increased);
        }

        internal static void Problem2()
        {
            int one, two;
            int last = int.MaxValue;
            int increased = 0;

            {
                IEnumerator<string> enumerator = Data.Enumerate().GetEnumerator();
                enumerator.MoveNext();
                one = int.Parse(enumerator.Current);

                enumerator.MoveNext();
                two = int.Parse(enumerator.Current);

                enumerator.Dispose();
            }

            foreach (string line in Data.Enumerate().Skip(2))
            {
                int cur = int.Parse(line);
                int rolling = one + two + cur;

                if (rolling > last)
                {
                    increased++;
                }

                last = rolling;
                one = two;
                two = cur;
            }

            Console.WriteLine(increased);
        }
    }
}

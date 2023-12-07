using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day7
    {
        internal static void Problem1()
        {
            Run((pos, i) => Math.Abs(pos - i));
        }

        internal static void Problem2()
        {
            Run((pos, i) =>
            {
                int delta = Math.Abs(pos - i);

                if ((delta % 2) == 0)
                {
                    return delta / 2 * (delta + 1);
                }

                return (delta + 1) / 2 * delta;
            });
        }

        private static void Run(Func<int,int,int> costFunc)
        {
            List<int> crabs = new List<int>();

            foreach (string line in Data.Enumerate())
            {
                ReadOnlySpan<char> lineSpan = line;
                int commaIdx;

                while ((commaIdx = lineSpan.IndexOf(',')) >= 0)
                {
                    crabs.Add(int.Parse(lineSpan.Slice(0, commaIdx)));
                    lineSpan = lineSpan.Slice(commaIdx + 1);
                }

                crabs.Add(int.Parse(lineSpan));
            }

            int min = crabs.Min();
            int max = crabs.Max();
            long lowestCost = long.MaxValue;
            int lowestPos = -1;

            for (int i = min; i <= max; i++)
            {
                int cost = 0;

                foreach (int pos in crabs)
                {
                    cost += costFunc(pos, i);
                }

                if (cost < lowestCost)
                {
                    lowestPos = i;
                    lowestCost = cost;
                }
            }

            Console.WriteLine($"Lowest cost is {lowestCost} at position {lowestPos}");
        }
    }
}
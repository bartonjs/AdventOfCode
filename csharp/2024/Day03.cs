using System;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day03
    {
        internal static void Problem1()
        {
            long ret = 0;
            Regex regex = new Regex("mul\\((\\d+),(\\d+)\\)");

            foreach (string line in Data.Enumerate())
            {
                foreach (Match match in regex.Matches(line))
                {
                    if (match.Groups[1].Length <= 3 && match.Groups[2].Length <= 3)
                    {
                        long part = long.Parse(match.Groups[1].ValueSpan) * long.Parse(match.Groups[2].ValueSpan);
                        ret += part;
                    }
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            Regex regex = new Regex("(?:mul\\((\\d+),(\\d+)\\)|do\\(\\)|don't\\(\\))");
            bool include = true;

            foreach (string line in Data.Enumerate())
            {
                foreach (Match match in regex.Matches(line))
                {
                    if (match.ValueSpan is "do()")
                    {
                        include = true;
                    }
                    else if (match.ValueSpan is "don't()")
                    {
                        include = false;
                    }
                    else if (include && match.Groups[1].Length <= 3 && match.Groups[2].Length <= 3)
                    {
                        long part = long.Parse(match.Groups[1].ValueSpan) * long.Parse(match.Groups[2].ValueSpan);
                        ret += part;
                    }
                }
            }

            Console.WriteLine(ret);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day09
    {
        internal static void Problem1()
        {
            long ret = 0;

            Stack<List<long>> diffStack = new();

            foreach (string s in Data.Enumerate())
            {
                List<long> baseLine = new List<long>();
                baseLine.AddRange(s.Split(' ').Select(long.Parse));

                List<long> diffs;

                do
                {
                    diffs = new(baseLine.Count);
                    diffStack.Push(baseLine);

                    long last = baseLine.First();

                    foreach (long val in baseLine.Skip(1))
                    {
                        diffs.Add(val - last);
                        last = val;
                    }

                    baseLine = diffs;
                } while (diffs.Any(l => l != 0));

                diffs.Add(0);

                while (diffStack.Count > 0)
                {
                    baseLine = diffStack.Pop();
                    long toAdd = baseLine.Last() + diffs.Last();
                    Utils.TraceForSample($"Adding {toAdd}");
                    baseLine.Add(toAdd);
                    diffs = baseLine;
                }

                Utils.TraceForSample($"Counting {baseLine.Last()} in answer...");
                ret += baseLine.Last();
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            Stack<List<long>> diffStack = new();

            foreach (string s in Data.Enumerate())
            {
                List<long> baseLine = new List<long>();
                baseLine.AddRange(s.Split(' ').Select(long.Parse));

                List<long> diffs;

                do
                {
                    diffs = new(baseLine.Count);
                    diffStack.Push(baseLine);

                    long last = baseLine.First();

                    foreach (long val in baseLine.Skip(1))
                    {
                        diffs.Add(val - last);
                        last = val;
                    }

                    baseLine = diffs;
                } while (diffs.Any(l => l != 0));

                diffs.Insert(0, 0);

                while (diffStack.Count > 0)
                {
                    baseLine = diffStack.Pop();
                    long toAdd = baseLine[0] - diffs[0];
                    Utils.TraceForSample($"Adding {toAdd}");
                    baseLine.Insert(0, toAdd);
                    diffs = baseLine;
                }

                Utils.TraceForSample($"Counting {baseLine.First()} in answer...");
                ret += baseLine[0];
            }

            Console.WriteLine(ret);
        }
    }
}
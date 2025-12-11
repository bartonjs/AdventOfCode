using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day11
    {
        private static Dictionary<string, string[]> Load()
        {
            Dictionary<string, string[]> ret = new();

            foreach (string s in Data.Enumerate())
            {
                int colon = s.IndexOf(':');
                string key = s.Substring(0, colon);
                string[] dests = s.Substring(colon + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                ret.Add(key, dests);
            }

            return ret;
        }

        internal static void Problem1()
        {
            long ret = 0;

            Dictionary<string, string[]> map = Load();

            Queue<string> queue = new();
            queue.Enqueue("you");

            while (queue.TryDequeue(out string node))
            {
                if (node == "out")
                {
                    ret++;
                    continue;
                }

                foreach (string dest in map[node])
                {
                    queue.Enqueue(dest);
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            Dictionary<string, string[]> map = Load();

            Console.WriteLine(Count(map, new VisitState("svr", 0), []));

            static long Count(
                Dictionary<string, string[]> map,
                VisitState visitState,
                Dictionary<VisitState, long> cache)
            {
                if (cache.TryGetValue(visitState, out long value))
                {
                    return value;
                }

                if (visitState.Name == "out")
                {
                    return visitState.State == 3 ? 1 : 0;
                }

                int nextVisitState = visitState.State;

                if (visitState.Name == "dac")
                {
                    nextVisitState |= 1;
                }
                else if (visitState.Name == "fft")
                {
                    nextVisitState |= 2;
                }

                value = 0;

                foreach (string next in map[visitState.Name])
                {
                    value += Count(map, new VisitState(next, nextVisitState), cache);
                }

                cache[visitState] = value;
                return value;
            }
        }

        private readonly record struct VisitState(string Name, int State);
    }
}

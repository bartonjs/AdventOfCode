using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day05
    {
        private static (Dictionary<long, HashSet<long>> OrderRules, List<List<long>> Updates) Load()
        {
            Dictionary<long, HashSet<long>> orderRules = new();
            List<List<long>> updates = null;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrEmpty(s))
                {
                    updates = new();
                    continue;
                }

                if (updates is not null)
                {
                    updates.Add(s.Split(',').Select(long.Parse).ToList());
                }
                else
                {
                    string[] parts = s.Split('|');

                    ref HashSet<long> rules =
                        ref CollectionsMarshal.GetValueRefOrAddDefault(orderRules, long.Parse(parts[0]), out _);

                    if (rules is null)
                    {
                        rules = new HashSet<long>();
                    }

                    rules.Add(long.Parse(parts[1]));
                }
            }

            return (orderRules, updates);
        }

        private static bool MoveEarlier(long candidate, long prev, Dictionary<long, HashSet<long>> rules)
        {
            if (rules.TryGetValue(candidate, out HashSet<long> set))
            {
                return set.Contains(prev);
            }

            return false;
        }

        internal static void Problem1()
        {
            long ret = 0;

            (Dictionary<long, HashSet<long>> orderRules, List<List<long>> updates) = Load();

            foreach (List<long> update in updates)
            {
                if (!update.PredicateSort((cur, prev) => MoveEarlier(cur, prev, orderRules)))
                {
                    ret += update[update.Count / 2];
                }
            }
            
            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            (Dictionary<long, HashSet<long>> orderRules, List<List<long>> updates) = Load();

            foreach (List<long> update in updates)
            {
                if (update.PredicateSort((cur, prev) => MoveEarlier(cur, prev, orderRules)))
                {
                    ret += update[update.Count / 2];
                }
            }

            Console.WriteLine(ret);
        }
    }
}
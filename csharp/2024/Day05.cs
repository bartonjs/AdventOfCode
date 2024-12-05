using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day05
    {
        private static (KeyedSets<long> OrderRules, List<List<long>> Updates) Load()
        {
            KeyedSets<long> orderRules = new();
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
                    orderRules.Add(long.Parse(parts[0]), long.Parse(parts[1]));
                }
            }

            return (orderRules, updates);
        }

        private static bool MoveEarlier(long candidate, long prev, KeyedSets<long> rules) =>
            rules.Contains(candidate, prev);

        internal static void Problem1()
        {
            long ret = 0;

            (KeyedSets<long> orderRules, List<List<long>> updates) = Load();

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

            (KeyedSets<long> orderRules, List<List<long>> updates) = Load();

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
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day08
    {
        internal static void Problem1()
        {
            string directions = null;
            Dictionary<string, (string Left, string Right)> map = new();

            foreach (string s in Data.Enumerate())
            {
                if (directions is null)
                {
                    directions = s;
                    continue;
                }

                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }

                string from = s.Substring(0, 3);
                string left = s.Substring(7, 3);
                string right = s.Substring(12, 3);

                map.Add(from, (left, right));
            }

            int counter = 0;
            string current = "AAA";

            while (current != "ZZZ")
            {
                int idx = counter % directions.Length;
                counter++;

                if (directions[idx] == 'L')
                {
                    current = map[current].Left;
                }
                else
                {
                    current = map[current].Right;
                }
            }

            Console.WriteLine(counter);
        }

        internal static void Problem2()
        {
            string directions = null;
            Dictionary<string, (string Left, string Right)> map = new();
            List<string> ghostPositions = new();
            List<string> endingPositions = new();

            foreach (string s in Data.Enumerate())
            {
                if (directions is null)
                {
                    directions = s;
                    continue;
                }

                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }

                string from = s.Substring(0, 3);
                string left = s.Substring(7, 3);
                string right = s.Substring(12, 3);

                map.Add(from, (left, right));

                if (from.EndsWith('A'))
                {
                    ghostPositions.Add(from);
                }
                else if (from.EndsWith('Z'))
                {
                    endingPositions.Add(from);
                }
            }

            List<List<long>> relativeCounts = new(ghostPositions.Count);

            for (int ghostId = 0; ghostId < ghostPositions.Count; ghostId++)
            {
                int counter = 0;
                string current = ghostPositions[ghostId];
                List<long> candidates = new();
                relativeCounts.Add(candidates);
                HashSet<string> ender = new HashSet<string>(endingPositions);

                string start = current;

                while (ender.Count > 0)
                {
                    if (current.EndsWith('Z') && ender.Remove(current))
                    {
                        candidates.Add(counter);
                        // Let's see if only the first ending for each ghost matters.
                        // Update: Worked for me.  Phew.
                        break;
                    }

                    int idx = counter % directions.Length;
                    counter++;

                    if (directions[idx] == 'L')
                    {
                        current = map[current].Left;
                    }
                    else
                    {
                        current = map[current].Right;
                    }

                    if (current == start && idx == directions.Length - 1)
                    {
                        break;
                    }
                }
            }

            List<long> maybe = relativeCounts.Select(Utils.LeastCommonMultiple).ToList();
            Console.WriteLine(Utils.LeastCommonMultiple(maybe));
        }
    }
}
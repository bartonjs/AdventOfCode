using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day25
    {
        internal static void Problem1()
        {
            Dictionary<string, List<string>> graph = new();

            foreach (string s in Data.Enumerate())
            {
                string key = s.Substring(0, 3);
                string[] rest = s.Substring(5).Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (string peer in rest)
                {
                    graph.GetOrAdd(key, () => new()).Add(peer);
                    graph.GetOrAdd(peer, () => new()).Add(key);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                var criticalLink = FindCriticalLink(graph);
                Console.WriteLine($"Removing link {criticalLink.Item1} <-> {criticalLink.Item2}");
                graph[criticalLink.Item1].Remove(criticalLink.Item2);
                graph[criticalLink.Item2].Remove(criticalLink.Item1);
            }

            HashSet<string> partition = new();

            Pathing.BreadthFirstSearch(
                graph,
                graph.First().Key,
                v =>
                {
                    partition.Add(v);
                    return false;
                },
                (k, d) => d[k]);

            Console.WriteLine($"Partition is of size {partition.Count}");
            long graphCount = graph.Count;
            Console.WriteLine($"Graph was originally of size {graphCount}");
            Console.WriteLine((graphCount - partition.Count) * partition.Count);
        }

        private static (string, string) FindCriticalLink(Dictionary<string, List<string>> graph)
        {
            Queue<string> queue = new();
            HashSet<string> seen = new();
            Dictionary<(string, string), long> counts = new();

            foreach (string node in graph.Keys)
            {
                queue.Enqueue(node);
                seen.Clear();

                while (queue.TryDequeue(out string next))
                {
                    foreach (string peer in graph[next])
                    {
                        if (!seen.Add(peer))
                        {
                            continue;
                        }

                        queue.Enqueue(peer);
                        counts.Increment(MakeEdge(next, peer));
                    }
                }
            }

            return counts.MaxBy(kvp => kvp.Value).Key;
        }

        private static (string, string) MakeEdge(string node1, string node2)
        {
            if (string.Compare(node1, node2, StringComparison.Ordinal) < 0)
            {
                return (node1, node2);
            }

            return (node2, node1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day23
    {
        internal static void Problem1()
        {
            long ret = 0;
            List<(string, string)> pairs = new List<(string, string)>();
            HashSet<string> allNodes = new();

            foreach (string s in Data.Enumerate())
            {
                int hyphen = s.IndexOf('-');
                string left = s.AsSpan(0, hyphen).ToString();
                string right = s.AsSpan(hyphen + 1).ToString();

                pairs.Add((left, right));
                pairs.Add((right, left));
                allNodes.Add(left);
                allNodes.Add(right);
            }

            HashSet<string> dedup = new();
            List<List<string>> graphs = new();
            Stack<string> work = new Stack<string>();

            HashSet<string> networks = new();

            foreach (string target in allNodes)
            {
                work.Push(target);
                MakeGraphs(work, pairs, dedup, graphs);
                work.Pop();

                foreach (var list in graphs)
                {
                    if (list.Count == 3 && list.Any(val => val.StartsWith('t')))
                    {
                        string str = string.Join(',', list.OrderBy(t => t));

                        if (networks.Add(str))
                        {
                            ret++;
                            Print.ForSample(str);
                        }
                    }
                }
            }

            Console.WriteLine(ret);
        }

        
        internal static void Problem2()
        {
            KeyedSets<string> map = new();
            HashSet<string> allNodes = new();

            foreach (string s in Data.Enumerate())
            {
                int hyphen = s.IndexOf('-');
                string left = s.AsSpan(0, hyphen).ToString();
                string right = s.AsSpan(hyphen + 1).ToString();

                map.Add(left, right);
                map.Add(right, left);
                allNodes.Add(left);
                allNodes.Add(right);
            }

            HashSet<string> work = new();
            List<HashSet<string>> allGraphs = MakeGraphs(map);
            HashSet<string> max = allGraphs.MaxBy(g => g.Count);

            string str = string.Join(',', max.OrderBy(t => t));
            Console.WriteLine(str);
        }

        private static List<HashSet<string>> MakeGraphs(
            KeyedSets<string> map)
        {
            List<HashSet<string>> groups = new();

            foreach (string key in map.Keys)
            {
                HashSet<string> newClique = [key];
                HashSet<string> processed = new();
                Queue<string> queue = new();
                queue.Enqueue(key);

                while (queue.Count > 0)
                {
                    string node = queue.Dequeue();
                    HashSet<string> helper = new HashSet<string>(newClique);
                    helper.Remove(node);

                    if (processed.Add(node))
                    {
                        int before = helper.Count;
                        HashSet<string> nodeTargets = map[node];
                        helper.IntersectWith(nodeTargets);

                        if (helper.Count >= before)
                        {
                            newClique.Add(node);

                            foreach (string next in nodeTargets)
                            {
                                if (!processed.Contains(next))
                                {
                                    queue.Enqueue(next);
                                }
                            }
                        }
                    }
                }

                groups.Add(newClique);
            }

            return groups;
        }

        private static void MakeGraphs(Stack<string> work, List<(string, string)> pairs, HashSet<string> dedup, List<List<string>> accum)
        {
            string target = work.Peek();

            foreach (var pair in pairs)
            {
                if (pair.Item1 == target)
                {
                    if (work.Contains(pair.Item2))
                    {
                        if (work.Last() == pair.Item2)
                        {
                            string manifest = string.Join(',', work);

                            if (dedup.Add(manifest))
                            {
                                accum.Add(work.ToList());
                            }
                        }
                    }
                    else if (work.Count < 3)
                    {
                        work.Push(pair.Item2);
                        MakeGraphs(work, pairs, dedup, accum);
                        work.Pop();
                    }
                }
            }
        }
    }
}
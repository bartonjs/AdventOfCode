using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Stopwatch sw = Stopwatch.StartNew();
            List<HashSet<string>> allGraphs = MakeGraphs(map);
            sw.Stop();
            HashSet<string> max = allGraphs.MaxBy(g => g.Count);

            string str = string.Join(',', max.OrderBy(t => t));
            Console.WriteLine(str);
            //Console.WriteLine(str == "cm,de,ez,gv,hg,iy,or,pw,qu,rs,sn,uc,wq");

#if STATS
            Stats(allGraphs, sw, "Rando");

            List<HashSet<string>> bk = new();
            sw.Restart();
            BronKerbosch(new HashSet<string>(), new HashSet<string>(map.Keys), new HashSet<string>(), bk, map);
            sw.Stop();

            Stats(bk, sw, "Bron-Kerbosch");

            List<HashSet<string>> bkp = new();
            sw.Restart();
            BronKerboschWithPivot(new HashSet<string>(), new HashSet<string>(map.Keys), new HashSet<string>(), bkp, map);
            sw.Stop();

            Stats(bk, sw, "Bron-Kerbosch with Pivot");

            static void Stats(IEnumerable<HashSet<string>> graphs, Stopwatch sw, string label)
            {
                Dictionary<int, int> counts = new();
                Console.WriteLine($"BEGIN {label}:");
                HashSet<string> distinct = new();

                foreach (HashSet<string> graph in graphs)
                {
                    if (distinct.Add(string.Join(',', graph.OrderBy(t => t))))
                    {
                        counts.Increment(graph.Count);
                    }
                }

                foreach (var kvp in counts.OrderBy(kvp => kvp.Key))
                {
                    Console.WriteLine($"Length {kvp.Key}: {kvp.Value}");
                }

                Console.WriteLine($"Runtime: {sw.Elapsed.TotalMilliseconds:F4}ms");
                Console.WriteLine();
            }
#endif
        }

        private static void BronKerbosch(
            HashSet<string> r,
            HashSet<string> p,
            HashSet<string> x,
            List<HashSet<string>> accum,
            KeyedSets<string> map)
        {
            if (p.Count == 0 && x.Count == 0)
            {
                accum.Add(new HashSet<string>(r));
                return;
            }

            foreach (string vertex in p)
            {
                r.Add(vertex);
                HashSet<string> pIntersectNv = new HashSet<string>(map[vertex]);
                pIntersectNv.IntersectWith(p);
                HashSet<string> xIntersectNv = new HashSet<string>(map[vertex]);
                xIntersectNv.IntersectWith(x);

                BronKerbosch(r, pIntersectNv, xIntersectNv, accum, map);
                r.Remove(vertex);
                p.Remove(vertex);
                x.Add(vertex);
            }
        }

        private static void BronKerboschWithPivot(
            HashSet<string> r,
            HashSet<string> p,
            HashSet<string> x,
            List<HashSet<string>> accum,
            KeyedSets<string> map)
        {
            if (p.Count == 0 && x.Count == 0)
            {
                accum.Add(new HashSet<string>(r));
                return;
            }

            string u = p.Count > 0 ? p.First() : x.First();
            HashSet<string> nOfU = map[u];

            foreach (string vertex in p)
            {
                if (nOfU.Contains(vertex))
                {
                    continue;
                }

                r.Add(vertex);
                HashSet<string> pIntersectNv = new HashSet<string>(map[vertex]);
                pIntersectNv.IntersectWith(p);
                HashSet<string> xIntersectNv = new HashSet<string>(map[vertex]);
                xIntersectNv.IntersectWith(x);

                BronKerbosch(r, pIntersectNv, xIntersectNv, accum, map);
                r.Remove(vertex);
                p.Remove(vertex);
                x.Add(vertex);
            }
        }

        // This algorithm is fast, but seems to rely a lot on luck.
        // For a true answer, see BronKerbosch (with or without pivot)
        private static List<HashSet<string>> MakeGraphs(
            KeyedSets<string> map)
        {
            List<HashSet<string>> groups = new();

            foreach (string key in map.Keys)
            {
                HashSet<string> newClique = [];
                HashSet<string> processed = new();
                Queue<string> queue = new();
                queue.Enqueue(key);

                while (queue.TryDequeue(out string node))
                {
                    HashSet<string> nodeTargets = map[node];
                    
                    if (processed.Add(node))
                    {
                        if (nodeTargets.Count >= newClique.Count - 1)
                        {
                            if (nodeTargets.IsSupersetOf(newClique))
                            {
                                newClique.Add(node);

                                foreach (string next in nodeTargets.OrderByDescending(a => map[a].Count))
                                {
                                    if (!processed.Contains(next))
                                    {
                                        queue.Enqueue(next);
                                    }
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
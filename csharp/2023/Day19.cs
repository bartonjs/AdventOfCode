using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day19
    {
        internal static void Problem1()
        {
            (Dictionary<string, RuleSet> rules, List<Part> parts) = Load();
            long ret = 0;

            foreach (Part part in parts)
            {
                string engine = "in";
                Utils.TraceForSample($"Starting part {part.OriginalString}");

                while (true)
                {
                    string nextEngine = rules[engine].Evaluate(part);
                    Utils.TraceForSample($"{engine} => {nextEngine}");
                    engine = nextEngine;

                    if (engine == "A")
                    {
                        long score = part.Score();
                        Utils.TraceForSample($"Accepted part with score {score}");
                        ret += score;
                        break;
                    }

                    if (engine == "R")
                    {
                        break;
                    }
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            (Dictionary<string, RuleSet> rules, List<Part> parts) = Load();
            Cache<TreeState, long> cache = new();

            long fullCount = Count(rules, cache, new TreeState());

            Console.WriteLine(fullCount);
            Console.WriteLine($"Cache hits: {cache.Hits}, misses: {cache.Misses}");

            static long Count(Dictionary<string, RuleSet> rules, Cache<TreeState, long> cache, TreeState state)
            {
                if (cache.TryGetValue(state, out long count))
                {
                    return count;
                }

                if (state.Workflow == "R")
                {
                    return 0;
                }

                count = state.Count();

                if (state.Workflow == "A")
                {
                    return count;
                }

                if (state.Count() == 0)
                {
                    return 0;
                }

                TreeState nextPart = state;
                RuleSet ruleSet = rules[state.Workflow];

                count = 0;

                foreach (Rule rule in ruleSet.Rules)
                {
                    (TreeState matchState, TreeState elseState) = nextPart.Intersect(rule);

                    if (matchState is not null)
                    {
                        count += Count(rules, cache, matchState);
                    }

                    nextPart = elseState;

                    if (elseState is null)
                    {
                        break;
                    }
                }

                if (nextPart is not null)
                {
                    nextPart.Workflow = ruleSet.DefaultDestination;
                    count += Count(rules, cache, nextPart);
                }

                return cache[state] = count;
            }
        }

        private class TreeState : IEquatable<TreeState>
        {
            public string Workflow { get; set; }
            public Range X { get; private set; }
            public Range M { get; private set; }
            public Range A { get; private set; }
            public Range S { get; private set; }

            public TreeState()
            {
                X = new Range(1, 4000);
                M = new Range(1, 4000);
                A = new Range(1, 4000);
                S = new Range(1, 4000);
                Workflow = "in";
            }

            private TreeState(TreeState copyFrom)
            {
                X = copyFrom.X;
                M = copyFrom.M;
                A = copyFrom.A;
                S = copyFrom.S;
            }

            public long Count()
            {
                return Width(X) * Width(M) * Width(A) * Width(S);
            }

            private static long Width(in Range range)
            {
                return long.Max(0, range.End.Value - range.Start.Value + 1);
            }

            private Range GetRange(char metric)
            {
                return metric switch
                {
                    'x' => X,
                    'm' => M,
                    'a' => A,
                    's' => S,
                };
            }

            private void SetRange(char metric, Range value)
            {
                _ = metric switch
                {
                    'x' => X = value,
                    'm' => M = value,
                    'a' => A = value,
                    's' => S = value,
                };
            }

            public (TreeState Match, TreeState Else) Intersect(Rule rule)
            {
                TreeState yes = null;
                TreeState no = null;

                int target = rule.Value;
                Range range = GetRange(rule.Metric);

                if (rule.Op == '<')
                {
                    Range lesser = new Range(range.Start, int.Min(target - 1, range.End.Value));
                    Range greater = new Range(int.Max(range.Start.Value, target), range.End);

                    if (lesser.Start.Value <= lesser.End.Value)
                    {
                        yes = new TreeState(this)
                        {
                            Workflow = rule.Destination,
                        };

                        yes.SetRange(rule.Metric, lesser);
                    }

                    if (greater.Start.Value <= greater.End.Value)
                    {
                        no = new TreeState(this)
                        {
                            Workflow = Workflow,
                        };

                        no.SetRange(rule.Metric, greater);
                    }
                }
                else
                {
                    Range greater = new Range(int.Max(range.Start.Value, target + 1), range.End);
                    Range lesser = new Range(range.Start, int.Min(target, range.End.Value));

                    if (lesser.Start.Value <= lesser.End.Value)
                    {
                        no = new TreeState(this)
                        {
                            Workflow = Workflow,
                        };

                        no.SetRange(rule.Metric, lesser);
                    }

                    if (greater.Start.Value <= greater.End.Value)
                    {
                        yes = new TreeState(this)
                        {
                            Workflow = rule.Destination,
                        };

                        yes.SetRange(rule.Metric, greater);
                    }
                }

                return (yes, no);
            }

            public bool Equals(TreeState other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Workflow == other.Workflow && X.Equals(other.X) && M.Equals(other.M) && A.Equals(other.A) && S.Equals(other.S);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TreeState)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Workflow, X, M, A, S);
            }
        }

        private static (Dictionary<string, RuleSet> Rules, List<Part> Parts) Load()
        {
            Dictionary<string, RuleSet> rules = new();
            List<Part> parts = null;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    if (parts is null)
                    {
                        parts = new();
                    }

                    continue;
                }

                if (parts is null)
                {
                    RuleSet ruleSet = RuleSet.Parse(s);
                    rules[ruleSet.Name] = ruleSet;
                }
                else
                {
                    parts.Add(Part.Parse(s));
                }
            }

            return (rules, parts);
        }

        private record struct Rule(char Metric, char Op, int Value, string Destination);

        private class RuleSet
        {
            public string Name { get; private set; }

            public List<Rule> Rules { get; } = new();
            public string DefaultDestination { get; private set; }

            public static RuleSet Parse(string s)
            {
                int idx = s.IndexOf('{');
                string name = s.Substring(0, idx);
                string[] parts = s.Substring(idx + 1, s.Length - idx - 2).Split(',');

                RuleSet set = new RuleSet
                {
                    Name = name,
                };

                foreach (string part in parts.SkipLast(1))
                {
                    char metric = part[0];
                    char op = part[1];
                    ReadOnlySpan<char> cdr = part.AsSpan(2);
                    int colon = cdr.IndexOf(':');
                    int value = int.Parse(cdr.Slice(0, colon));
                    string dest = cdr.Slice(colon + 1).ToString();

                    set.Rules.Add(new Rule(metric, op, value, dest));
                }

                set.DefaultDestination = parts[^1];

                return set;
            }

            public string Evaluate(Part part)
            {
                foreach (var rule in Rules)
                {
                    int metric = part.Metrics[rule.Metric];
                    int target = rule.Value;

                    if (rule.Op == '>')
                    {
                        if (metric > target)
                        {
                            return rule.Destination;
                        }
                    }
                    else
                    {
                        Debug.Assert(rule.Op == '<');

                        if (metric < target)
                        {
                            return rule.Destination;
                        }
                    }
                }

                return DefaultDestination;
            }
        }

        private class Part
        {
            public string OriginalString { get; private set; }
            public Dictionary<char, int> Metrics = new();

            public static Part Parse(string s)
            {
                Part ret = new()
                {
                    OriginalString = s,
                };

                s = s.Substring(1, s.Length - 2);
                string[] parts = s.Split(',');

                foreach (string part in parts)
                {
                    ret.Metrics[part[0]] = int.Parse(part.AsSpan(2));
                }

                return ret;
            }

            public long Score()
            {
                return Metrics.Values.Sum();
            }
        }
    }
}
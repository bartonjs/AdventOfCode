using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day16
    {
        [GeneratedRegex(@"Valve (..) has flow rate=(\d+); tunnels? leads? to valves? (.*)")]
        private static partial Regex MatchRegex();

        [DebuggerDisplay("{Name} - {Rate}")]
        private class Valve
        {
            public string Name;
            public int Index;
            public int Rate;
            public List<string> Connections;
        }

        [DebuggerDisplay("{CurrentNode} - {Remaining}")]
        private struct State : IEquatable<State>
        {
            public string CurrentNode;
            public long Remaining;

            public bool Equals(State other)
            {
                return CurrentNode == other.CurrentNode && Remaining == other.Remaining;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((State)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(CurrentNode, Remaining);
            }
        }

        private static List<Valve> Load()
        {
            Regex regex = MatchRegex();
            List<Valve> valves = new List<Valve>();
            int index = 0;

            foreach (string s in Data.Enumerate())
            {
                Match m = regex.Match(s);

                Valve v = new Valve
                {
                    Name = m.Groups[1].Value,
                    Index = index,
                    Rate = int.Parse(m.Groups[2].ValueSpan),
                    Connections = new List<string>(),
                };

                index++;
                ReadOnlySpan<char> conns = m.Groups[3].ValueSpan;

                while (!conns.IsEmpty)
                {
                    int commaIdx = conns.IndexOf(',');

                    if (commaIdx >= 0)
                    {
                        v.Connections.Add(conns.Slice(0, commaIdx).ToString());
                        conns = conns.Slice(commaIdx + 2);
                    }
                    else
                    {
                        v.Connections.Add(conns.ToString());
                        break;
                    }
                }

                valves.Add(v);
            }

            return valves;
        }

        private static List<Valve> Valves { get; } = Load();
        private static Dictionary<string, Valve> ValvesByName { get; } = Valves.ToDictionary(x => x.Name);
        private static int CacheHits;
        private static int CacheMisses;
        private static int CacheIgnoreds;

        private static void Stuff(Dictionary<State, (int Score, int Remain)> states, Valve candidate, State currentState, int remain, int score)
        {
            currentState.CurrentNode = candidate.Name;

            ref (int Score, int Remain) cur = ref CollectionsMarshal.GetValueRefOrAddDefault(states, currentState, out bool exists);

            if (exists)
            {
                if (cur.Score > score)
                {
                    CacheHits++;
                    return;
                }
                
                if (cur.Score == score && cur.Remain >= remain)
                {
                    CacheHits++;
                    return;
                }

                CacheIgnoreds++;
            }
            else
            {
                CacheMisses++;
            }

            cur = (score, remain);

            int rm1 = remain - 1;

            if (remain > 3)
            {
                foreach (string conn in candidate.Connections)
                {
                    Valve next = ValvesByName[conn];
                    Stuff(states, next, currentState, rm1, score);
                }
            }

            if (candidate.Rate > 0)
            {
                long me = 1L << candidate.Index;

                if ((currentState.Remaining & me) != 0)
                {
                    currentState.Remaining &= ~me;
                    int localScore = rm1 * candidate.Rate;
                    score += localScore;

                    ref (int Score, int Remain) cur2 = ref CollectionsMarshal.GetValueRefOrAddDefault(states, currentState, out _);

                    if (score > cur2.Score)
                    {
                        cur2 = (score, rm1);
                    }

                    if (currentState.Remaining == 0)
                    {
                        return;
                    }

                    if (remain > 3)
                    {
                        int rm2 = rm1 - 1;

                        foreach (string conn in candidate.Connections)
                        {
                            Valve next = ValvesByName[conn];
                            Stuff(states, next, currentState, rm2, score);
                        }
                    }
                }
            }
        }

        private static KeyValuePair<State, (int Score, int Remain)> MaxState(Dictionary<State, (int, int)> states)
        {
            return states.MaxBy(kvp => kvp.Value.Item1);
        }

        internal static void Problem1()
        {
            Valve start = ValvesByName["AA"];

            long available = 0;

            foreach (Valve v in Valves)
            {
                if (v.Rate > 0)
                {
                    available |= (1L << v.Index);
                }
            }

            State startState = new State { Remaining = available };
            Dictionary<State, (int, int)> states = new();
            Stuff(states, start, startState, 30, 0);
            KeyValuePair<State, (int Score, int Remain)> pair = MaxState(states);
            Console.WriteLine($"{states.Count} known state(s).");
            Console.WriteLine($"Cache hits: {CacheHits}");
            Console.WriteLine($"Cache misses: {CacheMisses}");
            Console.WriteLine($"Cache had an entry but I did work anyways: {CacheIgnoreds}");
            Console.WriteLine();
            Console.WriteLine(pair.Value);
            Console.WriteLine();
        }

        private record struct State2(int CurrentNode, long ClosedValves, int TimeRemaining, int ActorId);

        private static int Stuff2(Dictionary<State2, int> cache, int startTime, State2 testState)
        {
            if (testState.TimeRemaining <= 1)
            {
                if (testState.ActorId == 0)
                {
                    return 0;
                }

                State2 nextState = testState with
                {
                    ActorId = testState.ActorId - 1,
                    TimeRemaining = startTime,
                    CurrentNode = ValvesByName["AA"].Index,
                };

                return Stuff2(cache, startTime, nextState);
            }

            if (cache.TryGetValue(testState, out int knownValue))
            {
                CacheHits++;
                return knownValue;
            }

            CacheMisses++;

            int max = 0;
            Valve currentValve = Valves[testState.CurrentNode];
            long testBit = 1L << currentValve.Index;
            int trm1 = testState.TimeRemaining - 1;

            if ((testState.ClosedValves & testBit) == testBit)
            {
                Debug.Assert(currentValve.Rate > 0);

                int trm2 = trm1 - 1;
                int localScore = trm1 * currentValve.Rate;
                max = localScore;

                long newValveState = testState.ClosedValves & ~testBit;

                if (testState.ActorId == 0 && newValveState == 0)
                {
                    cache[testState] = localScore;
                    return localScore;
                }

                if (testState.TimeRemaining > 2)
                {
                    foreach (string conn in currentValve.Connections)
                    {
                        Valve nextValve = ValvesByName[conn];
                        State2 nextState = testState with
                        {
                            CurrentNode = nextValve.Index,
                            TimeRemaining = trm2,
                            ClosedValves = newValveState,
                        };

                        int dive = Stuff2(cache, startTime, nextState);

                        max = Math.Max(max, dive + localScore);
                    }
                }
                else
                {
                    State2 nextState = testState with { TimeRemaining = 0 };
                    return Stuff2(cache, startTime, nextState);
                }
            }

            if (testState.TimeRemaining > 2)
            {
                foreach (string conn in currentValve.Connections)
                {
                    Valve nextValve = ValvesByName[conn];
                    State2 nextState = testState with { CurrentNode = nextValve.Index, TimeRemaining = trm1 };
                    int dive = Stuff2(cache, startTime, nextState);

                    max = Math.Max(max, dive);
                }
            }
            else
            {
                State2 nextState = testState with { TimeRemaining = 0 };
                return Stuff2(cache, startTime, nextState);
            }

            cache[testState] = max;
            return max;
        }

        internal static void Problem2()
        {
            long available = 0;

            foreach (Valve v in Valves)
            {
                if (v.Rate > 0)
                {
                    available |= (1L << v.Index);
                }
            }

            State2 start = new State2
            {
                ActorId = 2,
                ClosedValves = available,
            };

            Dictionary<State2, int> cache = new Dictionary<State2, int>();
            int score = Stuff2(cache, 26, start);
            Console.WriteLine($"{cache.Count} known state(s).");
            Console.WriteLine($"Cache hits: {CacheHits}");
            Console.WriteLine($"Cache misses: {CacheMisses}");
            Console.WriteLine($"Cache had an entry but I did work anyways: {CacheIgnoreds}");
            Console.WriteLine();
            Console.WriteLine(score);
            Console.WriteLine();
        }
    }
}
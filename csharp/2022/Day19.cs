using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day19
    {
        [GeneratedRegex(
            @"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.")]
        private static partial Regex MatchRegex();

        private class Blueprint
        {
            internal int Id;
            internal int OreRobotOreCost;
            internal int ClayRobotOreCost;
            internal int ObsidianRobotOreCost;
            internal int ObsidianRobotClayCost;
            internal int GeodeRobotOreCost;
            internal int GeodeRobotObsidianCost;

            internal int MaxOreCost => 
                Math.Max(
                    Math.Max(OreRobotOreCost, ClayRobotOreCost),
                    Math.Max(ObsidianRobotOreCost, GeodeRobotOreCost));
        }

        private static List<Blueprint> Load()
        {
            List<Blueprint> list = new List<Blueprint>();
            Regex regex = MatchRegex();

            foreach (string s in Data.Enumerate())
            {
                Match m = regex.Match(s);

                if (!m.Success)
                {
                    throw new InvalidOperationException();
                }

                list.Add(new Blueprint
                {
                    Id = int.Parse(m.Groups[1].ValueSpan),
                    OreRobotOreCost = int.Parse(m.Groups[2].Value),
                    ClayRobotOreCost = int.Parse(m.Groups[3].Value),
                    ObsidianRobotOreCost = int.Parse(m.Groups[4].Value),
                    ObsidianRobotClayCost = int.Parse(m.Groups[5].Value),
                    GeodeRobotOreCost = int.Parse(m.Groups[6].Value),
                    GeodeRobotObsidianCost = int.Parse(m.Groups[7].Value),
                });
            }

            return list;
        }

        internal static void Problem1()
        {
            List<Blueprint> blueprints = Load();
            
            State start = new State
            {
                TimeRemaining = 24,
                OreRobots = 1,
            };

            int sum = 0;

            foreach (Blueprint blueprint in blueprints)
            {
                int localScore = Stuff(new(), start, blueprint);
                sum += localScore * blueprint.Id;
            }

            Console.WriteLine(sum);
        }

        internal static void Problem2()
        {
            List<Blueprint> blueprints = Load();

            State start = new State
            {
                TimeRemaining = 32,
                OreRobots = 1,
            };

            int product = 1;

            Console.WriteLine($"Starting part 2 at {DateTime.Now:HH:mm:ss.ssss}");

            foreach (Blueprint blueprint in blueprints.Take(3))
            {
                Console.WriteLine($"Starting blueprint {blueprint.Id}");
                int localScore = Stuff(new(), start, blueprint);

                Console.WriteLine($"Finished blueprint {blueprint.Id} at {DateTime.Now:HH:mm:ss.ssss} with score {localScore}");
                product *= localScore;
            }

            Console.WriteLine(product);
        }

        private record struct State(
            int TimeRemaining,
            int OreRobots,
            int ClayRobots,
            int ObsidianRobots,
            int GeodeRobots,
            int Ore,
            int Clay,
            int Obsidian,
            int Geodes)
        {
        }

        private static int Stuff(Dictionary<State, int> cache, State testState, Blueprint blueprint)
        {
            return Pathing.DepthFirstBest(
                testState,
                blueprint,
                Children,
                (state, _) => state.Geodes);

            static IEnumerable<State> Children(State testState, Blueprint blueprint)
            {
                if (testState.TimeRemaining <= 0)
                {
                    yield break;
                }

                State nextStateShared = testState with
                {
                    Ore = testState.Ore + testState.OreRobots,
                    Clay = testState.Clay + testState.ClayRobots,
                    Obsidian = testState.Obsidian + testState.ObsidianRobots,
                    Geodes = testState.Geodes + testState.GeodeRobots,
                    TimeRemaining = testState.TimeRemaining - 1,
                };

                if (testState.TimeRemaining > 1)
                {
                    if (testState.Ore >= blueprint.GeodeRobotOreCost &&
                        testState.Obsidian >= blueprint.GeodeRobotObsidianCost)
                    {
                        State geodeRobotState = nextStateShared with
                        {
                            GeodeRobots = nextStateShared.GeodeRobots + 1,
                            Ore = nextStateShared.Ore - blueprint.GeodeRobotOreCost,
                            Obsidian = nextStateShared.Obsidian - blueprint.GeodeRobotObsidianCost,
                        };

                        yield return geodeRobotState;
                        yield break;
                    }
                
                    if (testState.ObsidianRobots < blueprint.GeodeRobotObsidianCost &&
                        testState.Ore >= blueprint.ObsidianRobotOreCost &&
                        testState.Clay >= blueprint.ObsidianRobotClayCost)
                    {
                        State obsidianRobotState = nextStateShared with
                        {
                            ObsidianRobots = nextStateShared.ObsidianRobots + 1,
                            Ore = nextStateShared.Ore - blueprint.ObsidianRobotOreCost,
                            Clay = nextStateShared.Clay - blueprint.ObsidianRobotClayCost,
                        };

                        yield return obsidianRobotState;
                    }
                    else
                    {
                        if (testState.ClayRobots < blueprint.ObsidianRobotClayCost &&
                            testState.Ore >= blueprint.ClayRobotOreCost)
                        {
                            State clayRobotState = nextStateShared with
                            {
                                ClayRobots = nextStateShared.ClayRobots + 1,
                                Ore = nextStateShared.Ore - blueprint.ClayRobotOreCost,
                            };

                            yield return clayRobotState;
                        }

                        if (testState.OreRobots < blueprint.MaxOreCost &&
                            testState.Ore >= blueprint.OreRobotOreCost)
                        {
                            State oreRobotState = nextStateShared with
                            {
                                OreRobots = nextStateShared.OreRobots + 1,
                                Ore = nextStateShared.Ore - blueprint.OreRobotOreCost,
                            };

                            yield return oreRobotState;
                        }
                    }
                }

                yield return nextStateShared;
            }
        }

        private static int StuffOld(Dictionary<State, int> cache, State testState, Blueprint blueprint)
        {
            if (testState.TimeRemaining <= 0)
            {
                return testState.Geodes;
            }

            if (cache.TryGetValue(testState, out int knownValue))
            {
                return knownValue;
            }

            State nextStateShared = testState with
            {
                Ore = testState.Ore + testState.OreRobots,
                Clay = testState.Clay + testState.ClayRobots,
                Obsidian = testState.Obsidian + testState.ObsidianRobots,
                Geodes = testState.Geodes + testState.GeodeRobots,
                TimeRemaining = testState.TimeRemaining - 1,
            };

            int max;

            if (testState.Ore >= blueprint.GeodeRobotOreCost && testState.Obsidian >= blueprint.GeodeRobotObsidianCost)
            {
                State geodeRobotState = nextStateShared with
                {
                    GeodeRobots = nextStateShared.GeodeRobots + 1,
                    Ore = nextStateShared.Ore - blueprint.GeodeRobotOreCost,
                    Obsidian = nextStateShared.Obsidian - blueprint.GeodeRobotObsidianCost,
                };

                max = Stuff(cache, geodeRobotState, blueprint);
            }
            else
            {
                max = Stuff(cache, nextStateShared, blueprint);

                if (testState.ObsidianRobots < blueprint.GeodeRobotObsidianCost &&
                    testState.Ore >= blueprint.ObsidianRobotOreCost &&
                    testState.Clay >= blueprint.ObsidianRobotClayCost)
                {
                    State obsidianRobotState = nextStateShared with
                    {
                        ObsidianRobots = nextStateShared.ObsidianRobots + 1,
                        Ore = nextStateShared.Ore - blueprint.ObsidianRobotOreCost,
                        Clay = nextStateShared.Clay - blueprint.ObsidianRobotClayCost,
                    };

                    int localScore = Stuff(cache, obsidianRobotState, blueprint);
                    max = Math.Max(max, localScore);
                }
                else
                {
                    if (testState.OreRobots < blueprint.MaxOreCost && testState.Ore >= blueprint.OreRobotOreCost)
                    {
                        State oreRobotState = nextStateShared with
                        {
                            OreRobots = nextStateShared.OreRobots + 1,
                            Ore = nextStateShared.Ore - blueprint.OreRobotOreCost,
                        };

                        int localScore = Stuff(cache, oreRobotState, blueprint);
                        max = Math.Max(max, localScore);
                    }

                    if (testState.ClayRobots < blueprint.ObsidianRobotClayCost && testState.Ore >= blueprint.ClayRobotOreCost)
                    {
                        State clayRobotState = nextStateShared with
                        {
                            ClayRobots = nextStateShared.ClayRobots + 1,
                            Ore = nextStateShared.Ore - blueprint.ClayRobotOreCost,
                        };

                        int localScore = Stuff(cache, clayRobotState, blueprint);
                        max = Math.Max(max, localScore);
                    }
                }
            }

            cache[testState] = max;
            return max;
        }
    }
}
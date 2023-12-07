using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day05
    {
        private static List<long> initialSeeds;
        private static List<(long SrcStart, long DestStart, long Length)> seedToSoil = new();
        private static List<(long SrcStart, long DestStart, long Length)> soilToFert = new();
        private static List<(long SrcStart, long DestStart, long Length)> fertToWater = new();
        private static List<(long SrcStart, long DestStart, long Length)> waterToLight = new();
        private static List<(long SrcStart, long DestStart, long Length)> lightToTemp = new();
        private static List<(long SrcStart, long DestStart, long Length)> tempToHum = new();
        private static List<(long SrcStart, long DestStart, long Length)> humToLoc = new();

        internal static void Problem1()
        {
            Load(SrcSort);

            Dictionary<long, long> map = new();

            foreach (long seed in initialSeeds)
            {
                long soil = Map(seedToSoil, seed);
                long fert = Map(soilToFert, soil);
                long water = Map(fertToWater, fert);
                long light = Map(waterToLight, water);
                long temp = Map(lightToTemp, light);
                long hum = Map(tempToHum, temp);
                long loc = Map(humToLoc, hum);

                map[seed] = loc;
            }

            foreach (var kvp in map.OrderByDescending(kvp => kvp.Value))
            {
                Console.WriteLine($"{kvp.Key} => {kvp.Value}");
            }
        }

        private static long Map(List<(long SrcStart, long DestStart, long Length)> map, long target)
        {
            foreach (var entry in map)
            {
                long diff = target - entry.SrcStart;

                if (diff < 0)
                {
                    return target;
                }

                if (diff < entry.Length)
                {
                    return entry.DestStart + diff;
                }
            }

            return target;
        }

        private static void Load(Comparison<(long SrcStart, long DestStart, long Length)> sorter)
        {
            List<(long SrcStart, long DestStart, long Length)> target = null;
            foreach (string s in Data.Enumerate())
            {
                if (s.Length == 0 || string.IsNullOrWhiteSpace(s))
                {
                    target = null;
                    continue;
                }

                if (s.StartsWith("seeds: "))
                {
                    initialSeeds = s.Substring(7).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse)
                        .ToList();
                    continue;
                }

                if (s.StartsWith("seed-to-soil map:"))
                {
                    target = seedToSoil;
                    continue;
                }

                if (s.StartsWith("soil-to-fertilizer map:"))
                {
                    target = soilToFert;
                    continue;
                }

                if (s.StartsWith("fertilizer-to-water map:"))
                {
                    target = fertToWater;
                    continue;
                }

                if (s.StartsWith("water-to-light map:"))
                {
                    target = waterToLight;
                    continue;
                }

                if (s.StartsWith("light-to-temperature map:"))
                {
                    target = lightToTemp;
                    continue;
                }

                if (s.StartsWith("temperature-to-humidity map:"))
                {
                    target = tempToHum;
                    continue;
                }

                if (s.StartsWith("humidity-to-location map:"))
                {
                    target = humToLoc;
                    continue;
                }

                string[] parts = s.Split(' ');
                target.Add((long.Parse(parts[1]), long.Parse(parts[0]), long.Parse(parts[2])));
            }

            seedToSoil.Sort(sorter);
            soilToFert.Sort(sorter);
            fertToWater.Sort(sorter);
            waterToLight.Sort(sorter);
            lightToTemp.Sort(sorter);
            tempToHum.Sort(sorter);
            humToLoc.Sort(sorter);
        }

        private static int SrcSort((long SrcStart, long DestStart, long Length) x,
            (long SrcStart, long DestStart, long Length) y)
        {
            return long.Sign(x.SrcStart - y.SrcStart);
        }

        private static int DestSort((long SrcStart, long DestStart, long Length) x,
            (long SrcStart, long DestStart, long Length) y)
        {
            return long.Sign(x.DestStart - y.DestStart);
        }

        private static (long SrcStart, long DestStart, long Length) FindRange(
            List<(long SrcStart, long DestStart, long Length)> map, long target)
        {
            return FindRange(map, target, out _);
        }

        private static (long SrcStart, long DestStart, long Length) FindRange(List<(long SrcStart, long DestStart, long Length)> map, long target, out bool found)
        {
            (long SrcStart, long DestStart, long Length) lastEntry = (0, 0, 0);
            long start;

            found = false;

            foreach (var entry in map)
            {
                long diff = target - entry.DestStart;

                if (diff < 0)
                {
                    start = lastEntry.DestStart + lastEntry.Length;

                    return (start, start, entry.DestStart - start);
                }

                if (diff < entry.Length)
                {
                    found = true;
                    return entry;
                }

                lastEntry = entry;
            }

            start = lastEntry.DestStart + lastEntry.Length;
            long len = long.MaxValue - start;
            return (start, start, len);
        }

        private static (long SrcStart, long DestStart, long Length) FindSrcRange(
            List<(long SrcStart, long DestStart, long Length)> map, long target)
        {
            foreach (var entry in map)
            {
                long diff = target - entry.SrcStart;

                if (diff < 0)
                {
                    throw new Exception();
                }

                if (diff < entry.Length)
                {
                    return entry;
                }
            }

            throw new Exception();
        }

        internal static void Problem2()
        {
            Load(SrcSort);

            List<(long SrcStart, long DestStart, long Length)> seedRanges = new();
            List<(long SrcStart, long DestStart, long Length)> finalMap = new();

            for (int i = 0; i < initialSeeds.Count; i += 2)
            {
                seedRanges.Add((initialSeeds[i], initialSeeds[i], initialSeeds[i + 1]));
            }


            var allMaps = new[]
            {
                seedToSoil, soilToFert, fertToWater, waterToLight, lightToTemp, tempToHum, humToLoc,
            };

            foreach (var map in allMaps)
            {
                FillGaps(map);
            }

            finalMap = FoldMap(seedRanges, seedToSoil);
            finalMap = FoldMap(finalMap, soilToFert);
            finalMap = FoldMap(finalMap, fertToWater);
            finalMap = FoldMap(finalMap, waterToLight);
            finalMap = FoldMap(finalMap, lightToTemp);
            finalMap = FoldMap(finalMap, tempToHum);
            finalMap = FoldMap(finalMap, humToLoc);

            finalMap.Sort(DestSort);
            Console.WriteLine(finalMap[0]);
        }

        private static void FillGaps(List<(long SrcStart, long DestStart, long Length)> map)
        {
            long target = 0;

            for (int i = 0; i < map.Count; i++)
            {
                var entry = map[i];

                if (target < entry.SrcStart)
                {
                    map.Insert(i, (target, target, entry.SrcStart - target));
                    i++;
                }

                target = entry.SrcStart + entry.Length;
            }

            map.Add((target, target, long.MaxValue - target));
        }

        private static List<(long SrcStart, long DestStart, long Length)> FoldMap(
            List<(long SrcStart, long DestStart, long Length)> map1,
            List<(long SrcStart, long DestStart, long Length)> map2)
        {
            List<(long SrcStart, long DestStart, long Length)> ret = new();

            foreach (var range in map1)
            {
                long target = range.DestStart;
                long end = range.DestStart + range.Length;

                while (target < end)
                {
                    var next = FindSrcRange(map2, target);
                    long offset = target - next.SrcStart;
                    long destStart = offset + next.DestStart;
                    long len = long.Min(next.Length - offset, end - target);

                    ret.Add((target, destStart, len));
                    checked
                    {
                        target += len;
                    }
                }
            }

            return ret;
        }

        internal static void Problem2bw()
        {
            Load(DestSort);

            List<(long SrcStart, long DestStart, long Length)> seedRanges = new();
            seedRanges.Sort(DestSort);

            for (int i = 0; i < initialSeeds.Count; i += 2)
            {
                seedRanges.Add((initialSeeds[i], initialSeeds[i], initialSeeds[i + 1]));
            }

            {
                var startRange = FindRange(humToLoc, 0, out bool found);

                if (!found)
                {
                    humToLoc.Insert(0, startRange);
                }

                humToLoc.Sort(SrcSort);
                var lastRange = humToLoc.Last();
                long lastRangeEnd = lastRange.SrcStart + lastRange.Length;
                lastRangeEnd++;
                humToLoc.Add((lastRangeEnd, lastRangeEnd, long.MaxValue - lastRangeEnd));
                humToLoc.Sort(DestSort);
            }

            foreach (var locEntry in humToLoc)
            {
                long locTarget = locEntry.SrcStart;
                long locEnd = locEntry.SrcStart + locEntry.Length;

                //while (locTarget < locEnd)
                {
                    var tempRange = FindRange(tempToHum, locTarget);
                    long tempTarget = tempRange.SrcStart + locTarget - tempRange.DestStart;
                    long tempEnd = tempRange.SrcStart + tempRange.Length;

                    //while (tempTarget < tempEnd)
                    {
                        var lightRange = FindRange(lightToTemp, tempTarget);
                        long lightTarget = lightRange.SrcStart + tempTarget - lightRange.DestStart;
                        long lightEnd = lightRange.SrcStart + lightRange.Length;

                        //while (lightTarget < lightEnd)
                        {
                            var waterRange = FindRange(waterToLight, lightTarget);
                            long waterTarget = waterRange.SrcStart + lightTarget - waterRange.DestStart;
                            long waterEnd = waterRange.SrcStart + waterRange.Length;

                            //while (waterTarget < waterEnd)
                            {
                                var fertRange = FindRange(fertToWater, waterTarget);
                                long fertTarget = fertRange.SrcStart + waterTarget - fertRange.DestStart;
                                long fertEnd = fertRange.DestStart + fertRange.Length;

                               // while (fertTarget < fertEnd)
                                {
                                    var soilRange = FindRange(soilToFert, fertTarget);
                                    long soilTarget = soilRange.SrcStart + fertTarget - soilRange.DestStart;
                                    long soilEnd = soilRange.SrcStart + soilRange.Length;

                                    //while (soilTarget < soilEnd)
                                    {
                                        var seedRange = FindRange(seedToSoil, soilTarget);
                                        long seedTarget = seedRange.SrcStart + soilTarget - seedRange.SrcStart;
                                        long seedEnd = seedRange.SrcStart + seedRange.Length;

                                        while (seedTarget < seedEnd)
                                        {
                                            var lastRange = FindRange(seedRanges, seedTarget, out bool found);

                                            if (found)
                                            {
                                                Console.WriteLine(seedTarget);
                                                return;
                                            }

                                            seedTarget = lastRange.SrcStart + lastRange.Length;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
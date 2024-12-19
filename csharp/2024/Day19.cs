using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day19
    {
        private static (List<string> Parts, List<string> Patterns) Load()
        {
            string parts = null;
            List<string> patterns = null;

            foreach (string s in Data.Enumerate())
            {
                if (parts is null)
                {
                    parts = s;
                }
                else if (string.IsNullOrEmpty(s))
                {
                    patterns = new List<string>();
                }
                else
                {
                    patterns.Add(s);
                }
            }

            return (parts.Split(", ").ToList(), patterns);
        }

        internal static void Problem1()
        {
            (List<string> parts, List<string> patterns) = Load();

            long ret = 0;

            foreach (string pattern in patterns)
            {
                if (CanMake(pattern, parts))
                {
                    ret++;
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            (List<string> parts, List<string> patterns) = Load();
            Dictionary<string, long> counts = new();
            long ret = 0;

            foreach (string pattern in patterns)
            {
                ret += Count(pattern, parts, counts);
            }

            Console.WriteLine(ret);
        }

        private static bool CanMake(ReadOnlySpan<char> pattern, List<string> parts)
        {
            if (pattern.IsEmpty)
            {
                return true;
            }

            foreach (string part in parts)
            {
                if (pattern.StartsWith(part))
                {
                    if (CanMake(pattern.Slice(part.Length), parts))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static long Count(ReadOnlySpan<char> pattern, List<string> parts, Dictionary<string, long> counts)
        {
            if (pattern.IsEmpty)
            {
                return 1;
            }

            if (counts.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(pattern, out long existing))
            {
                return existing;
            }

            long count = 0;

            foreach (string part in parts)
            {
                if (pattern.StartsWith(part))
                {
                    ReadOnlySpan<char> rest = pattern.Slice(part.Length);
                    count += Count(rest, parts, counts);
                }
            }

            counts.GetAlternateLookup<ReadOnlySpan<char>>().TryAdd(pattern, count);
            return count;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    public class Day01
    {
        internal static void Problem1()
        {
            long ret = 0;
            List<long> left = new();
            List<long> right = new();

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                left.Add(long.Parse(parts[0]));
                right.Add(long.Parse(parts[1]));
            }

            left.Sort();
            right.Sort();

            for (int i = 0; i < left.Count; i++)
            {
                ret += long.Abs(left[i] - right[i]);
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            List<long> left = new();
            Dictionary<long, long> right = new();

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                left.Add(long.Parse(parts[0]));

                ref long rightSlot = ref CollectionsMarshal.GetValueRefOrAddDefault(right, long.Parse(parts[1]), out _);
                rightSlot++;
            }

            left.Sort();

            for (int i = 0; i < left.Count; i++)
            {
                ret += left[i] * right.GetValueOrDefault(left[i]);
            }

            Console.WriteLine(ret);
        }
    }
}
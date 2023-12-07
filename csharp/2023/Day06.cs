using System;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day06
    {
        internal static void Problem1()
        {
            int sum = 1;
            int[] times = null;
            int[] distances = null;

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                int[] data = parts.Skip(1).Select(int.Parse).ToArray();

                if (times is null)
                {
                    times = data;
                }
                else
                {
                    distances = data;
                }
            }

            for (int i = 0; i < times.Length; i++)
            {
                int local = 0;
                for (int j = 1; j < times[i]; j++)
                {
                    int buttonTime = j;
                    int travelTime = times[i] - buttonTime;
                    int val = buttonTime * travelTime;

                    if (val > distances[i])
                    {
                        local++;
                    }
                }

                sum *= local;
            }

            Console.WriteLine(sum);
        }

        internal static void Problem2()
        {
            long sum = 1;
            long[] times = null;
            long[] distances = null;

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Replace(" ", "").Split(':', StringSplitOptions.RemoveEmptyEntries);

                long[] data = parts.Skip(1).Select(long.Parse).ToArray();

                if (times is null)
                {
                    times = data;
                }
                else
                {
                    distances = data;
                }
            }

            for (int i = 0; i < times.Length; i++)
            {
                long local = 0;
                for (int j = 1; j < times[i]; j++)
                {
                    long buttonTime = j;
                    long travelTime = times[i] - buttonTime;
                    long val = buttonTime * travelTime;

                    if (val > distances[i])
                    {
                        local++;
                    }
                }

                sum *= local;
            }

            Console.WriteLine(sum);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day6
    {
        internal static void Problem1()
        {
            List<int> lanternfish = new List<int>();

            foreach (string line in Data.Enumerate())
            {
                ReadOnlySpan<char> lineSpan = line;
                int commaIdx;

                while ((commaIdx = lineSpan.IndexOf(',')) >= 0)
                {
                    lanternfish.Add(int.Parse(lineSpan.Slice(0, commaIdx)));
                    lineSpan = lineSpan.Slice(commaIdx + 1);
                }

                lanternfish.Add(int.Parse(lineSpan));
            }

            for (int day = 0; day < 80; day++)
            {
                for (int i = lanternfish.Count - 1; i >= 0; i--)
                {
                    int newCnt = lanternfish[i] - 1;

                    if (newCnt < 0)
                    {
                        lanternfish.Add(8);
                        lanternfish[i] = 6;
                    }
                    else
                    {
                        lanternfish[i] = newCnt;
                    }
                }
            }

            Console.WriteLine(lanternfish.Count);
        }

        internal static void Problem2()
        {
            long[] lanternfish = new long[9];

            foreach (string line in Data.Enumerate())
            {
                ReadOnlySpan<char> lineSpan = line;
                int commaIdx;

                while ((commaIdx = lineSpan.IndexOf(',')) >= 0)
                {
                    lanternfish[int.Parse(lineSpan.Slice(0, commaIdx))]++;
                    lineSpan = lineSpan.Slice(commaIdx + 1);
                }

                lanternfish[int.Parse(lineSpan)]++;
            }

            for (int i = 0; i < 256; i++)
            {
                long spawned = lanternfish[0];
                lanternfish[0] = lanternfish[1];
                lanternfish[1] = lanternfish[2];
                lanternfish[2] = lanternfish[3];
                lanternfish[3] = lanternfish[4];
                lanternfish[4] = lanternfish[5];
                lanternfish[5] = lanternfish[6];
                lanternfish[6] = lanternfish[7] + spawned;
                lanternfish[7] = lanternfish[8];
                lanternfish[8] = spawned;
            }

            Console.WriteLine(lanternfish.Sum());
        }
    }
}
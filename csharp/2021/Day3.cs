using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day3
    {
        internal static void Problem1()
        {
            int[] ctrs = null;
            int entries = 0;

            foreach (string line in Data.Enumerate())
            {
                entries++;
                ctrs ??= new int[line.Length];

                int pos = 0;

                foreach (char c in line)
                {
                    if (c == '1')
                    {
                        ctrs[pos]++;
                    }

                    pos++;
                }
            }

            long epsilon = 0;
            long gamma = 0;
            long half = entries / 2;
            int exponent = 0;

            for (int i = ctrs.Length - 1; i >= 0; i--)
            {
                long val = (long)Math.Pow(2, exponent);
                exponent++;

                if (ctrs[i] > half)
                {
                    gamma += val;
                }
                else
                {
                    epsilon += val;
                }
            }

            Console.WriteLine($"gamma={gamma},epsilon={epsilon},pwr={gamma*epsilon}");
        }

        internal static void Problem2()
        {
            List<string> oxy = Data.Enumerate().ToList();
            List<string> co2 = new List<string>(oxy);

            int len = oxy[0].Length;

            for (int i = 0; i < len && oxy.Count > 1; i++)
            {
                int set = oxy.Count(v => v[i] == '1');
                int rem = oxy.Count - set;

                Utils.TraceForSample($"{set}/{oxy.Count} bits set.");

                if (set < rem)
                {
                    Utils.TraceForSample("Removing the 1s.");
                    RemoveIf(oxy, i, '1');
                }
                else
                {
                    Utils.TraceForSample("Removing the 0s.");
                    RemoveIf(oxy, i, '0');
                }
            }

            for (int i = 0; i < len && co2.Count > 1; i++)
            {
                int set = co2.Count(v => v[i] == '1');
                int rem = co2.Count - set;

                Utils.TraceForSample($"{set}/{co2.Count} bits set.");

                if (set >= rem)
                {
                    Utils.TraceForSample("Removing the 1s.");
                    RemoveIf(co2, i, '1');
                }
                else
                {
                    Utils.TraceForSample("Removing the 0s.");
                    RemoveIf(co2, i, '0');
                }
            }

            long oxyVal = Utils.ParseBinary(oxy.Single());
            long co2Val = Utils.ParseBinary(co2.Single());

            Console.WriteLine($"oxy={oxy.Single()}=>{oxyVal},co2={co2.Single()}=>{co2Val},rating={oxyVal*co2Val}");
        }

        private static void RemoveIf(List<string> list, int bit, char match)
        {
            for (int j = list.Count - 1; j >= 0; j--)
            {
                if (list[j][bit] == match)
                {
                    list.RemoveAt(j);
                }
            }
        }
    }
}

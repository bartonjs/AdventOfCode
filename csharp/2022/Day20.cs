using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day20
    {
        private static List<WrappedInt> Load(long scale = 1)
        {
            List<WrappedInt> data = new List<WrappedInt>();

            foreach (string s in Data.Enumerate())
            {
                data.Add(new WrappedInt { Value = long.Parse(s) * scale });
            }

            return data;
        }

        private class WrappedInt
        {
            internal long Value;
        }

        internal static void Problem1()
        {
            List<WrappedInt> orig = Load();
            List<WrappedInt> manip = new List<WrappedInt>(orig);
            WrappedInt zero = null;

            foreach (WrappedInt value in orig)
            {
                long intVal = value.Value;
                int idx = manip.IndexOf(value);

                if (value.Value == 0)
                {
                    zero = value;
                    continue;
                }

                manip.RemoveAt(idx);
                long newIdx = idx + intVal;

                while (newIdx < 0)
                {
                    newIdx += manip.Count;
                }

                newIdx %= manip.Count;
                manip.Insert((int)newIdx, value);
#if SAMPLE
                Console.WriteLine(string.Join(", ", manip.Select(x => x.Value)));
#endif
            }

            long sum = 0;
            int zeroIdx = manip.IndexOf(zero);

            for (int i = 1000; i <= 3000; i += 1000)
            {
                int pos = (i + zeroIdx) % orig.Count;
                sum += manip[pos].Value;
            }

            Console.WriteLine(sum);
        }

        internal static void Problem2()
        {
            List<WrappedInt> orig = Load(811589153);
            List<WrappedInt> manip = new List<WrappedInt>(orig);
            WrappedInt zero = null;

            for (int i = 0; i < 10; i++)
            {
                foreach (WrappedInt value in orig)
                {
                    long intVal = value.Value;
                    int idx = manip.IndexOf(value);

                    if (value.Value == 0)
                    {
                        zero = value;
                        continue;
                    }

                    manip.RemoveAt(idx);
                    long newIdx = idx + intVal;

                    newIdx %= manip.Count;

                    while (newIdx < 0)
                    {
                        newIdx += manip.Count;
                    }

                    manip.Insert((int)newIdx, value);
                }
            }

            long sum = 0;
            int zeroIdx = manip.IndexOf(zero);

            for (int i = 1000; i <= 3000; i += 1000)
            {
                int pos = (i + zeroIdx) % orig.Count;
                sum += manip[pos].Value;
            }

            Console.WriteLine(sum);
        }
    }
}
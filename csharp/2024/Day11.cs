using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day11
    {
        internal static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                List<long> data = s.ToLongList(' ');

                for (int round = 0; round < 25; round++)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i] == 0)
                        {
                            data[i] = 1;
                        }
                        else if ((int)double.Log10(data[i]) % 2 == 1)
                        {
                            string val = data[i].ToString();
                            long topHalf = long.Parse(val.AsSpan(0, val.Length / 2));
                            long bottomHalf = long.Parse(val.AsSpan(val.Length / 2));

                            data[i] = topHalf;
                            data.Insert(i + 1, bottomHalf);
                            i++;
                        }
                        else
                        {
                            data[i] *= 2024;
                        }
                    }
                }

                ret += data.Count;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            Dictionary<(long, int), long> cache = new();

            foreach (string s in Data.Enumerate())
            {
                List<long> allData = s.ToLongList(' ');

                foreach (long datum in allData)
                {
                    ret += Count(datum, 75, cache);
                }
            }

            Console.WriteLine(ret);

            static long Count(long datum, int count, Dictionary<(long, int), long> cache)
            {
                if (count <= 0)
                {
                    return 1;
                }

                (long datum, int count) key = (datum, count);

                if (cache.TryGetValue(key, out long cached))
                {
                    return cached;
                }

                int countM1 = count - 1;

                if (datum == 0)
                {
                    return cache[key] = Count(1, countM1, cache);
                }

                if ((int)double.Log10(datum) % 2 == 1)
                {
                    string val = datum.ToString();
                    long topHalf = long.Parse(val.AsSpan(0, val.Length / 2));
                    long bottomHalf = long.Parse(val.AsSpan(val.Length / 2));

                    return cache[key] = (Count(topHalf, countM1, cache) + Count(bottomHalf, countM1, cache));
                }

                return cache[key] = Count(datum * 2024, countM1, cache);
            }
        }
    }
}
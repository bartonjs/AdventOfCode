using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day12
    {
        internal static void Problem1()
        {
            long ret = 0;
            int[] shapes = new int[7];
            List<Row> rows = null;
            int idx = -1;

            foreach (string s in Data.Enumerate())
            {
                if (rows is not null)
                {
                    rows.Add(Row.Parse(s));
                    continue;
                }

                if (idx < 0)
                {
                    int colon = s.IndexOf(':');
                    int x = s.AsSpan(0, colon).IndexOf('x');

                    if (x > 0)
                    {
                        rows = new List<Row>();
                        rows.Add(Row.Parse(s));
                        continue;
                    }

                    idx = int.Parse(s.AsSpan(0, colon));
                    continue;
                }

                if (string.IsNullOrEmpty(s))
                {
                    idx = -1;
                    continue;
                }

                shapes[idx] += s.Count(c => c == '#');
            }

            foreach (Row r in rows)
            {
                int totalArea = r.Area;

                for (int i = 0; i < r.Counts.Length; i++)
                {
                    totalArea -= r.Counts[i] * shapes[i];
                }

                if (totalArea >= 0)
                {
                    ret++;
                }
            }

            Console.WriteLine(ret);
        }

        internal record class Row(int W, int H, int[] Counts)
        {
            internal int Area => W * H;

            public static Row Parse(string s)
            {
                int x = s.IndexOf('x');
                int colon = s.IndexOf(':', x + 1);

                int w = int.Parse(s.AsSpan(0, x));
                int h = int.Parse(s.AsSpan(x + 1, colon - x - 1));
                int[] counts = s.Substring(colon + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                return new Row(w, h, counts);
            }
        }
    }
}

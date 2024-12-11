﻿using System;
using System.Collections.Generic;

namespace AdventOfCode.Util
{
    public static class Data
    {
#pragma warning disable 649
        private static string[] s_lines;
#pragma warning restore 649

        public static IEnumerable<string> Enumerate()
        {
            foreach (string line in s_lines)
            {
                yield return line;
            }
        }

        public static List<long> ToLongList(this string commaSeparated, char separator = ',')
        {
            ReadOnlySpan<char> span = commaSeparated;
            int comma = commaSeparated.IndexOf(separator);
            List<long> list = new List<long>();

            while (comma >= 0)
            {
                list.Add(long.Parse(span.Slice(0, comma)));
                span = span.Slice(comma + 1);
                comma = span.IndexOf(separator);
            }

            list.Add(long.Parse(span));
            return list;
        }
    }
}

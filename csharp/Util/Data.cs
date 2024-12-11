using System;
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

        public static List<long> ToLongList(this string separated, char separator = ',')
        {
            return new List<long>(AsLongs(separated, separator));
        }

        public static IEnumerable<long> AsLongs(this string separated, char separator = ',')
        {
            int comma = separated.IndexOf(separator);
            int lastComma = 0;

            while (comma >= 0)
            {
                yield return long.Parse(separated.AsSpan(lastComma, comma - lastComma));
                lastComma = comma + 1;
                comma = separated.IndexOf(separator, lastComma);
            }

            yield return long.Parse(separated.AsSpan(lastComma));
        }
    }
}

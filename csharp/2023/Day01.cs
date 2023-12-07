using System;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    public class Day01
    {
        private static readonly char[] s_digits = "0123456789".ToCharArray();

        internal static void Problem1()
        {
            int sum = 0;

            foreach (string s in Data.Enumerate())
            {
                int first = s.IndexOfAny(s_digits);
                int last = s.LastIndexOfAny(s_digits);

                int fv = s[first] - '0';
                int lv = s[last] - '0';

                sum += fv * 10;
                sum += lv;
            }

            Console.WriteLine(sum);
        }

        private static readonly string[] s_digitWords =
            { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

        internal static void Problem2()
        {
            int sum = 0;

            foreach (string s in Data.Enumerate())
            {
                int first = int.MaxValue;
                int last = -1;

                int fv = -1;
                int lv = -1;

                ReadOnlySpan<char> line = s;

                for (int wordIdx = 0; wordIdx < s_digitWords.Length; wordIdx++)
                {
                    string word = s_digitWords[wordIdx];
                    int idx = line.IndexOf(word);
                    int idx2 = line.LastIndexOf(word);

                    if (idx >= 0 && idx < first)
                    {
                        fv = wordIdx;
                        first = idx;
                    }

                    if (idx2 > last)
                    {
                        lv = wordIdx;
                        last = idx2;
                    }
                }

                int first2 = s.IndexOfAny(s_digits);
                int last2 = s.LastIndexOfAny(s_digits);

                if (first2 < first)
                {
                    fv = s[first2] - '0';
                }

                if (last2 > last)
                {
                    lv = s[last2] - '0';
                }

                int local = fv * 10 + lv;
                Console.WriteLine($"{s}: {fv} - {lv} -- {local}");

                sum += local;
            }

            Console.WriteLine(sum);
        }
    }
}
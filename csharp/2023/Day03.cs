using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day03
    {
        private static char[] s_digits = "0123456789".ToCharArray();
        private static char[] s_nonsymbol = "0123456789. \u00a1".ToCharArray();

        internal static void Problem1()
        {
            int sum = 0;

            List<char[]> data = Data.Enumerate().Select(s => s.ToCharArray()).ToList();

            for (int i = 0; i < data.Count; i++)
            {
                Span<char> line = data[i];
                int idx = line.IndexOfAnyExcept(s_nonsymbol);

                while (idx >= 0)
                {
                    line[idx] = ' ';

                    foreach (var tuple in FindNumbers(i, idx, data))
                    {
                        Span<char> slice = data[tuple.Item1].AsSpan(tuple.Item2, tuple.Item3);
                        int numval = int.Parse(slice);
                        sum += numval;
                        slice.Fill(' ');
                    }

                    idx = line.IndexOfAnyExcept(s_nonsymbol);
                }
            }

            Console.WriteLine(sum);
        }

        private static IEnumerable<(int, int, int)> FindNumbers(int row, int col, List<char[]> data)
        {
            var val = FindNumberAt(row - 1, col - 1, data);
            int terminate = int.MinValue;

            if (val.HasValue)
            {
                terminate = val.Value.Item2 + val.Value.Item3;
                yield return val.Value;
            }

            val = null;
            
            if (terminate < col)
            {
                val = FindNumberAt(row - 1, col, data);
            }

            if (val.HasValue)
            {
                terminate = val.Value.Item2 + val.Value.Item3;
                yield return val.Value;
            }

            val = null;
            
            if (terminate < col + 1)
            {
                val = FindNumberAt(row - 1, col + 1, data);
            }

            if (val.HasValue)
            {
                yield return val.Value;
            }

            val = FindNumberAt(row, col - 1, data);

            if (val.HasValue)
            {
                yield return val.Value;
            }

            val = FindNumberAt(row, col + 1, data);

            if (val.HasValue)
            {
                yield return val.Value;
            }

            terminate = int.MinValue;
            val = FindNumberAt(row + 1, col - 1, data);

            if (val.HasValue)
            {
                terminate = val.Value.Item2 + val.Value.Item3;
                yield return val.Value;
            }

            val = null;

            if (terminate < col)
            {
                val = FindNumberAt(row + 1, col, data);
            }

            if (val.HasValue)
            {
                terminate = val.Value.Item2 + val.Value.Item3;
                yield return val.Value;
            }

            val = null;

            if (terminate < col + 1)
            {
                val = FindNumberAt(row + 1, col + 1, data);
            }

            if (val.HasValue)
            {
                yield return val.Value;
            }
        }

        private static (int, int, int)? FindNumberAt(int row, int col, List<char[]> data)
        {
            if (row >= 0 && row < data.Count)
            {
                ReadOnlySpan<char> line = data[row];

                if (col >= 0 && col < line.Length)
                {
                    if (char.IsAsciiDigit(line[col]))
                    {
                        ReadOnlySpan<char> before = line.Slice(0, col);
                        int left = before.LastIndexOfAnyExcept(s_digits);
                        left++;

                        ReadOnlySpan<char> atAfter = line.Slice(col);
                        int right = atAfter.IndexOfAnyExcept(s_digits);

                        if (right < 0)
                        {
                            right = atAfter.Length;
                        }

                        return (row, left, right + before.Length - left);
                    }
                }
            }

            return null;
        }

        internal static void Problem2()
        {
            long sum = 0;

            List<char[]> data = Data.Enumerate().Select(s => s.ToCharArray()).ToList();

            for (int i = 0; i < data.Count; i++)
            {
                Span<char> line = data[i];
                int idx = line.IndexOfAnyExcept(s_nonsymbol);

                while (idx >= 0)
                {
                    line[idx] = ' ';

                    List<(int, int, int)> numbers = FindNumbers(i, idx, data).ToList();

                    if (numbers.Count == 2)
                    {
                        long gear = 1;

                        foreach (var tuple in numbers)
                        {
                            Span<char> slice = data[tuple.Item1].AsSpan(tuple.Item2, tuple.Item3);
                            int numval = int.Parse(slice);
                            Console.WriteLine(numval);
                            gear *= numval;
                        }

                        sum += gear;
                        Console.WriteLine($"Found gear at ({i}, {idx}) ({gear})");
                    }

                    idx = line.IndexOfAnyExcept(s_nonsymbol);
                }
            }

            Console.WriteLine(sum);
        }
    }
}
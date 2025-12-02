using System;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day02
    {
        private static readonly long[] s_pow10 = [1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 10000000000];

        private static int CountDigits(long value)
        {
            int i = 2;

            while (true)
            {
                if (value < s_pow10[i])
                {
                    return i;
                }

                i++;
            }
        }

        public static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                int comma = -1;

                while (comma < s.Length)
                {
                    int start = comma + 1;
                    int hyphen = s.IndexOf('-', start);

                    if (hyphen < 0)
                    {
                        break;
                    }

                    int nextComma = s.IndexOf(',', hyphen);

                    if (nextComma < 0)
                    {
                        nextComma = s.Length;
                    }

                    long first = long.Parse(s.AsSpan(start, hyphen - start));
                    long second = long.Parse(s.AsSpan(hyphen + 1, nextComma - (hyphen + 1)));
                    comma = nextComma;

                    first = long.Max(11, first);

                    for (long i = first; i <= second; i++)
                    {
                        int digits = CountDigits(i);

                        if (digits % 2 != 0)
                        {
                            i = s_pow10[digits] - 1;
                            continue;
                        }

                        long halfPow = s_pow10[digits / 2];
                        (long quot, long rem) = Math.DivRem(i, halfPow);

                        if (quot == rem)
                        {
                            Utils.TraceForSample(i.ToString());
                            ret += i;
                        }
                    }
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                int comma = -1;

                while (comma < s.Length)
                {
                    int start = comma + 1;
                    int hyphen = s.IndexOf('-', start);

                    if (hyphen < 0)
                    {
                        break;
                    }

                    int nextComma = s.IndexOf(',', hyphen);

                    if (nextComma < 0)
                    {
                        nextComma = s.Length;
                    }

                    long first = long.Parse(s.AsSpan(start, hyphen - start));
                    long second = long.Parse(s.AsSpan(hyphen + 1, nextComma - (hyphen + 1)));
                    comma = nextComma;

                    first = long.Max(11, first);

                    for (long i = first; i <= second; i++)
                    {
                        int digits = CountDigits(i);

                        for (int probe = digits / 2; probe > 0; probe--)
                        {
                            if (digits % probe != 0)
                            {
                                continue;
                            }

                            if (IsInvalid(i, probe))
                            {
                                Utils.TraceForSample(i.ToString());
                                ret += i;
                                break;
                            }
                        }
                    }
                }
            }

            Console.WriteLine(ret);

            static bool IsInvalid(long value, int digits)
            {
                long mod = s_pow10[digits];
                long candidate = value % mod;

                if (candidate == 0)
                {
                    return false;
                }

                long probe = candidate * mod + candidate;

                while (probe < value)
                {
                    checked
                    {
                        probe *= mod;
                        probe += candidate;
                    }
                }

                return probe == value;
            }
        }
    }
}

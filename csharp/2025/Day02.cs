using System;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day02
    {
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
                        double log10 = Math.Log10(i);
                        int digits = (int)Math.Ceiling(log10);

                        if (double.IsInteger(log10))
                        {
                            digits++;
                        }

                        if (digits % 2 != 0)
                        {
                            i = (long)Math.Pow(10, digits) - 1;
                            continue;
                        }

                        long halfPow = (long)Math.Pow(10, digits / 2);
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
                        double log10 = Math.Log10(i);
                        int digits = (int)Math.Ceiling(log10);

                        if (double.IsInteger(log10))
                        {
                            digits++;
                        }

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
                long mod = (long)Math.Pow(10, digits);
                long candidate = value % mod;

                if (candidate == 0)
                {
                    return false;
                }

                (long div, long rem) = Math.DivRem(value, candidate);

                if (rem != 0)
                {
                    return false;
                }

                while (div > 0)
                {
                    (div, rem) = Math.DivRem(div, mod);

                    if (rem != 1)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}

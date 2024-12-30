using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day25
    {
        private static (List<short[]> Locks, List<short[]> Keys) Load()
        {
            List<string> list = new List<string>();
            List<short[]> locks = new();
            List<short[]> keys = new();

            foreach (string s in Data.Enumerate().Append(""))
            {
                if (string.IsNullOrEmpty(s))
                {
                    short[] lockOrKey = new short[Vector<short>.Count];

                    if (list[0].IndexOf('.') == -1)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            for (int y = 1; y < list.Count; y++)
                            {
                                if (list[y][x] == '#')
                                {
                                    lockOrKey[x]++;
                                }
                            }
                        }

                        locks.Add(lockOrKey);
                    }
                    else
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            for (int y = list.Count - 2; y >= 0; y--)
                            {
                                if (list[y][x] == '#')
                                {
                                    lockOrKey[x]++;
                                }
                            }
                        }

                        keys.Add(lockOrKey);
                    }

                    list.Clear();
                }
                else
                {
                    list.Add(s);
                }
            }

            return (locks, keys);
        }

        internal static void Problem1()
        {
            (List<short[]> locks, List<short[]> keys) = Load();
            long ret = 0;

            foreach (short[] lck in locks)
            {
                foreach (short[] key in keys)
                {
                    bool add = true;

                    for (int i = 0; i < lck.Length; i++)
                    {
                        if (lck[i] + key[i] >= 6)
                        {
                            add = false;
                            break;
                        }
                    }

                    if (add)
                    {
                        ret++;
                    }
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem3()
        {
            (List<short[]> locks, List<short[]> keys) = Load();
            long ret = 0;
            Vector128<short> safeLimit = Vector128.Create<short>(5);

            foreach (short[] lck in locks)
            {
                foreach (short[] key in keys)
                {
                    Vector128<short> sum = Vector128.Create<short>(lck) + Vector128.Create<short>(key);

                    if (Vector128.LessThanOrEqualAll(sum, safeLimit))
                    {
                        ret++;
                    }
                }
            }

            Console.WriteLine(ret);
        }
    }
}
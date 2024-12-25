using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day25
    {
        private static (List<int[]> Locks, List<int[]> Keys) Load()
        {
            List<string> list = new List<string>();
            List<int[]> locks = new();
            List<int[]> keys = new();

            foreach (string s in Data.Enumerate().Append(""))
            {
                if (string.IsNullOrEmpty(s))
                {
                    int[] lockOrKey = new int[5];

                    if (list[0].IndexOf('.') == -1)
                    {
                        for (int x = 0; x < lockOrKey.Length; x++)
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
                        for (int x = 0; x < lockOrKey.Length; x++)
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
            (List<int[]> locks, List<int[]> keys) = Load();
            long ret = 0;

            foreach (int[] lck in locks)
            {
                foreach (int[] key in keys)
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

        internal static void Problem2x()
        {
        }
    }
}
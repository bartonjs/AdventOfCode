using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day11
    {
        private class Monkey
        {
            public Queue<long> Items;
            public Func<long, long> Func;
            public int Test;
            public int True;
            public int False;
            public int Count;
        }

        private static List<Monkey> Load()
        {
            return new()
            {
#if SAMPLE
                new Monkey()
                {
                    Items = new Queue<long>(new long[] {79, 98}),
                    Func = x => x * 19,
                    Test = 23,
                    True = 2,
                    False = 3,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] {54, 65, 75, 74}),
                    Func = x => x + 6,
                    Test = 19,
                    True = 2,
                    False = 0,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] {79, 60, 97 }),
                    Func = x => x * x,
                    Test = 13,
                    True = 1,
                    False = 3,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 74 }),
                    Func = x => x + 3,
                    Test = 17,
                    True = 0,
                    False = 1,
                }
#else
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 56, 56, 92, 65, 71, 61, 79 }),
                    Func = x => x * 7,
                    Test = 3,
                    True = 3,
                    False = 7,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 61, 85 }),
                    Func = x => x + 5,
                    Test = 11,
                    True = 6,
                    False = 4,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 54, 96, 82, 78, 69 }),
                    Func = x => x * x,
                    Test = 7,
                    True = 0,
                    False = 7,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 57, 59, 65, 95 }),
                    Func = x => x + 4,
                    Test = 2,
                    True = 5,
                    False = 1,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 62, 67, 80 }),
                    Func = x => x * 17,
                    Test = 19,
                    True = 2,
                    False = 6,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 91 }),
                    Func = x => x + 7,
                    Test = 5,
                    True = 1,
                    False = 4,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 79, 83, 64, 52, 77, 56, 63, 92 }),
                    Func = x => x + 6,
                    Test = 17,
                    True = 2,
                    False = 0,
                },
                new Monkey()
                {
                    Items = new Queue<long>(new long[] { 50, 97, 76, 96, 80, 56 }),
                    Func = x => x + 3,
                    Test = 13,
                    True = 3,
                    False = 5,
                },
#endif
            };
        }

        internal static void Problem1()
        {
            List<Monkey> monkeys = Load();

            for (int round = 0; round < 20; round++)
            {
                for (int m = 0; m < monkeys.Count; m++)
                {
                    Monkey monkey = monkeys[m];

                    while (monkey.Items.TryDequeue(out long cur))
                    {
                        cur = monkey.Func(cur) / 3;
                        monkey.Count++;

                        if ((cur % monkey.Test) == 0)
                        {
                            monkeys[monkey.True].Items.Enqueue(cur);
                        }
                        else
                        {
                            monkeys[monkey.False].Items.Enqueue(cur);
                        }
                    }
                }
            }

            int score = 1;

            foreach (Monkey m in monkeys.OrderByDescending(mon => mon.Count).Take(2))
            {
                score *= m.Count;
            }

            Console.WriteLine(score);
        }

        internal static void Problem2()
        {
            List<Monkey> monkeys = Load();

            BigInteger bigProd = BigInteger.One;
            BigInteger gcd = BigInteger.One;

            foreach (Monkey m in monkeys)
            {
                bigProd *= m.Test;
                gcd = BigInteger.GreatestCommonDivisor(gcd, m.Test);
            }

            BigInteger bigLcm = bigProd / gcd;
            int lcm = (int)bigLcm;

            for (int round = 0; round < 10000; round++)
            {
                for (int m = 0; m < monkeys.Count; m++)
                {
                    Monkey monkey = monkeys[m];

                    while (monkey.Items.TryDequeue(out long cur))
                    {
                        cur = monkey.Func(cur) % lcm;
                        monkey.Count++;

                        if ((cur % monkey.Test) == 0)
                        {
                            monkeys[ monkey.True ].Items.Enqueue(cur);
                        }
                        else
                        {
                            monkeys[ monkey.False ].Items.Enqueue(cur);
                        }
                    }
                }
            }

            long score = 1;

            foreach (Monkey m in monkeys.OrderByDescending(mon => mon.Count).Take(2))
            {
                score *= m.Count;
            }

            Console.WriteLine(score);
        }
    }
}
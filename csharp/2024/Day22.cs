using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day22
    {
        internal static void Problem1x()
        {
            long value = 123;

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(value);
                value = NextVal(value);
            }

            Console.WriteLine(value);
        }

        internal static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                long value = long.Parse(s);

                for (int i = 0; i < 2000; i++)
                {
                    value = NextVal(value);
                }

                ret += value;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            Dictionary<int, long> bananas = new();
            Dictionary<int, long> theseBananas = new();

            //int testKey = 0;
            
            // bigger sample
            //testKey |= (-2 & 0xFF);
            //testKey <<= 8;
            //testKey |= (1 & 0xFF);
            //testKey <<= 8;
            //testKey |= (-1 & 0xFF);
            //testKey <<= 8;
            //testKey |= (3 & 0xFF);

            // seeding sample
            //testKey |= (-1 & 0xFF);
            //testKey <<= 8;
            //testKey |= (-1 & 0xFF);
            //testKey <<= 8;
            //testKey |= (0 & 0xFF);
            //testKey <<= 8;
            //testKey |= (2 & 0xFF);

            //testKey = 0x0200FF02;

            foreach (string s in Data.Enumerate())
            {
                long value = long.Parse(s);
                int pos = 0;
                int last = 0;
                theseBananas.Clear();

                for (int i = 0; i < 2000; i++)
                {
                    long next = NextVal(value);
                    int part = (int)(value % 10);
                    int delta = part - last;
                    pos <<= 8;
                    pos |= (delta & 0xFF);
                    //Print.ForSample($"pos={pos:X8}, part={part}, delta={delta}, next={next}");
                    
                    if (i > 3)
                    {
                        if (theseBananas.TryAdd(pos, part))
                        {
                            //if (pos == testKey)
                            //{
                            //    Console.WriteLine(part);
                            //}
                        }
                    }

                    last = part;
                    value = next;
                }

                foreach (var kvp in theseBananas)
                {
                    bananas.Increment(kvp.Key, kvp.Value);
                }
            }

            var finalKvp = bananas.MaxBy(item => item.Value);
            Console.WriteLine($"{finalKvp.Key:X8} => {finalKvp.Value}");
        }

        private static long NextVal(long initialVal)
        {
            long secret = initialVal;
            secret ^= (secret * 64);
            secret %= 16777216;
            secret ^= (secret / 32);
            secret %= 16777216;
            secret ^= (secret * 2048);
            secret %= 16777216;
            return secret;
        }
    }
}
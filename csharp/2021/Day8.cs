using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day8
    {
        internal static void Problem1()
        {
            long count = 0;

            foreach (string line in Data.Enumerate())
            {
                string[] parts = line.Split(' ');
                Debug.Assert(parts[^5] == "|");

                for (int i = 4; i >= 1; i--)
                {
                    switch (parts[^i].Length)
                    {
                        case 2:
                        case 3:
                        case 4:
                        case 7:
                            count++;
                            break;
                        case 5:
                        case 6:
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            Console.WriteLine(count);
        }

        internal static void Problem2()
        {
            long bigSum = 0;

            foreach (string line in Data.Enumerate())
            {
                string[] parts = line.Split(' ');
                Debug.Assert(parts[^5] == "|");

                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = new string(parts[i].AsEnumerable().OrderBy(x => x).ToArray());
                }

                List<string> tests = new List<string>(parts.Take(10));
                string one = tests.Single(x => x.Length == 2);
                string seven = tests.Single(x => x.Length == 3);
                string four = tests.Single(x => x.Length == 4);
                string eight = tests.Single(x => x.Length == 7);

                tests.Remove(one);
                tests.Remove(seven);
                tests.Remove(four);
                tests.Remove(eight);

                string six = tests.Single(x => !eight.Except(x).Except(one).Any());
                tests.Remove(six);

                string zero = tests.Single(x => x.Length == 6 && !eight.Except(x).Except(four).Any());
                tests.Remove(zero);

                string nine = tests.Single(x => x.Length == 6);
                tests.Remove(nine);

                string two = tests.Single(x => !eight.Except(x).Except(four).Any());
                tests.Remove(two);

                string five = tests.Single(x => eight.Except(x).Except(one).Count() == 1);
                tests.Remove(five);

                string three = tests.Single();
                
//                Console.WriteLine($@"
//1: {one},
//7: {seven},
//4: {four},
//8: {eight},
//6: {six},
//0: {zero},
//9: {nine},
//2: {two},
//5: {five},
//3: {three}");


                Dictionary<string,char> map = new Dictionary<string, char>
                {
                    { one, '1' },
                    { two, '2' },
                    { three, '3' },
                    { four, '4' },
                    { five, '5' },
                    { six, '6' },
                    { seven, '7' },
                    { eight, '8' },
                    { nine, '9' },
                    { zero, '0' },
                };

                int offset = parts.Length - 4;
                char[] val = new char[4];

                for (int i = 0; i < val.Length; i++)
                {
                    val[i] = map[parts[i + offset]];
                }

                int value = int.Parse(val);
                bigSum += value;
            }

            Console.WriteLine(bigSum);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day06
    {
        internal static (List<long[]> Numbers, List<char> Operations) Load()
        {
            List<long[]> numbers = new();
            List<char> operations = new();

            foreach (string s in Data.Enumerate())
            {
                if (char.IsDigit(s[0]))
                {
                    numbers.Add(s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray());
                }
                else
                {
                    operations.AddRange(s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(str => str[0]));
                }
            }

            return (numbers, operations);
        }

        internal static void Problem1()
        {
            long ret = 0;
            List<long[]> numbers;
            List<char> operations;

            (numbers, operations) = Load();

            for (int col = 0; col < operations.Count; col++)
            {
                if (operations[col] == '+')
                {
                    for (int row = 0; row < numbers.Count; row++)
                    {
                        ret += numbers[row][col];
                    }
                }
                else if (operations[col] == '*')
                {
                    long loc = 1;

                    for (int row = 0; row < numbers.Count; row++)
                    {
                        loc *= numbers[row][col];
                    }

                    ret += loc;
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            string[] lines = Data.Enumerate().ToArray();
            int opRow = lines.Length - 1;
            string ops = lines[opRow];
            int lastTextCol = lines.Max(s => s.Length);
            int textCol = 0;

            List<long> curProblem = new List<long>();

            while (textCol < lastTextCol)
            {
                char op = ops[textCol];
                int term = ops.AsSpan(textCol + 1).IndexOfAnyExcept(' ');

                if (term < 0)
                {
                    term = lastTextCol;
                }
                else
                {
                    term += textCol;
                }

                curProblem.Clear();

                for (; textCol < term; textCol++)
                {
                    long cur = 0;

                    for (int row = 0; row < opRow; row++)
                    {
                        string rowLine = lines[row];

                        if (rowLine.Length > textCol)
                        {
                            char text = rowLine[textCol];

                            if (!char.IsWhiteSpace(text))
                            {
                                cur *= 10;
                                cur += (text - '0');
                            }
                        }
                    }

                    curProblem.Add(cur);
                }

                ret += op switch
                {
                    '+' => curProblem.Sum(),
                    '*' => curProblem.Product(),
                    _ => throw new InvalidOperationException(),
                };

                textCol++;
            }

            Console.WriteLine(ret);
        }
    }
}

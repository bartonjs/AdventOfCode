using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day10
    {
        internal static void Problem1()
        {
            Stack<char> expected = new Stack<char>();
            long totalScore = 0;

            foreach (string line in Data.Enumerate())
            {
                int lineScore = 0;
                expected.Clear();

                foreach (char c in line)
                {
                    switch (c)
                    {
                        case '(':
                            expected.Push(')');
                            break;
                        case '[':
                            expected.Push(']');
                            break;
                        case '{':
                            expected.Push('}');
                            break;
                        case '<':
                            expected.Push('>');
                            break;
                        case ')':
                        case ']':
                        case '}':
                        case '>':
                            char next = expected.Pop();

                            if (next != c)
                            {
                                switch (c)
                                {
                                    case ')':
                                        lineScore = 3;
                                        break;
                                    case ']':
                                        lineScore = 57;
                                        break;
                                    case '}':
                                        lineScore = 1197;
                                        break;
                                    case '>':
                                        lineScore = 25137;
                                        break;
                                }
                            }

                            break;
                    }

                    if (lineScore != 0)
                    {
                        totalScore += lineScore;
                        break;
                    }
                }
            }

            Console.WriteLine(totalScore);
        }

        internal static void Problem2()
        {
            Stack<char> expected = new Stack<char>();
            List<long> scores = new List<long>();

            int lineNum = 0;

            foreach (string line in Data.Enumerate())
            {
                lineNum++;
                bool invalid = false;
                expected.Clear();

                foreach (char c in line)
                {
                    switch (c)
                    {
                        case '(':
                            expected.Push(')');
                            break;
                        case '[':
                            expected.Push(']');
                            break;
                        case '{':
                            expected.Push('}');
                            break;
                        case '<':
                            expected.Push('>');
                            break;
                        case ')':
                        case ']':
                        case '}':
                        case '>':
                            char next = expected.Pop();

                            if (next != c)
                            {
                                Console.WriteLine($"Line {lineNum} is invalid: expected '{next}', actual '{c}'");
                                invalid = true;
                            }

                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    if (invalid)
                    {
                        break;
                    }
                }

                if (!invalid && expected.Count > 0)
                {
                    Console.Write($"Line {lineNum} is incomplete - {expected.Count} terms unclosed \"{string.Join("", expected.AsEnumerable())}\". ");

                    long score = 0;

                    while (expected.TryPop(out char next))
                    {
                        score *= 5;

                        score += next switch
                        {
                            ')' => 1,
                            ']' => 2,
                            '}' => 3,
                            '>' => 4,
                            _ => throw new InvalidOperationException(),
                        };
                    }

                    Console.WriteLine($"Score: {score}");
                    scores.Add(score);
                }
                else if (!invalid)
                {
                    throw new InvalidOperationException($"Line {lineNum} was valid.");
                }
            }

            scores.Sort();
            int mid = scores.Count / 2;

            Console.WriteLine($"{scores.Count} total scores.");
            Console.WriteLine($"{mid} score is {scores[mid]}.");
            Console.WriteLine($"{mid+1} score is {scores[mid + 1]}.");
        }
    }
}
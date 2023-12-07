using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal class Day05
    {
        private static Stack<char>[] LoadStacks()
        {
            // [C]         [S] [H]                
            // [F] [B]     [C] [S]     [W]        
            // [B] [W]     [W] [M] [S] [B]        
            // [L] [H] [G] [L] [P] [F] [Q]        
            // [D] [P] [J] [F] [T] [G] [M] [T]    
            // [P] [G] [B] [N] [L] [W] [P] [W] [R]
            // [Z] [V] [W] [J] [J] [C] [T] [S] [C]
            // [S] [N] [F] [G] [W] [B] [H] [F] [N]
            // 1   2   3   4   5   6   7   8   9 
            //return new[]
            //{
            //    null,
            //    new Stack<char>("SZPDLBFC"),
            //    new Stack<char>("NVGPHWB"),
            //    new Stack<char>("FWBJG"),
            //    new Stack<char>("GJNFLWCS"),
            //    new Stack<char>("WJLTPMSH"),
            //    new Stack<char>("BCWGFS"),
            //    new Stack<char>("HTPMQBW"),
            //    new Stack<char>("FSWT"),
            //    new Stack<char>("NCR"),
            //};

            List<string> data = new List<string>();

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    break;
                }

                data.Add(s);
            }

            string countLine = data[data.Count - 1].Trim();
            int lastSpace = countLine.LastIndexOf(' ');
            int count = int.Parse(countLine.AsSpan(lastSpace + 1));
            Stack<char>[] stacks = new Stack<char>[count + 1];

            for (int i = 1; i < stacks.Length; i++)
            {
                stacks[i] = new Stack<char>();
            }

            for (int i = data.Count - 2; i >= 0; i--)
            {
                ReadOnlySpan<char> line = data[i];

                int k = 1;

                for (int j = 1; j < line.Length; j += 4, k++)
                {
                    char c = line[j];

                    if (c != ' ')
                    {
                        stacks[k].Push(c);
                    }
                }
            }

            return stacks;
        }

        private static IEnumerable<(int Count, int From, int To)> EnumerateInstructions()
        {
            bool started = false;

            foreach (string s in Data.Enumerate())
            {
                if (!started)
                {
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        started = true;
                    }

                    continue;
                }

                int spaceIdx = s.IndexOf(' ', 5);
                int count = int.Parse(s.AsSpan(5, spaceIdx - 5));
                int fromIdx = s.IndexOf("from") + 5;
                spaceIdx = s.IndexOf(' ', fromIdx);
                int from = int.Parse(s.AsSpan(fromIdx, spaceIdx - fromIdx));
                int toIdx = s.IndexOf("to") + 3;
                int to = int.Parse(s.AsSpan(toIdx));

                yield return (count, from, to);
            }
        }

        internal static void Problem1()
        {
            Stack<char>[] stacks = LoadStacks();

            foreach ((int count, int from, int to) in EnumerateInstructions())
            {
                for (int i = 0; i < count; i++)
                {
                    char tmp = stacks[from].Pop();
                    stacks[to].Push(tmp);
                }
            }
            
            foreach (Stack<char> stack in stacks)
            {
                if (stack is null)
                {
                    continue;
                }

                Console.Write(stack.Peek());
            }

            Console.WriteLine();
        }

        internal static void Problem2()
        {
            Stack<char>[] stacks = LoadStacks();
            Stack<char> tmpStack = new Stack<char>();

            foreach ((int count, int from, int to) in EnumerateInstructions())
            {
                for (int i = 0; i < count; i++)
                {
                    char tmp = stacks[from].Pop();
                    tmpStack.Push(tmp);
                }

                while (tmpStack.TryPop(out char tmp))
                {
                    stacks[ to ].Push(tmp);
                }
            }
            
            foreach (Stack<char> stack in stacks)
            {
                if (stack is null)
                {
                    continue;
                }

                Console.Write(stack.Peek());
            }

            Console.WriteLine();
        }
    }
}
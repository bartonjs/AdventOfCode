using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day04
    {
        internal static void Problem1()
        {
            long sum = 0;

            foreach (string s in Data.Enumerate())
            {
                int pipe = s.IndexOf('|');
                ReadOnlySpan<char> winners = s.AsSpan(pipe + 2).Trim();
                HashSet<int> winNums = new(ParseNums(winners.ToString()));

                int colon = s.IndexOf(':');
                ReadOnlySpan<char> cards = s.AsSpan(colon + 2, pipe - colon - 3);
                cards = cards.Trim();

                long score = 0;

                foreach (int val in ParseNums(cards.ToString()))
                {
                    if (winNums.Contains(val))
                    {
                        if (score == 0)
                        {
                            score = 1;
                        }
                        else
                        {
                            score *= 2;
                        }
                    }
                }

                sum += score;
            }

            Console.WriteLine(sum);
        }

        private static IEnumerable<int> ParseNums(string winners)
        {
            int idx = winners.IndexOf(' ');

            while (idx >= 0)
            {
                int val = int.Parse(winners.Substring(0, idx));
                yield return val;
                winners = winners.Substring(idx + 1).Trim();

                idx = winners.IndexOf(' ');
            }

            yield return int.Parse(winners);
        }

        internal static void Problem2()
        {
            Dictionary<int, int> winState = new();
            winState[1] = 0;

            foreach (string s in Data.Enumerate())
            {
                int pipe = s.IndexOf('|');
                ReadOnlySpan<char> winners = s.AsSpan(pipe + 2).Trim();
                HashSet<int> winNums = new(ParseNums(winners.ToString()));

                int colon = s.IndexOf(':');
                ReadOnlySpan<char> cards = s.AsSpan(colon + 2, pipe - colon - 3);
                cards = cards.Trim();

                int cardNum = int.Parse(s.AsSpan(4, colon - 4).Trim());
                int myState = winState.Increment(cardNum);
                int score = 0;

                foreach (int val in ParseNums(cards.ToString()))
                {
                    if (winNums.Contains(val))
                    {
                        score++;
                    }
                }

                for (int i = 1; i <= score; i++)
                {
                    winState.Increment(cardNum + i, myState);
                }
            }

            Console.WriteLine(winState.Sum(kvp => kvp.Value));
        }
    }
}
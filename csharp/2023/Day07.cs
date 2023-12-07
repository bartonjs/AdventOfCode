using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day07
    {
        internal static void Problem1()
        {
            MetaProblem(Hand.Parse);
        }

        internal static void Problem2()
        {
            MetaProblem(Hand.ParseWithWilds);
        }

        private static void MetaProblem(Func<string, Hand> parser)
        {
            long ret = 0;

            List<(Hand Hand, long Bid)> hands = new();

            foreach (string s in Data.Enumerate())
            {
                string[] pieces = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                hands.Add((parser(pieces[0]), long.Parse(pieces[1])));
            }

            hands.Sort();

            long scale = hands.Count;

            foreach (var hand in hands)
            {
                long value = hand.Bid * scale;
                ret += value;

                scale--;
                Utils.TraceForSample(hand.ToString());
            }

            Console.WriteLine(ret);
        }

        private struct Hand : IComparable<Hand>
        {
            private static char[] DefaultOrdering = "AKQJT98765432".ToCharArray();
            private static char[] JacksLastOrdering = "AKQT98765432J".ToCharArray();

            private string Cards;
            private TierKind Tier;
            private char[] Ordering;

            public static Hand Parse(string s)
            {
                char[] chars = s.ToCharArray();
                TierKind tier = GetTier(chars);

                return new Hand
                {
                    Cards = s,
                    Tier = tier,
                    Ordering = DefaultOrdering,
                };
            }

            public static Hand ParseWithWilds(string s)
            {
                char[] chars = s.ToCharArray();
                TierKind max = GetTier(chars);

                if (max < TierKind.FiveOfAKind)
                {
                    max = MakeHands(chars).Select(GetTier).Max();
                }

                return new Hand
                {
                    Cards = s,
                    Tier = max,
                    Ordering = JacksLastOrdering,
                };
            }

            private static TierKind GetTier(char[] hand)
            {
                Dictionary<char, long> counts = Utils.CountBy(hand);

                if (counts.Count == 1)
                {
                    return TierKind.FiveOfAKind;
                }

                if (counts.Count == 2)
                {
                    long firstCount = counts.First().Value;

                    if (firstCount is 1 or 4)
                    {
                        return TierKind.FourOfAKind;
                    }

                    return TierKind.FullHouse;
                }

                if (counts.Count == 3)
                {
                    if (counts.Any(kvp => kvp.Value == 3))
                    {
                        return TierKind.ThreeOfAKind;
                    }

                    return TierKind.TwoPair;
                }

                if (counts.Count == 4)
                {
                    return TierKind.OnePair;
                }

                return TierKind.HighCard;
            }

            private static IEnumerable<char[]> MakeHands(char[] cards)
            {
                int idx = Array.IndexOf(cards, 'J');

                if (idx == -1)
                {
                    yield return cards;
                    yield break;
                }

                foreach (char c in cards.Distinct().ToList())
                {
                    if (c == 'J')
                    {
                        continue;
                    }

                    cards[idx] = c;

                    foreach (var nested in MakeHands(cards))
                    {
                        yield return nested;
                    }
                }

                cards[idx] = 'J';
            }

            public int CompareTo(Hand other)
            {
                int comp = other.Tier.CompareTo(Tier);

                if (comp == 0)
                {
                    for (int i = 0; i < Cards.Length; i++)
                    {
                        int a = Array.IndexOf(Ordering, Cards[i]);
                        int b = Array.IndexOf(Ordering, other.Cards[i]);

                        if (a != b)
                        {
                            return a - b;
                        }
                    }
                }

                return comp;
            }

            public override string ToString()
            {
                return $"Cards: {Cards}  Tier: {Tier}";
            }

            private enum TierKind
            {
                HighCard,
                OnePair,
                TwoPair,
                ThreeOfAKind,
                FullHouse,
                FourOfAKind,
                FiveOfAKind,
            }
        }
    }
}
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
            long ret = 0;

            List<(Hand Hand, long Bid)> hands = new();

            foreach (string s in Data.Enumerate())
            {
                string[] pieces = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                hands.Add((Hand.Parse(pieces[0]), long.Parse(pieces[1])));
            }

            hands.Sort();

            long scale = hands.Count;

            foreach (var hand in hands)
            {
                long value = hand.Bid * scale;
                ret += value;

                scale--;
                Console.WriteLine(hand);
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            List<(Hand2 Hand, long Bid)> hands = new();

            foreach (string s in Data.Enumerate())
            {
                string[] pieces = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                hands.Add((Hand2.Parse(pieces[0]), long.Parse(pieces[1])));
            }

            hands.Sort();

            long scale = hands.Count;

            foreach (var hand in hands)
            {
                long value = hand.Bid * scale;
                ret += value;

                scale--;
                Console.WriteLine(hand);
            }

            Console.WriteLine(ret);
        }

        private struct Hand : IComparable<Hand>
        {
            public string Cards;
            public int Tier;

            public static Hand Parse(string s)
            {
                char[] ordered = s.OrderBy(c => c).ToArray();
                int tier = 0;
                int distinct = ordered.Distinct().Count();

                if (ordered[0] == ordered[4])
                {
                    tier = 6;
                }
                else if (ordered[0] == ordered[3] || ordered[1] == ordered[4])
                {
                    tier = 5;
                }
                else if (ordered[0] == ordered[2] && ordered[3] == ordered[4] ||
                         ordered[0] == ordered[1] && ordered[2] == ordered[4])
                {
                    tier = 4;
                }
                else if (ordered[0] == ordered[2] || ordered[1] == ordered[3] || ordered[2] == ordered[4])
                {
                    tier = 3;
                }
                else if (distinct == 3)
                {
                    tier = 2;
                }
                else if (distinct == 4)
                {
                    tier = 1;
                }

                return new Hand
                {
                    Cards = s,
                    Tier = tier,
                };
            }

            private static char[] Ordering = "AKQJT98765432".ToCharArray();

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
        }

        private struct Hand2 : IComparable<Hand2>
        {
            public string Cards;
            public int Tier;

            public static Hand2 Parse(string s)
            {
                int tier = 0;

                char[] chars = s.ToCharArray();
                int max = MakeHands(chars).Select(GetTier).Max();

                return new Hand2
                {
                    Cards = s,
                    Tier = max,
                };
            }

            private static int GetTier(char[] hand)
            {
                char[] ordered = hand.OrderBy(c => c).ToArray();
                int distinct = ordered.Distinct().Count();
                int tier = 0;

                if (ordered[0] == ordered[4])
                {
                    tier = 6;
                }
                else if (ordered[0] == ordered[3] || ordered[1] == ordered[4])
                {
                    tier = 5;
                }
                else if (ordered[0] == ordered[2] && ordered[3] == ordered[4] ||
                         ordered[0] == ordered[1] && ordered[2] == ordered[4])
                {
                    tier = 4;
                }
                else if (ordered[0] == ordered[2] || ordered[1] == ordered[3] || ordered[2] == ordered[4])
                {
                    tier = 3;
                }
                else if (distinct == 3)
                {
                    tier = 2;
                }
                else if (distinct == 4)
                {
                    tier = 1;
                }

                return tier;
            }

            private static IEnumerable<char[]> MakeHands(char[] cards)
            {
                int idx = Array.IndexOf(cards, 'J');

                if (idx == -1)
                {
                    yield return cards;
                    yield break;
                }

                foreach (char c in Ordering.Take(Ordering.Length - 1))
                {
                    cards[idx] = c;

                    foreach (var nested in MakeHands(cards))
                    {
                        yield return nested;
                    }
                }

                cards[idx] = 'J';
            }

            private static char[] Ordering = "AKQT98765432J".ToCharArray();

            public int CompareTo(Hand2 other)
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
        }
    }
}
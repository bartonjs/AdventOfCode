using System;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day02
    {
        internal static void Problem1()
        {
            int sum = 0;
            
            foreach (string line in Data.Enumerate())
            {
                ReadOnlySpan<char> s = line.AsSpan(5);
                int colon = s.IndexOf(':');
                int gameId = int.Parse(s.Slice(0, colon));
                s = s.Slice(colon + 2);

                bool valid = true;

                while (valid && !s.IsEmpty)
                {
                    int semi = s.IndexOf(';');

                    if (semi < 0)
                    {
                        semi = s.Length;
                    }

                    ReadOnlySpan<char> game = s.Slice(0, semi);
                    s = s.Slice(semi);
                    
                    if (!s.IsEmpty)
                    {
                        s = s.Slice(2);
                    }

                    while (!game.IsEmpty)
                    {
                        int space = game.IndexOf(' ');
                        int val = int.Parse(game.Slice(0, space));

                        game = game.Slice(space + 1);

                        if (game.StartsWith("green"))
                        {
                            game = game.Slice(5);

                            if (val > 13)
                            {
                                valid = false;
                                break;
                            }
                        }
                        else if (game.StartsWith("red"))
                        {
                            game = game.Slice(3);

                            if (val > 12)
                            {
                                valid = false;
                                break;
                            }
                        }
                        else if (game.StartsWith("blue"))
                        {
                            game = game.Slice(4);

                            if (val > 14)
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (!game.IsEmpty)
                        {
                            game = game.Slice(2);
                        }
                    }
                }

                if (valid)
                {
                    sum += gameId;
                }
            }

            Console.WriteLine(sum);
        }

        internal static void Problem2()
        {
            int sum = 0;

            foreach (string line in Data.Enumerate())
            {
                ReadOnlySpan<char> s = line.AsSpan(5);
                int colon = s.IndexOf(':');
                s = s.Slice(colon + 2);

                int red = 0;
                int blue = 0;
                int green = 0;

                while (!s.IsEmpty)
                {
                    int semi = s.IndexOf(';');

                    if (semi < 0)
                    {
                        semi = s.Length;
                    }

                    ReadOnlySpan<char> game = s.Slice(0, semi);
                    s = s.Slice(semi);

                    if (!s.IsEmpty)
                    {
                        s = s.Slice(2);
                    }

                    while (!game.IsEmpty)
                    {
                        int space = game.IndexOf(' ');
                        int val = int.Parse(game.Slice(0, space));

                        game = game.Slice(space + 1);

                        if (game.StartsWith("green"))
                        {
                            game = game.Slice(5);

                            green = int.Max(green, val);
                        }
                        else if (game.StartsWith("red"))
                        {
                            game = game.Slice(3);

                            red = int.Max(red, val);
                        }
                        else if (game.StartsWith("blue"))
                        {
                            game = game.Slice(4);

                            blue = int.Max(blue, val);
                        }

                        if (!game.IsEmpty)
                        {
                            game = game.Slice(2);
                        }
                    }
                }

                int power = red * blue * green;
                sum += power;
            }

            Console.WriteLine(sum);
        }
    }
}
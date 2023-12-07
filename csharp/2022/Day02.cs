using System;
using AdventOfCode.Util;

#pragma warning disable CS8509

namespace AdventOfCode2022.Solutions
{
    internal class Day02
    {
        internal static void Problem1()
        {
            int total = 0;
            
            foreach (string s in Data.Enumerate())
            {
                int them = s[0] switch { 'A' => 0, 'B' => 1, 'C' => 2 };
                int us = s[2] switch { 'X' => 0, 'Y' => 1, 'Z' => 2 };

                int score = us + 1;

                if (us == them)
                {
                    score += 3;
                }
                else if (us == ((them + 1) % 3))
                {
                    score += 6;
                }

                total += score;
            }

            Console.WriteLine(total);
        }

        internal static void Problem2()
        {
            int total = 0;

            foreach (string s in Data.Enumerate())
            {
                int them = s[0] switch { 'A' => 0, 'B' => 1, 'C' => 2 };
                int us;
                int score = 0;

                switch (s[2])
                {
                    case 'X':
                        us = them - 1;

                        if (us < 0)
                        {
                            us += 3;
                        }

                        break;
                    case 'Y':
                        us = them;
                        score = 3;
                        break;
                    case 'Z':
                        us = (them + 1) % 3;
                        score = 6;
                        break;
                    default:
                        throw new NotFiniteNumberException();
                }

                score += us + 1;
                total += score;
            }

            Console.WriteLine(total);
        }
    }
}
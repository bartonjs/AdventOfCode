using System;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day10
    {
        internal static void Problem1()
        {
            int x = 1;
            int cycleCount = 1;
            int score = 0;

            foreach (Instruction ins in CommComputer.ReadInstructions())
            {
                switch (ins.Verb)
                {
                    case Verbs.AddX:
                        cycleCount++;

                        if (cycleCount is 20 or 60 or 100 or 140 or 180 or 220)
                        {
                            score += cycleCount * x;
                        }

                        cycleCount++;
                        x += ins.Arg1;
                        break;
                    case Verbs.NoOp:
                        cycleCount++;
                        break;
                }

                if (cycleCount is 20 or 60 or 100 or 140 or 180 or 220)
                {
                    score += cycleCount * x;
                }

                if (cycleCount > 220)
                {
                    break;
                }
            }

            Console.WriteLine(score);
        }

        internal static void Problem2()
        {
            int x = 1;
            int cycleCount = 1;

            foreach (Instruction ins in CommComputer.ReadInstructions())
            {
                switch (ins.Verb)
                {
                    case Verbs.AddX:
                        DrawPixel(cycleCount, x);
                        cycleCount++;
                        x += ins.Arg1;
                        DrawPixel(cycleCount, x);
                        cycleCount++;
                        break;
                    case Verbs.NoOp:
                        DrawPixel(cycleCount, x);
                        cycleCount++;
                        break;
                }
            }

            static void DrawPixel(int cycleCount, int x)
            {
                int col = (cycleCount - 1) % 40 + 1;

                if (col - x is -1 or 0 or 1)
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }

                if (col == 40)
                {
                    Console.WriteLine();
                }
            }
        }
    }
}
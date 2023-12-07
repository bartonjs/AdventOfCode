using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day17
    {
#if SAMPLE
        const int TargetXMin = 20;
        const int TargetXMax = 30;
        const int TargetYMin = -10;
        const int TargetYMax = -5;
#else
        const int TargetXMin = 57;
        const int TargetXMax = 116;
        const int TargetYMin = -198;
        const int TargetYMax = -148;
#endif

        internal static void Problem1()
        {
            int highestY = -1;
            (int, int) bestVelocity = (-1, -1);

            for (int vx = 1; vx <= TargetXMax; vx++)
            {
                for (int vy = 1; vy < 1000; vy++)
                {
                    int? trialHighestY = Simulate(vx, vy);

                    //Console.WriteLine($"({vx}, {vy}) => {trialHighestY}");

                    if (trialHighestY > highestY)
                    {
                        highestY = trialHighestY.Value;
                        bestVelocity = (vx, vy);
                    }
                }
            }

            Console.WriteLine(bestVelocity);
            Console.WriteLine(highestY);
        }

        internal static void Problem2()
        {
            int validEntries = 0;

            for (int vx = 1; vx <= TargetXMax; vx++)
            {
                for (int vy = TargetYMin; vy < 1000; vy++)
                {
                    if (Simulate(vx, vy).HasValue)
                    {
                        validEntries++;

                        Utils.TraceForSample($"{vx},{vy}");
                    }

                }
            }

            Console.WriteLine(validEntries);
        }

        private static int? Simulate(int vx, int vy)
        {
            int posX = 0;
            int posY = 0;
            int highestY = posY;
            bool inRange = false;

            //Console.WriteLine($"Simulate({vx}, {vy})");

            while (posX <= TargetXMax && posY >= TargetYMin)
            {
                posX += vx;
                posY += vy;

                highestY = Math.Max(highestY, posY);

                //Console.WriteLine($"  ({posX}, {posY}) {highestY == posY}");

                if (posY <= TargetYMax && posY >= TargetYMin &&
                    posX <= TargetXMax && posX >= TargetXMin)
                {
                    inRange = true;
                    break;
                }

                vx -= Math.Sign(vx);
                vy--;
            }

            //Console.WriteLine($"  inRange={inRange}");

            if (inRange)
            {
                return highestY;
            }

            return null;
        }
    }
}

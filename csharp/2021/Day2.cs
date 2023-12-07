using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day2
    {
        internal static void Problem1()
        {
            long hPos = 0;
            long depth = 0;
            
            foreach (string line in Data.Enumerate())
            {
                string[] parts = line.Split(' ');
                string dir = parts[0];
                int amt = int.Parse(parts[1]);

                switch (dir)
                {
                    case "forward":
                        hPos += amt;
                        break;
                    case "down":
                        depth += amt;
                        break;
                    case "up":
                        depth -= amt;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("direction", parts[0], "");
                }
            }

            Console.WriteLine($"hPos={hPos},depth={depth},answer={hPos*depth}");
        }

        internal static void Problem2()
        {
            long hPos = 0;
            long depth = 0;
            long aim = 0;

            foreach (string line in Data.Enumerate())
            {
                string[] parts = line.Split(' ');
                string dir = parts[0];
                int amt = int.Parse(parts[1]);

                switch (dir)
                {
                    case "forward":
                        hPos += amt;
                        depth += aim * amt;
                        break;
                    case "down":
                        aim += amt;
                        break;
                    case "up":
                        aim -= amt;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("direction", dir, "");
                }
            }

            Console.WriteLine($"hPos={hPos},depth={depth},answer={hPos * depth}");
        }
    }
}

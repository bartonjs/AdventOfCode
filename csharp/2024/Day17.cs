using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day17
    {
        private static (long[] Registers, List<int> Program) Load()
        {
            Regex regA = new Regex("^Register A: (-?\\d+)$");
            Regex regB = new Regex("^Register B: (-?\\d+)$");
            Regex regC = new Regex("^Register C: (-?\\d+)$");

            long a = 0;
            long b = 0;
            long c = 0;

            foreach (string data in Data.Enumerate())
            {
                if (string.IsNullOrEmpty(data))
                {
                    continue;
                }

                Match m = regA.Match(data);

                if (m.Success)
                {
                    a = int.Parse(m.Groups[1].ValueSpan);
                    continue;
                }

                m = regB.Match(data);

                if (m.Success)
                {
                    b = int.Parse(m.Groups[1].ValueSpan);
                    continue;
                }

                m = regC.Match(data);

                if (m.Success)
                {
                    c = int.Parse(m.Groups[1].ValueSpan);
                    continue;
                }

                return ([a, b, c], data.Substring("Program: ".Length).Split(',').Select(int.Parse).ToList());
            }

            throw new InvalidDataException();
        }

        internal static void Problem1()
        {
            (long[] registers, List<int> program) = Load();

            int ip = 0;

            while (true)
            {
                int inc = Compute(ip, registers, program);

                if (inc == int.MinValue)
                {
                    break;
                }

                ip += inc;
            }

            Console.WriteLine();
        }

        internal static void Problem2()
        {
            (long[] registers, List<int> program) = Load();
            long[] originalRegisters = registers.AsSpan().ToArray();
            string programStr = string.Join(",", program.Select(i => i.ToString())) + ",";

            StringBuilder builder = new();
            Queue<long> aValues = new();
            aValues.Enqueue(0);

            while (aValues.TryDequeue(out long a))
            {
                a <<= 3;

                for (int i = 0; i <= 7; i++)
                {
                    a = (a >> 3 << 3) | i;
                    builder.Clear();
                    long saveA = a;

                    originalRegisters.AsSpan().CopyTo(registers);
                    registers[0] = a;

                    int ip = 0;

                    while (true)
                    {
                        int inc = Compute(ip, registers, program, builder);

                        if (inc == int.MinValue)
                        {
                            break;
                        }

                        ip += inc;
                    }

                    string output = builder.ToString();
                    if (programStr.EndsWith(output))
                    {
                        Console.WriteLine(saveA);

                        Console.WriteLine(output);

                        if (programStr.Equals(output))
                        {
                            Console.WriteLine($"Submit {saveA}");
                            return;
                        }

                        aValues.Enqueue(a);
                    }
                }
            }
        }

        private static int Compute(int ip, long[] registers, List<int> program, StringBuilder builder = default)
        {
            if (ip > program.Count - 1)
            {
                return int.MinValue;
            }

            int op = program[ip];
            int arg = program[ip + 1];
            int inc = 2;

            switch (op)
            {
                case 0:
                    // adv
                    registers[0] /= (int)double.Pow(2, ComboReg(arg, registers));
                    break;
                case 1:
                    // bxl
                    registers[1] ^= arg;
                    break;
                case 2:
                    // bst
                    registers[1] = ComboReg(arg, registers) & 7;
                    break;
                case 3:
                    // jnz
                    if (registers[0] != 0)
                    {
                        int delta = arg - ip;
                        inc = delta;
                    }

                    break;
                case 4:
                    // bxc
                    registers[1] ^= registers[2];
                    break;
                case 5:
                    // out
                    string output = $"{ComboReg(arg, registers) & 7},";

                    if (builder is not null)
                    {
                        builder.Append(output);
                    }
                    else
                    {
                        Console.Write(output);
                    }

                    break;
                case 6:
                    // bdv
                    registers[1] = registers[0] / (int)double.Pow(2, ComboReg(arg, registers));
                    break;
                case 7:
                    registers[2] = registers[0] / (int)double.Pow(2, ComboReg(arg, registers));
                    break;
                default:
                    throw new InvalidDataException();
            }

            return inc;

            static long ComboReg(int arg, long[] registers)
            {
                return arg switch
                {
                    <4 => arg,
                    <6 => registers[arg - 4],
                };
            }
        }
    }
}
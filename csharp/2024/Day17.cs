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
            RunProgram(registers, program);
            Console.WriteLine();
        }

        internal static void Problem2()
        {
            (long[] registers, List<int> program) = Load();
            long[] originalRegisters = registers.AsSpan().ToArray();

            StringBuilder builder = new();
            Queue<(int,long)> aValues = new();
            aValues.Enqueue((program.Count - 1, 0));

            while (aValues.TryDequeue(out var tuple))
            {
                (int idx, long a) = tuple;
                int target = program[idx];
                a <<= 3;

                for (int i = 0; i <= 7; i++)
                {
#pragma warning disable CS0675
                    a = (a >> 3 << 3) | i;
#pragma warning restore CS0675
                    builder.Clear();

                    originalRegisters.AsSpan().CopyTo(registers);
                    registers[0] = a;

                    int ret = RunProgram(registers, program, true);

                    if (ret == target)
                    {
                        Console.WriteLine(a);

                        if (idx == 0)
                        {
                            Console.WriteLine($"Submit {a}");
                            return;
                        }

                        aValues.Enqueue((idx - 1, a));
                    }
                }
            }
        }

        private static int RunProgram(long[] registers, List<int> program, bool returnPrint = false)
        {
            int ip = 0;
            int lastPrint = int.MinValue;

            while (Compute(ref ip, registers, program, ref lastPrint, returnPrint))
            {
            }

            return lastPrint;
        }

        private static bool Compute(
            ref int ip,
            long[] registers,
            List<int> program,
            ref int lastPrint,
            bool returnPrint)
        {
            if (ip > program.Count - 1)
            {
                return false;
            }

            int op = program[ip];
            int arg = program[ip + 1];
            int inc = 2;

            switch (op)
            {
                case 0:
                    // adv
                    registers[0] >>= (int)ComboReg(arg, registers);
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
                    lastPrint = (int)(ComboReg(arg, registers) & 7);

                    if (returnPrint)
                    {
                        return false;
                    }

                    Console.Write($"{lastPrint},");
                    break;
                case 6:
                    // bdv
                    registers[1] = registers[0] >> (int)ComboReg(arg, registers);
                    break;
                case 7:
                    // cdv
                    registers[2] = registers[0] >> (int)ComboReg(arg, registers);
                    break;
                default:
                    throw new InvalidDataException();
            }

            ip += inc;
            return true;

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
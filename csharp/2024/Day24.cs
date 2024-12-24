using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day24
    {
        private class Operation
        {
            public readonly string A;
            public readonly char Op;
            public readonly string B;
            public string Res;

            public Operation(string a, char op, string b, string res)
            {
                A = a;
                Op = op;
                B = b;
                Res = res;
            }

            public void Deconstruct(out string A, out char Op, out string B, out string Res)
            {
                A = this.A;
                Op = this.Op;
                B = this.B;
                Res = this.Res;
            }
        }

        private static (Dictionary<string, bool> InitialValues, List<Operation> operations) Load()
        {
            Dictionary<string, bool> initialValues = new();
            List<Operation> operations = null;

            foreach (string s in Data.Enumerate())
            {
                if (operations is not null)
                {
                    string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    operations.Add(new Operation(
                        parts[0],
                        parts[1] switch
                        {
                            "XOR" => '^',
                            "AND" => '&',
                            "OR" => '|',
                        },
                        parts[2],
                        parts[4]));
                }
                else if (string.IsNullOrEmpty(s))
                {
                    operations = new();
                }
                else
                {
                    int colon = s.IndexOf(':');
                    initialValues[s.AsSpan(0, colon).ToString()] = s[colon + 2] != '0';
                }
            }

            return (initialValues, operations);
        }

        internal static void Problem1()
        {
            (Dictionary<string, bool> values, List<Operation> ops) = Load();

            long ret = RunLogic(values, ops);
            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            (Dictionary<string, bool> values, List<Operation> ops) = Load();
            Dictionary<string, bool> zeroVals = new Dictionary<string, bool>(values);

            foreach (string key in zeroVals.Keys)
            {
                zeroVals[key] = false;
            }

            Evaluate(ops, zeroVals, true);
            
            Console.WriteLine();
            Console.WriteLine();
            
            ToGraphviz(ops);
        }

        private static void ToGraphviz(List<Operation> ops)
        {
            Console.WriteLine("digraph {");

            foreach (Operation op in ops)
            {
                string color = op.Op switch
                {
                    '^' => "purple",
                    '&' => "blue",
                    '|' => "red",
                };

                Console.WriteLine($"  {op.A} -> {op.Res} [color={color}]");
                Console.WriteLine($"  {op.B} -> {op.Res} [color={color}]");
            }

            Console.WriteLine("}");
        }

        internal static void Problem2abandoned()
        {
            (Dictionary<string, bool> values, List<Operation> ops) = Load();
            Dictionary<(string, char, string), string> pending =
                ops.ToDictionary(op => (op.A, op.Op, op.B), op => op.Res);

            // Find build things up until something isn't right.
            // Then.. do something about it?

            string[] x = new string[64];
            string[] a = new string[64];
            string[] ca = new string[64];
            string[] c = new string[64];
            string[] s = new string[64];

            s[0] = TakeOp("x00", '^', "y00", pending);

            if (s[0] != "z00")
            {
                Console.WriteLine($"z00 is swapped with {s[0]}");
            }

            c[0] = TakeOp("x00", '&', "y00", pending);

            for (int i = 1; i < 64; i++)
            {
                string xName = $"x{i:D2}";
                string yName = $"y{i:D2}";

                if (!values.ContainsKey(xName))
                {
                    break;
                }

                x[i] = TakeOp(xName, '^', yName, pending);
                a[i] = TakeOp(xName, '&', yName, pending);
                ca[i] = TakeOp(a[i], '^', c[i - 1], pending);
                c[i] = TakeOp(ca[i], '|', a[i], pending);
                s[i] = TakeOp(x[i], '^', c[i - 1], pending);

                if (s[i] != $"z{i:D2}")
                {
                    Console.WriteLine($"z{i:D2} is swapped with {s[0]}");
                }
            }

            static string TakeOp(string node1, char op, string node2, Dictionary<(string, char, string), string> dict)
            {
                if (!dict.Remove((node1, op, node2), out string value))
                {
                    if (!dict.Remove((node2, op, node1), out value))
                    {
                        throw new InvalidDataException();
                    }

                    return value;
                }

                return value;
            }
        }

        private static int Evaluate(List<Operation> ops, Dictionary<string, bool> zeroVals, bool print = false)
        {
            int badBits = 0;

            for (int i = 63; i >= 0; i--)
            {
                string id2 = i.ToString("D2");
                string yKey = "y" + id2;

                if (!zeroVals.ContainsKey(yKey))
                {
                    continue;
                }

                Dictionary<string, bool> thisVal = new Dictionary<string, bool>(zeroVals);
                thisVal[yKey] = true;

                long expected = 1L << i;
                long actual = RunLogic(thisVal, new List<Operation>(ops));

                bool interesting = actual != expected;

                if (interesting)
                {
                    badBits++;

                    if (print)
                    {
                        Console.WriteLine($"Y is wrong in bit {i}");
                    }
                }

                thisVal[yKey] = false;
                string xKey = "x" + id2;
                thisVal[xKey] = true;

                actual = RunLogic(thisVal, new List<Operation>(ops));

                bool xInteresting = actual != expected;

                if (xInteresting != interesting)
                {
                    throw new InvalidDataException("X-interesting != Y-interesting");
                }
            }

            return badBits;
        }

        private static long RunLogic(
            Dictionary<string, bool> values,
            List<Operation> ops)
        {
            while (ops.Count > 0)
            {
                bool removed = false;

                for (int i = ops.Count - 1; i >= 0; i--)
                {
                    var op = ops[i];

                    if (values.TryGetValue(op.A, out bool a) && values.TryGetValue(op.B, out bool b))
                    {
                        values[op.Res] = op.Op switch
                        {
                            '^' => a ^ b,
                            '&' => a & b,
                            '|' => a | b,
                        };

                        ops.RemoveAt(i);
                        removed = true;
                    }
                }

                if (!removed)
                {
                    throw new InvalidDataException();
                }
            }

            long ret = 0;

            for (int i = 0; i < 64; i++)
            {
                string key = $"z{i:D2}";

                if (!values.TryGetValue(key, out bool value))
                {
                    break;
                }

                if (value)
                {
                    ret |= 1L << i;
                }
            }

            return ret;
        }
    }
}
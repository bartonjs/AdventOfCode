using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AdventOfCode.Util;

namespace AdventOfCode2025
{
    public class Day10
    {
        private static List<ProblemState> Load()
        {
            List<ProblemState> ret = new();

            foreach (string s in Data.Enumerate())
            {
                ret.Add(ProblemState.Load(s));
            }

            return ret;
        }

        internal static void Problem1()
        {
            long ret = 0;

            foreach (ProblemState problemState in Load())
            {
                int score = FewestPresses(problemState);
                Utils.TraceForSample("" + score);
                ret += score;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;

            foreach (ProblemState problemState in Load())
            {
                long machineSum = FewestPresses2(problemState);
                Utils.TraceForSample($"Machine sum: {machineSum}");
                ret += machineSum;
            }

            Console.WriteLine(ret);
        }

        private static int FewestPresses(ProblemState problemState)
        {
            Dictionary<int, int> mins = new();
            PriorityQueue<int, int> queue = new();
            queue.Enqueue(0, 0);

            while (queue.TryDequeue(out int state, out int presses))
            {
                if (state == problemState.RequiredPattern)
                {
                    return presses;
                }

                ref int min = ref CollectionsMarshal.GetValueRefOrAddDefault(mins, state, out bool exists);

                if (!exists || min > presses)
                {
                    min = presses;
                    int morePresses = presses + 1;

                    foreach (int newState in Next(state, problemState))
                    {
                        queue.Enqueue(newState, morePresses);
                    }
                }
            }

            throw new InvalidOperationException();

            static IEnumerable<int> Next(
                int current,
                ProblemState problemState)
            {
                foreach (int toggle in problemState.Toggles)
                {
                    yield return current ^ toggle;
                }
            }
        }

        private static long FewestPresses2(ProblemState problemState)
        {
            long ret = 0;

            using (Microsoft.Z3.Context ctx = new())
            using (Microsoft.Z3.Optimize optimize = ctx.MkOptimize())
            {
                Microsoft.Z3.ArithExpr[] pressCounts = new Microsoft.Z3.ArithExpr[problemState.Toggles.Count];
                Microsoft.Z3.IntExpr zero = ctx.MkInt(0);

                for (int i = 0; i < pressCounts.Length; i++)
                {
                    Microsoft.Z3.IntExpr button = ctx.MkIntConst($"b{i}");
                    pressCounts[i] = button;
                    optimize.Add(ctx.MkGe(button, zero));
                }

                for (int i = 0; i < problemState.Joltages.Count; i++)
                {
                    Microsoft.Z3.IntExpr target = ctx.MkInt(problemState.Joltages[i]);

                    Microsoft.Z3.ArithExpr sumConstraint = ctx.MkAdd(
                        problemState.Toggles.Select((value, index) => (Value: value, Index: index))
                            .Where(w => (w.Value & (1 << i)) != 0).
                            Select(s => pressCounts[s.Index]));

                    optimize.Add(ctx.MkEq(target, sumConstraint));
                }

                Microsoft.Z3.ArithExpr targetSum = ctx.MkAdd(pressCounts);
                optimize.MkMinimize(targetSum);

                if (optimize.Check() != Microsoft.Z3.Status.SATISFIABLE)
                {
                    throw new InvalidOperationException();
                }

                Microsoft.Z3.Model model = optimize.Model;

                for (int i = 0; i < pressCounts.Length; i++)
                {
                    var count = (Microsoft.Z3.IntNum)model.Eval(pressCounts[i], true);
                    Utils.TraceForSample($"Button {i}: {count.Int64}");
                    ret += count.Int64;
                }
            }

            return ret;
        }

        private class ProblemState
        {
            internal readonly int RequiredPattern;
            internal readonly int ObservedBitCount;
            internal readonly List<int> Toggles;
            internal readonly List<int> Joltages;

            private ProblemState(int requiredPattern, int observedBitCount, List<int> toggles, List<int> joltages)
            {
                RequiredPattern = requiredPattern;
                ObservedBitCount = observedBitCount;
                Toggles = toggles;
                Joltages = joltages;
            }

            internal static ProblemState Load(string s)
            {
                int closeSquare = s.IndexOf(']');
                int requiredPattern = 0;

                for (int i = closeSquare - 1; i > 0; i--)
                {
                    requiredPattern <<= 1;

                    if (s[i] == '#')
                    {
                        requiredPattern |= 1;
                    }
                }

                int openParen = closeSquare + 2;
                int closeParen = s.IndexOf(')', openParen + 2);
                int comma = openParen;
                int nextComma;
                int val;
                List<int> toggles = new();

                while (closeParen > 0)
                {
                    int toggle = 0;

                    foreach (string part in s.Substring(openParen + 1, closeParen - openParen - 1).Split(','))
                    {
                        int num = int.Parse(part);
                        int bit = 1 << num;
                        toggle |= bit;
                    }

                    toggles.Add(toggle);
                    openParen = closeParen + 2;
                    closeParen = s.IndexOf(')', openParen + 1);
                }

                int openCurly = s.IndexOf('{');
                int closeCurly = s.IndexOf('}', openCurly);
                comma = openCurly;
                nextComma = s.IndexOf(',', comma);

                List<int> joltages = new();

                while (nextComma > 0)
                {
                    val = int.Parse(s.AsSpan(comma + 1, nextComma - comma - 1));
                    joltages.Add(val);
                    comma = nextComma;
                    nextComma = s.IndexOf(',', comma + 1);
                }

                val = int.Parse(s.AsSpan(comma + 1, closeCurly - comma - 1));
                joltages.Add(val);

                ProblemState ret = new ProblemState(requiredPattern, closeSquare - 1, toggles, joltages);
                Debug.Assert(ret.ToString() == s);
                return ret;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append('[');

                for (int i = 0; i < ObservedBitCount; i++)
                {
                    bool hasBit = (RequiredPattern & (1 << i)) != 0;

                    builder.Append(hasBit ? '#' : '.');
                }

                builder.Append(']');

                foreach (int toggle in Toggles)
                {
                    builder.Append(" (");

                    int log2 = int.Log2(toggle) + 1;
                    bool first = true;
                    for (int i = 0; i < log2; i++)
                    {
                        if ((toggle & (1 << i)) != 0)
                        {
                            if (!first)
                            {
                                builder.Append(',');
                            }

                            builder.Append(i);
                            first = false;
                        }
                    }

                    builder.Append(')');
                }

                builder.Append(" {");
                builder.AppendJoin(',', Joltages);
                builder.Append('}');

                return builder.ToString();
            }
        }
    }
}

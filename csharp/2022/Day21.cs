using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day21
    {
        [GeneratedRegex(@"(....): (-?\d+)")]
        private static partial Regex SimpleRegex();

        [GeneratedRegex(@"(....): (....) (.) (....)")]
        private static partial Regex MathRegex();

        private static Dictionary<string, Monkey> Load(long scale = 1)
        {
            Dictionary<string, Monkey> monkeys = new Dictionary<string, Monkey>();

            Regex simple = SimpleRegex();
            Regex math = MathRegex();

            foreach (string s in Data.Enumerate())
            {
                Match m = simple.Match(s);

                if (m.Success)
                {
                    monkeys.Add(m.Groups[1].Value, new Monkey(long.Parse(m.Groups[2].ValueSpan)));
                }
                else
                {
                    m = math.Match(s);

                    if (!m.Success)
                    {
                        throw new InvalidOperationException();
                    }

                    monkeys.Add(
                        m.Groups[1].Value,
                        new Monkey(m.Groups[2].Value, m.Groups[3].ValueSpan[0],
                            m.Groups[4].Value));
                }
            }

            return monkeys;
        }

        private class Monkey
        {
            internal long Value;
            internal bool DeferredValue;
            internal string Other1;
            internal string Other2;
            internal char Operator;
            internal bool Humn;

            internal Monkey(long value)
            {
                DeferredValue = false;
                Value = value;
            }

            internal Monkey(string one, char op, string two)
            {
                DeferredValue = true;
                Other1 = one;
                Operator = op;
                Other2 = two;
            }

            internal long Evaluate(Dictionary<string, Monkey> monkeys)
            {
                if (!DeferredValue)
                {
                    return Value;
                }

                Monkey one = monkeys[Other1];
                Monkey two = monkeys[Other2];

                long oneVal = one.Evaluate(monkeys);
                long twoVal = two.Evaluate(monkeys);
                long ret;

                switch (Operator)
                {
                    case '+':
                        ret = oneVal + twoVal;
                        break;
                    case '-':
                        ret = oneVal - twoVal;
                        break;
                    case '/':
                        ret = oneVal / twoVal;
                        break;
                    case '*':
                        ret = oneVal * twoVal;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Value = ret;
                //DeferredValue = false;
                return ret;
            }

            internal bool PartiallyEvaluate(Dictionary<string, Monkey> monkeys)
            {
                if (Humn)
                {
                    return false;
                }

                if (!DeferredValue)
                {
                    return true;
                }

                Monkey one = monkeys[Other1];
                Monkey two = monkeys[Other2];

                bool canLeft = one.PartiallyEvaluate(monkeys);
                bool canRight = two.PartiallyEvaluate(monkeys);

                if (canLeft && canRight)
                {
                    long oneVal = one.Value;
                    long twoVal = two.Value;
                    long ret;

                    switch (Operator)
                    {
                        case '+':
                            ret = oneVal + twoVal;
                            break;
                        case '-':
                            ret = oneVal - twoVal;
                            break;
                        case '/':
                            ret = oneVal / twoVal;
                            break;
                        case '*':
                            ret = oneVal * twoVal;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    Value = ret;
                    DeferredValue = false;
                    return true;
                }

                return false;
            }

            internal void ReverseEvaluate(Dictionary<string, Monkey> monkeys, long expressionResult)
            {
                if (Humn)
                {
                    Value = expressionResult;
                    return;
                }

                if (!DeferredValue)
                {
                    if (Value != expressionResult)
                    {
                        throw new InvalidOperationException();
                    }

                    return;
                }

                Monkey left = monkeys[Other1];
                Monkey right = monkeys[Other2];
                long fixedValue;

                if (left.PartiallyEvaluate(monkeys))
                {
                    fixedValue = left.Value;

                    switch (Operator)
                    {
                        case '+':
                            right.ReverseEvaluate(monkeys, expressionResult - fixedValue);
                            break;
                        case '-':
                            right.ReverseEvaluate(monkeys, fixedValue - expressionResult);
                            break;
                        case '*':
                            right.ReverseEvaluate(monkeys, expressionResult / fixedValue);
                            break;
                        case '/':
                            right.ReverseEvaluate(monkeys, fixedValue / expressionResult);
                            break;
                        case '=':
                            right.ReverseEvaluate(monkeys, fixedValue);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    right.PartiallyEvaluate(monkeys);
                    fixedValue = right.Value;

                    switch (Operator)
                    {
                        case '+':
                            left.ReverseEvaluate(monkeys, expressionResult - fixedValue);
                            break;
                        case '-':
                            left.ReverseEvaluate(monkeys, fixedValue + expressionResult);
                            break;
                        case '*':
                            left.ReverseEvaluate(monkeys, expressionResult / fixedValue);
                            break;
                        case '/':
                            left.ReverseEvaluate(monkeys, fixedValue * expressionResult);
                            break;
                        case '=':
                            left.ReverseEvaluate(monkeys, fixedValue);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            internal string PrettyPrint(Dictionary<string, Monkey> monkeys)
            {
                StringBuilder builder = new StringBuilder();
                ToString(builder, monkeys);
                return builder.ToString();
            }

            private void ToString(StringBuilder builder, Dictionary<string, Monkey> monkeys)
            {
                if (Humn)
                {
                    builder.Append("humn");
                }
                else if (!DeferredValue)
                {
                    builder.Append(Value);
                }
                else
                {
                    builder.Append('(');
                    monkeys[Other1].ToString(builder, monkeys);
                    builder.Append(' ');
                    builder.Append(Operator);
                    builder.Append(' ');
                    monkeys[Other2].ToString(builder, monkeys);
                    builder.Append(')');
                }
            }
        }

        internal static void Problem1()
        {
            Dictionary<string, Monkey> monkeys = Load();
            Monkey root = monkeys["root"];
            Console.WriteLine(root.Evaluate(monkeys));
        }

        internal static void Problem2()
        {
            Dictionary<string, Monkey> monkeys = Load();
            Monkey root = monkeys["root"];
            Monkey humn = monkeys["humn"];
            
            humn.Humn = true;
            root.Operator = '=';
            root.ReverseEvaluate(monkeys, 0);

            Console.WriteLine(humn.Value);
        }
    }
}
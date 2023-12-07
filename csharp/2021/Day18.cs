using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day18
    {
        internal static void Problem1()
        {
            //{
            //    SnailfishNode left = SnailfishNode.Parse("[[[[4,3],4],4],[7,[[8,4],9]]]");
            //    SnailfishNode right = SnailfishNode.Parse("[1,1]");

            //    SnailfishNode sum = left.Add(right);
            //    Console.WriteLine(sum);

            //    while (sum.ReduceOne())
            //    {
            //        Console.WriteLine(sum);
            //    }

            //    Console.WriteLine(sum);
            //}

            {
                SnailfishNode sum = null;

                foreach (string line in Data.Enumerate())
                {
                    SnailfishNode next = SnailfishNode.Parse(line);

                    if (sum != null)
                    {
                        Utils.TraceForSample($"{sum} + {next}");
                        sum = sum.Add(next);
                        sum.Reduce();
                        Utils.TraceForSample($"  {sum}");
                    }
                    else
                    {
                        sum = next;
                    }
                }

                Console.WriteLine(sum);
                Console.WriteLine(sum.Magnitude());
            }
        }

        internal static void Problem2()
        {
            List<string> inputs = new List<string>(Data.Enumerate());

            int maxMag = int.MinValue;

            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = 0; j < inputs.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    SnailfishNode left = SnailfishNode.Parse(inputs[i]);
                    SnailfishNode right = SnailfishNode.Parse(inputs[j]);
                    SnailfishNode sum = left.Add(right);
                    sum.Reduce();

                    int mag = sum.Magnitude();
                    maxMag = Math.Max(mag, maxMag);
                }
            }

            Console.WriteLine(maxMag);
        }

        private class SnailfishNode
        {
            public SnailfishNode Parent { get; private set; }
            public SnailfishNode LeftNode { get; private set; }
            public int? LeftValue { get; private set; }
            public SnailfishNode RightNode { get; private set; }
            public int? RightValue { get; private set; }

            public static SnailfishNode Parse(ReadOnlySpan<char> input)
            {
                SnailfishNode node = Parse(input, out int charsRead);
                Debug.Assert(charsRead == input.Length);
                return node;
            }

            private static SnailfishNode Parse(ReadOnlySpan<char> input, out int charsRead)
            {
                Debug.Assert(input[0] == '[');
                input = input.Slice(1);

                charsRead = 1;
                SnailfishNode left = null;
                int? leftValue = null;
                SnailfishNode right = null;
                int? rightValue = null;
                int skip;

                if (input[0] == '[')
                {
                    left = Parse(input, out skip);
                    input = input.Slice(skip);
                    charsRead += skip;
                }
                else
                {
                    leftValue = input[0] - '0';
                    input = input.Slice(1);
                    charsRead++;
                }

                Debug.Assert(input[0] == ',');
                charsRead++;
                input = input.Slice(1);

                if (input[0] == '[')
                {
                    right = Parse(input, out skip);
                    input = input.Slice(skip);
                    charsRead += skip;
                }
                else
                {
                    rightValue = input[0] - '0';
                    input = input.Slice(1);
                    charsRead++;
                }

                Debug.Assert(input[0] == ']');
                charsRead++;

                SnailfishNode node = new SnailfishNode
                {
                    LeftNode = left,
                    LeftValue = leftValue,
                    RightNode = right,
                    RightValue = rightValue,
                };

                if (left != null)
                {
                    left.Parent = node;
                }

                if (right != null)
                {
                    right.Parent = node;
                }

                return node;
            }

            public SnailfishNode Add(SnailfishNode other)
            {
                SnailfishNode node = new SnailfishNode
                {
                    LeftNode = this,
                    RightNode = other,
                };

                Parent = node;
                other.Parent = node;
                return node;
            }

            public bool Reduce()
            {
                bool reduced = false;

                while (ReduceOne())
                {
                    reduced = true;
                }

                return reduced;
            }

            public bool ReduceOne()
            {
                return Explode(1) || Split();
            }

            private bool Split()
            {
                if (LeftValue > 9)
                {
                    LeftNode = MakeSplitNode(LeftValue.Value);
                    LeftValue = null;
                    //Console.WriteLine("Split (left)");
                    return true;
                }

                if (LeftNode?.Split() ?? false)
                {
                    return true;
                }

                if (RightValue > 9)
                {
                    RightNode = MakeSplitNode(RightValue.Value);
                    RightValue = null;
                    //Console.WriteLine("Split (right)");
                    return true;
                }

                return RightNode?.Split() ?? false;
            }

            private SnailfishNode MakeSplitNode(int value)
            {
                int low = value / 2;
                int high = (value + 1) / 2;

                return new SnailfishNode { LeftValue = low, RightValue = high, Parent = this };
            }

            private bool Explode(int depth)
            {
                if ((LeftNode?.Explode(depth + 1) ?? false) || (RightNode?.Explode(depth + 1) ?? false))
                {
                    return true;
                }

                if (depth > 4)
                {
                    Debug.Assert(LeftValue.HasValue && RightValue.HasValue);

                    (SnailfishNode peerNumberNode, bool useRight) = Parent.FindRightmostLeftValue(this);

                    if (peerNumberNode != null)
                    {
                        if (useRight)
                        {
                            peerNumberNode.RightValue += LeftValue;
                        }
                        else
                        {
                            peerNumberNode.LeftValue += LeftValue;
                        }
                    }

                    (peerNumberNode, useRight) = Parent.FindLeftmostRightValue(this);

                    if (peerNumberNode != null)
                    {
                        if (useRight)
                        {
                            peerNumberNode.RightValue += RightValue;
                        }
                        else
                        {
                            peerNumberNode.LeftValue += RightValue;
                        }
                    }

                    Parent.Zero(this);
                    //Console.WriteLine("Exploded");
                    return true;
                }

                return false;
            }

            private void Zero(SnailfishNode child)
            {
                if (LeftNode == child)
                {
                    LeftValue = 0;
                    LeftNode = null;
                }
                else
                {
                    Debug.Assert(RightNode == child);
                    RightValue = 0;
                    RightNode = null;
                }
            }

            private (SnailfishNode, bool) FindRightmostLeftValue(SnailfishNode from)
            {
                if (LeftValue.HasValue)
                {
                    return (this, false);
                }

                if (from == RightNode)
                {
                    return LeftNode.FindRightmostValue();
                }

                if (Parent == null)
                {
                    return (null, false);
                }

                return Parent.FindRightmostLeftValue(this);
            }

            private (SnailfishNode, bool) FindRightmostValue()
            {
                if (RightValue.HasValue)
                {
                    return (this, true);
                }

                return RightNode.FindRightmostValue();
            }

            private (SnailfishNode, bool) FindLeftmostRightValue(SnailfishNode from)
            {
                if (RightValue.HasValue)
                {
                    return (this, true);
                }

                if (from == LeftNode)
                {
                    return RightNode.FindLeftmostValue();
                }

                if (Parent == null)
                {
                    return (null, false);
                }

                return Parent.FindLeftmostRightValue(this);
            }

            private (SnailfishNode, bool) FindLeftmostValue()
            {
                if (LeftValue.HasValue)
                {
                    return (this, false);
                }

                return LeftNode.FindLeftmostValue();
            }

            private int LeftMagnitude()
            {
                return 3 * (LeftNode?.Magnitude() ?? LeftValue).Value;
            }

            private int RightMagnitude()
            {
                return 2 * (RightNode?.Magnitude() ?? RightValue).Value;
            }

            public int Magnitude()
            {
                return LeftMagnitude() + RightMagnitude();
            }

            public override string ToString()
            {
                string leftStr = LeftValue.HasValue ? LeftValue.ToString() : LeftNode?.ToString();
                string rightStr = RightValue.HasValue ? RightValue.ToString() : RightNode?.ToString();

                return $"[{leftStr},{rightStr}]";
            }
        }
    }
}

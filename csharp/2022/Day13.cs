using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day13
    {
        private sealed class Node : IComparable<Node>
        {
            public int Value;
            public List<Node> List;
            public Node Parent;

            public int CompareTo(Node other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;

                if (List is null && other.List is null)
                {
                    return Value.CompareTo(other.Value);
                }
                else if (List is null)
                {
                    Node virtualNode = new Node()
                    {
                        List = new List<Node> { new Node { Value = Value } },
                    };

                    return virtualNode.CompareTo(other);
                }
                else if (other.List is null)
                {
                    Node virtualNode = new Node()
                    {
                        List = new List<Node> { new Node { Value = other.Value } },
                    };

                    return CompareTo(virtualNode);
                }
                else
                {
                    int min = Math.Min(List.Count, other.List.Count);

                    for (int i = 0; i < min; i++)
                    {
                        int cmp = List[i].CompareTo(other.List[i]);

                        if (cmp != 0)
                        {
                            return cmp;
                        }
                    }

                    if (List.Count < other.List.Count)
                    {
                        return -1;
                    }
                    else if (other.List.Count < List.Count)
                    {
                        return 1;
                    }
                }

                return 0;
            }

            public override string ToString()
            {
                if (List is null)
                {
                    return Value.ToString();
                }

                StringBuilder builder = new StringBuilder();
                ToString(builder);
                return builder.ToString();
            }

            private void ToString(StringBuilder builder)
            {
                if (List is null)
                {
                    builder.Append(Value.ToString());
                }
                else
                {
                    builder.Append('[');
                    bool first = true;

                    foreach (Node n in List)
                    {
                        if (!first)
                        {
                            builder.Append(',');
                            first = false;

                        }

                        n.ToString(builder);
                    }

                    builder.Append(']');
                }
            }

            public static Node Parse(ReadOnlySpan<char> s)
            {
                Node cur = new Node();
                ReadOnlySpan<char> numbers = "0123456789";

                while (!s.IsEmpty)
                {
                    char c = s[0];

                    if (c == '[')
                    {
                        if (cur.List is null)
                        {
                            cur.List = new List<Node>();
                        }
                        else
                        {
                            Node next = new Node
                            {
                                List = new List<Node>(),
                                Parent = cur,
                            };

                            cur.List.Add(next);
                            cur = next;
                        }

                        s = s.Slice(1);
                    }
                    else if (c == ']')
                    {
                        if (cur.Parent is null)
                        {
                            Debug.Assert(s.Length == 1);
                            return cur;
                        }

                        cur = cur.Parent;
                        s = s.Slice(1);
                    }
                    else if (c == ',')
                    {
                        s = s.Slice(1);
                    }
                    else
                    {
                        int until = s.IndexOfAnyExcept(numbers);
                        int value;

                        if (until >= 0)
                        {
                            value = int.Parse(s.Slice(0, until));
                            s = s.Slice(until);
                        }
                        else
                        {
                            value = int.Parse(s);
                            s = default;
                        }

                        if (cur.List is not null)
                        {
                            cur.List.Add(new Node { Value = value });
                        }
                        else
                        {
                            cur.Value = value;
                        }
                    }
                }

                return cur;
            }
        }

        private static IEnumerable<(Node,Node)> LoadData()
        {
            Node first = null;

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                Node cur = Node.Parse(s);

                if (first is null)
                {
                    first = cur;
                }
                else
                {
                    yield return (first, cur);
                    first = null;
                }
            }
        }

        internal static void Problem1()
        {
            int index = 1;
            int score = 0;

            foreach ((Node first, Node second) in LoadData())
            {
                int cmp = first.CompareTo(second);

                if (cmp < 0)
                {
                    score += index;
                }

//#if SAMPLE
                Console.WriteLine($"Pair {index} compared as {cmp}");
//#endif
                index++;
            }

            Console.WriteLine(score);
        }

        internal static void Problem2()
        {
            List<Node> nodes = new List<Node>();

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                nodes.Add(Node.Parse(s));
            }

            Node a = Node.Parse("[[2]]");
            Node b = Node.Parse("[[6]]");

            nodes.Add(a);
            nodes.Add(b);

            nodes.Sort();

            int idxA = nodes.IndexOf(a) + 1;
            int idxB = nodes.IndexOf(b) + 1;

#if SAMPLE
            foreach (Node n in nodes)
            {
                Console.WriteLine(n);
            }
#endif

            Console.WriteLine($"idxA:{idxA} idxB:{idxB}");
            Console.WriteLine(idxA * idxB);
        }
    }
}
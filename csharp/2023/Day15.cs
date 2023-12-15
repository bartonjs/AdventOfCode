using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day15
    {
        internal static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(',');
                ret += parts.Sum(s => Day15Hash(s));
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            string[] instructions = null;

            foreach (string s in Data.Enumerate())
            {
                instructions = s.Split(',');
            }

            Box[] boxes = new Box[256];

            foreach (string instruction in instructions)
            {
                if (instruction.EndsWith('-'))
                {
                    ReadOnlySpan<char> label = instruction.AsSpan(0, instruction.Length - 1);
                    int boxId = Day15Hash(label);
                    Box box = boxes[boxId];

                    if (box is not null)
                    {
                        string labelStr = label.ToString();

                        int idx = box.Labels.IndexOf(labelStr);

                        if (idx >= 0)
                        {
                            box.Labels.RemoveAt(idx);
                            box.Lenses.RemoveAt(idx);
                        }
                    }
                }
                else
                {
                    string[] parts = instruction.Split('=');
                    int focalLen = int.Parse(parts[1]);
                    string label = parts[0];
                    int boxId = Day15Hash(label);

                    Box box = boxes[boxId];

                    if (box is null)
                    {
                        box = boxes[boxId] = new Box();
                    }

                    int idx = box.Labels.IndexOf(label);

                    if (idx < 0)
                    {
                        box.Labels.Add(label);
                        box.Lenses.Add(focalLen);
                    }
                    else
                    {
                        box.Lenses[idx] = focalLen;
                    }
                }
            }

            long ret = 0;

            for (int i = 0; i < boxes.Length; i++)
            {
                long boxPart = i + 1;
                Box box = boxes[i];

                if (box is not null)
                {
                    for (int lensId = 0; lensId < box.Lenses.Count; lensId++)
                    {
                        long lensIdPart = lensId + 1;

                        long score = boxPart * lensIdPart * box.Lenses[lensId];
                        ret += score;
                    }
                }
            }

            Console.WriteLine(ret);
        }

        private class Box
        {
            public readonly List<int> Lenses = new();
            public readonly List<string> Labels = new();
        }

        private static int Day15Hash(ReadOnlySpan<char> text)
        {
            byte hash = 0;

            foreach (char c in text)
            {
                unchecked
                {
                    byte ascii = (byte)c;
                    hash += ascii;
                    hash *= 17;
                }
            }

            return hash;
        }
    }
}
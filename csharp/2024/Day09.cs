using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using AdventOfCode.Util;

namespace AdventOfCode2024
{
    internal class Day09
    {
        internal static void Problem1()
        {
            long ret = 0;
            int[] blocks = null;

            foreach (string s in Data.Enumerate())
            {
                int sum = s.Select(c => c - '0').Sum();
                blocks = new int[sum];
                blocks.AsSpan().Fill(-1);

                int max = 0;
                int min = int.MaxValue;
                bool data = true;
                int id = 0;

                foreach (char c in s)
                {
                    int len = c - '0';

                    if (data)
                    {
                        for (int i = 0; i < len; i++)
                        {
                            blocks[max] = id;
                            max++;
                        }

                        id++;
                    }
                    else
                    {
                        min = int.Min(min, max);
                        max += len;
                    }

                    data = !data;
                }

                for (int i = max - 1; i >= min; i--)
                {
                    if (blocks[i] != -1)
                    {
                        while (blocks[min] != -1)
                        {
                            min++;
                        }

                        if (min < i)
                        {
                            blocks[min] = blocks[i];
                            blocks[i] = -1;
                            max--;
                        }
                    }
                }

                for (int i = 0; i < max; i++)
                {
                    int fileId = blocks[i];

                    if (fileId != -1)
                    {
                        ret += i * fileId;
                    }
                }
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            List<(int Id, int Len)> blocks = new();

            foreach (string s in Data.Enumerate())
            {
                int max = 0;
                int min = int.MaxValue;
                bool data = true;
                int id = 0;

                foreach (char c in s)
                {
                    int len = c - '0';

                    if (data)
                    {
                        blocks.Add((id, len));
                        id++;
                    }
                    else
                    {
                        blocks.Add((-1, len));
                    }

                    data = !data;
                }

                for (int i = blocks.Count - 1; i > 0; i--)
                {
                    var iBlock = blocks[i];

                    if (iBlock.Id == -1)
                    {
                        continue;
                    }

                    for (int j = 1; j < i; j++)
                    {
                        var jBlock = blocks[j];

                        if (jBlock.Id == -1)
                        {
                            if (jBlock.Len >= iBlock.Len)
                            {
                                Utils.TraceForSample($"Moving file {iBlock.Id} to position {j}");

                                blocks[i] = (-1, iBlock.Len);
                            
                                if (jBlock.Len > iBlock.Len)
                                {
                                    blocks.Insert(j + 1, (-1, jBlock.Len - iBlock.Len));
                                    i++;
                                }

                                blocks[j] = iBlock;
                                DrawMap(blocks);
                                break;
                            }
                            else
                            {
                                Utils.TraceForSample($"Cannot move {iBlock.Id} ({iBlock.Len})  to position {j} ({jBlock.Len})");
                            }
                        }
                    }
                }

#if SAMPLE
                DrawMap(blocks);
#endif
                [Conditional("SAMPLE")]
                static void DrawMap(List<(int Id, int Len)> blocks)
                {
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        var block = blocks[i];
                        char blockId = (char)(block.Id == -1 ? '.' : (block.Id + '0'));

                        for (int j = 0; j < block.Len; j++)
                        {
                            Console.Write(blockId);
                        }

                    }

                    Console.WriteLine();
                }

                int pos = 0;

                for (int i = 0; i < blocks.Count; i++)
                {
                    var block = blocks[i];

                    if (block.Id == -1)
                    {
                        pos += block.Len;
                    }
                    else
                    {
                        for (int j = 0; j < block.Len; j++)
                        {
                            ret += pos * block.Id;
                            pos++;
                        }
                    }
                }
            }

            Console.WriteLine(ret);
        }
    }
}
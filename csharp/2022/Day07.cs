using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day07
    {
        private static Path LoadPath()
        {
            Path root = new Path("/", null);
            Path cwd = root;

            foreach (string line in Data.Enumerate())
            {
                if (line.Equals($"$ cd /"))
                {
                    cwd = root;
                }
                else if (line.Equals($"$ cd .."))
                {
                    cwd = cwd.Parent ?? root;
                }
                else if (line.StartsWith($"$ cd "))
                {
                    string sub = line.Substring(5);

                    ref Path match = ref CollectionsMarshal.GetValueRefOrAddDefault(cwd.Subdirs, sub, out _);

                    if (match is null)
                    {
                        match = new Path(sub, cwd);
                    }

                    cwd = match;
                }
                else if (line.Equals($"$ ls"))
                {
                }
                else if (line.StartsWith("dir "))
                {
                    string sub = line.Substring(4);
                    ref Path match = ref CollectionsMarshal.GetValueRefOrAddDefault(cwd.Subdirs, sub, out _);

                    if (match is null)
                    {
                        match = new Path(sub, cwd);
                    }
                }
                else
                {
                    int spaceIdx = line.IndexOf(' ');
                    int size = int.Parse(line.AsSpan(0, spaceIdx));
                    string name = line.Substring(spaceIdx + 1);

                    cwd.Files.Add((name, size));
                }
            }

            return root;
        }


        internal static void Problem1()
        {
            Path root = LoadPath();

            static long Sum(Path path)
            {
                long sum = 0;

                foreach (Path p in path.Subdirs.Values)
                {
                    sum += Sum(p);
                }

                int local = path.GetFullSize();

                if (local <= 100000)
                {
                    sum += local;
                }

                return sum;
            }

            Console.WriteLine(Sum(root));
        }

        internal static void Problem2()
        {
            int totalSpace = 70000000;
            Path root = LoadPath();
            int minFree = 30000000;
            int curFree = totalSpace - root.GetFullSize();
            int mustFree = minFree - curFree;
            Path smallest = null;

            static void FindSmallest(Path candidate, ref Path smallest, int target)
            {
                int cur = candidate.GetFullSize();

                if (cur >= target)
                {
                    if (smallest == null || cur < smallest.GetFullSize())
                    {
                        smallest = candidate;
                    }

                    foreach (Path p in candidate.Subdirs.Values)
                    {
                        FindSmallest(p, ref smallest, target);
                    }
                }
            }

            FindSmallest(root, ref smallest, mustFree);
            Console.WriteLine(smallest.GetFullSize());
        }

        private class Path
        {
            public Path Parent;
            public string RelativePath;
            public List<(string, int)> Files;
            public Dictionary<string, Path> Subdirs;

            public Path(string relativePath, Path parent)
            {
                Parent = parent;
                RelativePath = relativePath;
                Files = new List<(string, int)>();
                Subdirs = new();
            }

            public int GetLocalSize()
            {
                return Files.Sum(f => f.Item2);
            }

            public int GetFullSize()
            {
                return Subdirs.Values.Sum(d => d.GetFullSize()) + GetLocalSize();
            }
        }
    }
}
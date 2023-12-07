using System;
using System.Collections.Generic;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day18
    {
        private const int GridSize = 50;

        private static byte[,,] Load()
        {
            byte[,,] grid = new byte[GridSize,GridSize,GridSize];

            foreach (string s in Data.Enumerate())
            {
                ReadOnlySpan<char> line = s;
                int commaIdx = line.IndexOf(',');
                int x = int.Parse(line.Slice(0, commaIdx));
                line = line.Slice(commaIdx + 1);
                commaIdx = line.IndexOf(',');
                int y = int.Parse(line.Slice(0, commaIdx));
                line = line.Slice(commaIdx + 1);
                int z = int.Parse(line);

                grid[x + 1, y + 1, z + 1] = 1;
            }

            return grid;
        }

        internal static void Problem1()
        {
            byte[,,] grid = Load();
            int faces = CountFaces(grid);

            Console.WriteLine(faces);
        }

        private static int CountFaces(byte[,,] grid)
        {
            int faces = 0;

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    byte inside = 0;

                    for (int z = 0; z < GridSize; z++)
                    {
                        if (grid[x, y, z] != inside)
                        {
                            faces++;
                            inside = grid[x, y, z];
                        }
                    }
                }
            }

            for (int x = 0; x < GridSize; x++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    byte inside = 0;

                    for (int y = 0; y < GridSize; y++)
                    {
                        if (grid[x, y, z] != inside)
                        {
                            faces++;
                            inside = grid[x, y, z];
                        }
                    }
                }
            }

            for (int y = 0; y < GridSize; y++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    byte inside = 0;

                    for (int x = 0; x < GridSize; x++)
                    {
                        if (grid[x, y, z] != inside)
                        {
                            faces++;
                            inside = grid[x, y, z];
                        }
                    }
                }
            }

            return faces;
        }

        internal static void Problem2()
        {
            byte[,,] grid = Load();
            byte[,,] outside = new byte[GridSize, GridSize, GridSize];

            Point3 zero = new Point3(0, 0, 0);

            Pathing.BreadthFirstSearch(
                grid,
                zero,
                node =>
                {
                    outside[node.X, node.Y, node.Z] = 1;
                    return false;
                },
                Children);

            static IEnumerable<Point3> Children(Point3 from, byte[,,] world)
            {
                if (from.X > 0 && world[from.X - 1, from.Y, from.Z] == 0)
                {
                    yield return new Point3(from.X - 1, from.Y, from.Z);
                }

                if (from.X < GridSize - 1 && world[from.X + 1, from.Y, from.Z] == 0)
                {
                    yield return new Point3(from.X + 1, from.Y, from.Z);
                }

                if (from.Y > 0 && world[from.X, from.Y - 1, from.Z] == 0)
                {
                    yield return new Point3(from.X, from.Y - 1, from.Z);
                }

                if (from.Y < GridSize - 1 && world[from.X, from.Y + 1, from.Z] == 0)
                {
                    yield return new Point3(from.X, from.Y + 1, from.Z);
                }

                if (from.Z > 0 && world[from.X, from.Y, from.Z - 1] == 0)
                {
                    yield return new Point3(from.X, from.Y, from.Z - 1);
                }

                if (from.Z < GridSize - 1 && world[from.X, from.Y, from.Z + 1] == 0)
                {
                    yield return new Point3(from.X, from.Y, from.Z + 1);
                }
            }

            for (int x = GridSize - 1; x >= 0; x--)
            {
                for (int y = GridSize - 1; y >= 0; y--)
                {
                    for (int z = GridSize - 1; z >= 0; z--)
                    {
                        if (grid[x, y, z] == 0 && outside[x, y, z] != 1)
                        {
                            grid[x, y, z] = 1;
                        }
                    }
                }
            }

            int faces = CountFaces(grid);
            Console.WriteLine(faces);
        }
    }
}
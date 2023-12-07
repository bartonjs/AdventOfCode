using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day13
    {
        internal static void Problem1()
        {
            IEnumerable<string> dataEnumerable = Data.Enumerate();
            IEnumerator<string> dataEnumerator = dataEnumerable.GetEnumerator();

            Origami o = new Origami();
            o.Load(dataEnumerator);

            Console.WriteLine(o.CountPoints());
            dataEnumerator.MoveNext();

            o.ProcessFold(dataEnumerator.Current);
            Console.WriteLine(o.CountPoints());
        }

        internal static void Problem2()
        {
            IEnumerable<string> dataEnumerable = Data.Enumerate();
            IEnumerator<string> dataEnumerator = dataEnumerable.GetEnumerator();

            Origami o = new Origami();
            o.Load(dataEnumerator);

            while (dataEnumerator.MoveNext())
            {
                o.ProcessFold(dataEnumerator.Current);
            }

            o.Print();
            Console.WriteLine();
            o.PrintTranspose();
        }

        private class Origami
        {
            private bool[] _points;
            private int _cols;
            private int _rows;

            public void Load(IEnumerator<string> enumerator)
            {
                List<(int, int)> coords = new List<(int, int)>();

                while (enumerator.MoveNext())
                {
                    string val = enumerator.Current;

                    if (string.IsNullOrEmpty(val))
                    {
                        break;
                    }

                    int comma = val.IndexOf(',');

                    coords.Add((int.Parse(val.AsSpan(0, comma)), int.Parse(val.AsSpan(comma + 1))));
                }

                int cols = coords.Max(p => p.Item2) + 1;
                int rows = coords.Max(p => p.Item1) + 1;

                int total = rows * cols;
                bool[] points = new bool[total];
                
                Console.WriteLine($"Cols={cols}, Rows={rows}, total points read={coords.Count}");

                foreach ((int row, int col) in coords)
                {
                    points[row * cols + col] = true;
                }

                _points = points;
                _cols = cols;
                _rows = rows;
            }

            public int CountPoints()
            {
                return _points.Sum(x => x ? 1 : 0);
            }

            public void ProcessFold(string instruction)
            {
                int equals = instruction.IndexOf('=');
                char dir = instruction[equals - 1];
                int line = int.Parse(instruction.AsSpan(equals + 1));

                if (dir == 'x')
                {
                    int lostRows = _rows - line;
                    int newRows = _rows - lostRows;
                    int newTotal = newRows * _cols;

                    bool[] newPoints = _points.AsSpan(0, newTotal).ToArray();

                    int placeAt = line - 1;

                    for (int row = line + 1; row < _rows; row++)
                    {
                        for (int col = 0; col < _cols; col++)
                        {
                            if (_points[row * _cols + col])
                            {
                                newPoints[placeAt * _cols + col] = true;
                            }
                        }

                        placeAt--;
                    }

                    _points = newPoints;
                    _rows = newRows;
                }
                else if (dir == 'y')
                {
                    int lostCols = _cols - line;
                    int newCols = _cols - lostCols;
                    int newTotal = _rows * newCols;

                    bool[] newPoints = new bool[newTotal];

                    for (int col = 0; col < newCols; col++)
                    {
                        for (int row = 0; row < _rows; row++)
                        {
                            if (_points[row * _cols + col])
                            {
                                newPoints[row * newCols + col] = true;
                            }
                        }
                    }

                    int placeAt = line - 1;

                    for (int col = line + 1; col < _cols; col++)
                    {
                        for (int row = 0; row < _rows; row++)
                        {
                            if (_points[row * _cols + col])
                            {
                                newPoints[row * newCols + placeAt] = true;
                            }
                        }

                        placeAt--;
                    }

                    _points = newPoints;
                    _cols = newCols;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            public void Print()
            {
                for (int row = 0; row < _rows; row++)
                {
                    for (int col = 0; col < _cols; col++)
                    {
                        if (_points[row * _cols + col])
                        {
                            Console.Write('#');
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }

                    Console.WriteLine();
                }
            }

            public void PrintTranspose()
            {
                for (int col = 0; col < _cols; col++)
                {
                    for (int row = 0; row < _rows; row++)
                    {
                        if (_points[row * _cols + col])
                        {
                            Console.Write('#');
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
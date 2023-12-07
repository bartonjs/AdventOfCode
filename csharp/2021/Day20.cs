using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day20
    {
        internal static void Problem1()
        {
            Image image = Image.Load();
            image.Update();
            image.Update();
            image.Print();
            Console.WriteLine(image.CountSetPixels());
        }

        internal static void Problem2()
        {
            Image image = Image.Load();

            for (int i = 0; i < 25; i++)
            {
                image.Update();
                image.Update();
                Console.WriteLine(image.CountSetPixels());
            }
        }

        private class Image
        {
            private readonly List<bool> _algorithm;
            private List<List<bool>> _data;
            private bool _infinityValue;

            private Image(List<bool> algo, List<List<bool>> data)
            {
                _algorithm = algo;
                _data = data;
            }

            public int Rows => _data.Count + 2;
            public int Cols => _data[0].Count + 2;

            private int GetScore(int row, int col)
            {
                int score = 0;

                for (int rowShift = 2; rowShift >= 0; rowShift--)
                {
                    List<bool> cur = _data.SafeIndex(row - rowShift);

                    if (cur is not null)
                    {
                        for (int colShift = 2; colShift >= 0; colShift--)
                        {
                            score <<= 1;

                            if (cur.SafeIndex(col - colShift, _infinityValue))
                            {
                                score |= 1;
                            }
                        }
                    }
                    else
                    {
                        score <<= 3;

                        if (_infinityValue)
                        {
                            score |= 0b111;
                        }
                    }
                }

                return score;
            }

            public void Update()
            {
                List<List<bool>> newData = new List<List<bool>>(_data.Count + 2);

                for (int row = 0; row < Rows; row++)
                {
                    List<bool> newRow = new List<bool>(Cols);
                    newData.Add(newRow);

                    for (int col = 0; col < Cols; col++)
                    {
                        int score = GetScore(row, col);
                        newRow.Add(_algorithm[score]);
                    }
                }

                _data = newData;
                _infinityValue = _algorithm[GetScore(-1000, -1000)];
            }

            public int CountSetPixels()
            {
                return _data.SelectMany(x => x).Count(x => x);
            }

            public void Print()
            {
                foreach (List<bool> row in _data)
                {
                    foreach (bool val in row)
                    {
                        Console.Write(val ? '#' : '.');
                    }

                    Console.WriteLine();
                }
            }

            public static Image Load()
            {
                IEnumerator<string> ator = Data.Enumerate().GetEnumerator();
                ator.MoveNext();
                List<bool> algo = ator.Current.Select(x => x == '#').ToList();
                ator.MoveNext();

                Debug.Assert(string.IsNullOrEmpty(ator.Current));
                List<List<bool>> data = new List<List<bool>>();

                while (ator.MoveNext())
                {
                    string line = ator.Current;
                    data.Add(line.Select(x => x == '#').ToList());
                }

                return new Image(algo, data);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day4
    {
        internal static void Problem1()
        {
            (List<int> numbers, List<int[][]> boards) = Load();

            foreach (int call in numbers)
            {
                MarkBoards(boards, call);

                for (int game = 0; game < boards.Count; game++)
                {
                    if (CheckWin(boards[game], game))
                    {
                        CalculateScore(boards[game], call);
                        return;
                    }
                }
            }
        }

        internal static void Problem2()
        {
            (List<int> numbers, List<int[][]> boards) = Load();
            HashSet<int> winnable = new HashSet<int>(Enumerable.Range(0, boards.Count));

            foreach (int call in numbers)
            {
                MarkBoards(boards, call);

                for (int game = 0; game < boards.Count; game++)
                {
                    if (!winnable.Contains(game))
                    {
                        continue;
                    }

                    if (CheckWin(boards[game], game))
                    {
                        winnable.Remove(game);

                        if (winnable.Count == 0)
                        {
                            CalculateScore(boards[game], call);
                            return;
                        }
                    }
                }
            }
        }

        private static (List<int> Numbers, List<int[][]> Boards) Load()
        {
            List<int> numbers = null;
            List<int[][]> boards = new List<int[][]>();
            int[][] curBoard = null;
            int loadRow = 0;

            foreach (string line in Data.Enumerate())
            {
                if (numbers is null)
                {
                    numbers = new List<int>(line.Split(',').Select(int.Parse));
                    continue;
                }

                if (string.IsNullOrEmpty(line))
                {
                    if (curBoard is not null)
                    {
                        boards.Add(curBoard);
                        curBoard = null;
                    }

                    continue;
                }

                if (curBoard is null)
                {
                    curBoard = new int[5][];
                    loadRow = 0;
                }

                curBoard[loadRow] = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                Debug.Assert(curBoard[loadRow].Length == 5);
                loadRow++;
            }

            return (numbers, boards);
        }

        private static void MarkBoards(List<int[][]> boards, int call)
        {
            for (int game = 0; game < boards.Count; game++)
            {
                int[][] board = boards[game];

                for (int row = 0; row < board.Length; row++)
                {
                    for (int col = 0; col < board[row].Length; col++)
                    {
                        if (board[row][col] == call)
                        {
                            board[row][col] *= -1;
                        }
                    }
                }
            }
        }

        private static void CalculateScore(int[][] board, int call)
        {
            long sum = 0;

            for (int row = 0; row < board.Length; row++)
            {
                for (int col = 0; col < board.Length; col++)
                {
                    if (board[row][col] > 0)
                    {
                        sum += board[row][col];
                    }
                }
            }

            Console.WriteLine($"Winner won with call {call}");
            Console.WriteLine($"Answer is {call * sum}");
        }

        private static bool CheckWin(int[][] board, int game)
        {
            for (int row = 0; row < board.Length; row++)
            {
                if (board[row].All(v => v < 0))
                {
                    Console.WriteLine($"Winner in game {game} by row {row}");
                    return true;
                }
            }

            for (int col = 0; col < board.Length; col++)
            {
                if (board.All(row => row[col] < 0))
                {
                    Console.WriteLine($"Winner in game {game} by col {col}");
                    return true;
                }
            }

            return false;
        }
    }
}

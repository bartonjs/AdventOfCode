using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Microsoft.VisualBasic.CompilerServices;

namespace AdventOfCode2021.Solutions
{
    public class Day21
    {
        internal static void Problem1()
        {
            //DiracDice dice = new DiracDice(4, 8);
            DiracDice dice = new DiracDice(5, 9);
            long lowScore = dice.PlayDeterministic();
            lowScore *= dice.RollCount;
            Console.WriteLine(lowScore);
        }

        internal static void Problem2()
        {
            //int p1Start = 4;
            //int p2Start = 8;
            int p1Start = 5;
            int p2Start = 9;

            GameState state = new GameState(p1Start, p2Start, 0, 0, false);
            Twople wins = Run(state);
            
            Console.WriteLine($"p1Wins: {wins.P1Wins}");
            Console.WriteLine($"p2Wins: {wins.P2Wins}");
        }

        private static Twople Zero = new Twople(0, 0);
        private static Dictionary<GameState, Twople> s_cache = new();

        private static Twople Run(GameState state)
        {
            if (s_cache.TryGetValue(state, out var score))
            {
                return score;
            }

            score = RunWithRoll(state, 3) * 1;
            score += RunWithRoll(state, 4) * 3;
            score += RunWithRoll(state, 5) * 6;
            score += RunWithRoll(state, 6) * 7;
            score += RunWithRoll(state, 7) * 6;
            score += RunWithRoll(state, 8) * 3;
            score += RunWithRoll(state, 9) * 1;

            s_cache[state] = score;
            return score;
        }

        private static Twople RunWithRoll(GameState state, int rollValue)
        {
            int pos, score;

            if (state.P2Turn)
            {
                pos = state.P2Pos;
                score = state.P2Score;
            }
            else
            {
                pos = state.P1Pos;
                score = state.P1Score;
            }

            pos += rollValue;

            while (pos > 10)
            {
                pos -= 10;
            }

            score += pos;

            if (score >= 21)
            {
                if (state.P2Turn)
                {
                    return new Twople(0, 1);
                }
                else
                {
                    return new Twople(1, 0);
                }
            }

            GameState newState;

            if (state.P2Turn)
            {
                newState = state with { P2Pos = pos, P2Score = score, P2Turn = false };
            }
            else
            {
                newState = state with { P1Pos = pos, P1Score = score, P2Turn = true };
            }

            return Run(newState);
        }

        private record Twople(long P1Wins, long P2Wins)
        {
            public static Twople operator +(Twople a, Twople b)
            {
                return new Twople(a.P1Wins + b.P1Wins, a.P2Wins + b.P2Wins);
            }

            public static Twople operator *(Twople a, int scale)
            {
                return new Twople(a.P1Wins * scale, a.P2Wins * scale);
            }
        }

        private record GameState(int P1Pos, int P2Pos, int P1Score, int P2Score, bool P2Turn)
        {
        }

        private class DiracDice
        {
            private int _p1Pos;
            private int _p2Pos;
            private int _p1Score;
            private int _p2Score;
            public int RollCount { get; private set; }
            private bool _p2Turn;

            public DiracDice(int p1Pos, int p2Pos)
            {
                _p1Pos = p1Pos;
                _p2Pos = p2Pos;
            }

            private int RollDeterministic()
            {
                int roll = RollCount % 100;
                RollCount++;
                return roll + 1;
            }

            public int PlayDeterministic()
            {
                while (_p1Score < 1000 && _p2Score < 1000)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int roll = RollDeterministic();

                        if (_p2Turn)
                        {
                            MoveAndScore(ref _p2Pos, roll);
                        }
                        else
                        {
                            MoveAndScore(ref _p1Pos, roll);
                        }
                    }

                    if (_p2Turn)
                    {
                        _p2Score += _p2Pos;
                    }
                    else
                    {
                        _p1Score += _p1Pos;
                    }

                    _p2Turn = !_p2Turn;
                }

                return Math.Min(_p1Score, _p2Score);
            }

            private static int MoveAndScore(ref int playerPos, int roll)
            {
                playerPos += roll;

                while (playerPos > 10)
                {
                    playerPos -= 10;
                }

                return playerPos;
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace AdventOfCode2021.Solutions
{
    public class Day23
    {
        internal static void Problem1()
        {
            State state = new State();
            State solved = state.Solve();

            solved.PrintMoveLog();
            Console.WriteLine(solved.Score);
        }

        internal static void Problem2()
        {
            State state = new State(true);
            State solved = state.Solve();

            solved.PrintMoveLog();
            Console.WriteLine(solved.Score);
        }

        private class State
        {
            private static readonly int[] s_scale = { 1, 10, 100, 1000 };

            private readonly char[] _hallway = new char[11];
            private readonly char[,] _rooms;
            private readonly List<(char, int, int, int, int, int)> _moveLog;
            private readonly bool _partTwo;

            public int Score { get; private set; }

            public State(bool partTwo = false)
            {
                _hallway[2] = _hallway[4] = _hallway[6] = _hallway[8] = 'X';
                _partTwo = partTwo;

                if (partTwo)
                {
                    //_rooms = new char[4, 4]
                    //{
                    //    { 'B', 'D', 'D', 'A' },
                    //    { 'C', 'C', 'B', 'D' },
                    //    { 'B', 'B', 'A', 'C' },
                    //    { 'D', 'A', 'C', 'A' },
                    //};

                    _rooms = new char[4, 4]
                    {
                        { 'B', 'D', 'D', 'D' },
                        { 'B', 'C', 'B', 'C' },
                        { 'D', 'B', 'A', 'A' },
                        { 'A', 'A', 'C', 'C' },
                    };
                }
                else
                {
                    //_rooms = new char[4, 2]
                    //{
                    //    { 'B', 'A' },
                    //    { 'C', 'D' },
                    //    { 'B', 'C' },
                    //    { 'D', 'A' },
                    //};

                    _rooms = new char[4, 2]
                    {
                        { 'B', 'D' },
                        { 'B', 'C' },
                        { 'D', 'A' },
                        { 'A', 'C' },
                    };
                }

                _moveLog = new List<(char, int, int, int, int, int)>();
            }

            private State(State other)
            {
                _hallway = (char[])other._hallway.Clone();
                _rooms = (char[,])other._rooms.Clone();
                _moveLog = new List<(char, int, int, int, int, int)>(other._moveLog);
                _partTwo = other._partTwo;
                Score = other.Score;
            }

            public State Solve()
            {
                return Min(SolveMore(true));
            }

            private State Move(int fromRoom, int fromRoomSlot, int toRoom, int toRoomSlot)
            {
                State other = new State(this);
                char cur;

                if (fromRoom >= 0)
                {
                    other._rooms[fromRoom, fromRoomSlot] = (char)0;
                    cur = _rooms[fromRoom, fromRoomSlot];
                }
                else
                {
                    other._hallway[fromRoomSlot] = (char)0;
                    cur = _hallway[fromRoomSlot];
                }

                if (toRoom >= 0)
                {
                    other._rooms[toRoom, toRoomSlot] = cur;
                }
                else
                {
                    other._hallway[toRoomSlot] = cur;
                }

                int moveScore = ScoreMove(cur, fromRoom, fromRoomSlot, toRoom, toRoomSlot);
                other.Score += moveScore;
                other._moveLog.Add((cur, fromRoom, fromRoomSlot, toRoom, toRoomSlot, moveScore));
                return other;
            }

            private IEnumerable<State> SolveMore(bool print = false)
            {
                if (_rooms[0, 0] == 'A' && _rooms[0, 1] == 'A' &&
                    _rooms[1, 0] == 'B' && _rooms[1, 1] == 'B' &&
                    _rooms[2, 0] == 'C' && _rooms[2, 1] == 'C' &&
                    _rooms[3, 0] == 'D' && _rooms[3, 1] == 'D')
                {
                    if (_partTwo)
                    {
                        if (_rooms[0, 2] == 'A' && _rooms[0, 3] == 'A' &&
                            _rooms[1, 2] == 'B' && _rooms[1, 3] == 'B' &&
                            _rooms[2, 2] == 'C' && _rooms[2, 3] == 'C' &&
                            _rooms[3, 2] == 'D' && _rooms[3, 3] == 'D')
                        {
                            yield return this;
                            yield break;
                        }
                    }
                    else
                    {
                        yield return this;
                        yield break;
                    }
                }

                for (int i = 0; i < _hallway.Length; i++)
                {
                    char cur = _hallway[i];

                    if (cur >= 'A' && cur <= 'D')
                    {
                        int roomNo = cur - 'A';

                        if (ClearHallwayToRoom(i, roomNo))
                        {
                            int freeSlotNum = FreeSlotNum(roomNo);

                            if (freeSlotNum >= 0)
                            {
                                State moved = Move(-1, i, roomNo, freeSlotNum);
                                yield return Min(moved.SolveMore());
                            }
                        }
                    }
                }

                for (int roomNo = 0; roomNo < 4; roomNo++)
                {
                    if (print)
                    {
                        Console.WriteLine($"Starting with room {roomNo}");
                    }

                    int fromSlot = FindMoveFromSlot(roomNo);

                    if (fromSlot >= 0)
                    {
                        yield return Min(MoveFromRoom(roomNo, fromSlot));
                    }
                }
            }

            private IEnumerable<State> MoveFromRoom(int roomNo, int roomSlot)
            {
                int targetRoom = (_rooms[roomNo, roomSlot] - 'A');
                bool movedDirect = false;

                if (ClearRoomToRoom(roomNo, targetRoom))
                {
                    int destSlot = FreeSlotNum(targetRoom);

                    if (destSlot >= 0)
                    {
                        State moved = Move(roomNo, roomSlot, targetRoom, destSlot);
                        movedDirect = true;
                        yield return Min(moved.SolveMore());
                    }
                }

                if (!movedDirect)
                {
                    for (int i = 0; i < _hallway.Length; i++)
                    {
                        if (_hallway[i] == 0 && ClearHallwayToRoom(i, roomNo))
                        {
                            State moved = Move(roomNo, roomSlot, -1, i);
                            yield return Min(moved.SolveMore());
                        }
                    }
                }
            }

            private int FindMoveFromSlot(int roomNo)
            {
                char expected = (char)(roomNo + 'A');

                if (_partTwo)
                {
                    if (_rooms[roomNo, 3] != expected)
                    {
                        if (_rooms[roomNo, 0] != 0)
                        {
                            return 0;
                        }

                        if (_rooms[roomNo, 1] != 0)
                        {
                            return 1;
                        }

                        if (_rooms[roomNo, 2] != 0)
                        {
                            return 2;
                        }

                        if (_rooms[roomNo, 3] != 0)
                        {
                            return 3;
                        }

                        return -1;
                    }

                    if (_rooms[roomNo, 2] != expected)
                    {
                        if (_rooms[roomNo, 0] != 0)
                        {
                            return 0;
                        }

                        if (_rooms[roomNo, 1] != 0)
                        {
                            return 1;
                        }

                        if (_rooms[roomNo, 2] != 0)
                        {
                            return 2;
                        }

                        return -1;
                    }
                }

                if (_rooms[roomNo, 1] != expected)
                {
                    if (_rooms[roomNo, 0] != 0)
                    {
                        return 0;
                    }

                    if (_rooms[roomNo, 1] != 0)
                    {
                        return 1;
                    }

                    return -1;
                }

                if (_rooms[roomNo, 0] != expected)
                {
                    if (_rooms[roomNo, 0] != 0)
                    {
                        return 0;
                    }
                }

                return -1;
            }

            private int FreeSlotNum(int roomNo)
            {
                char expected = (char)(roomNo + 'A');

                if (_partTwo)
                {
                    return
                        CheckIndex(this, roomNo, expected, 3) ??
                        CheckIndex(this, roomNo, expected, 2) ??
                        CheckIndex(this, roomNo, expected, 1) ??
                        CheckIndex(this, roomNo, expected, 0) ??
                        -1;
                }
                else
                {
                    return
                        CheckIndex(this, roomNo, expected, 1) ??
                        CheckIndex(this, roomNo, expected, 0) ??
                        -1;
                }

                static int? CheckIndex(State that, int roomNo, char expected, int slotNum)
                {
                    char cur = that._rooms[roomNo, slotNum];

                    if (cur == 0)
                    {
                        return slotNum;
                    }

                    if (cur != expected)
                    {
                        return -1;
                    }

                    return null;
                }
            }

            private bool ClearHallwayToRoom(int hallwaySlot, int roomNo)
            {
                int roomPos = 2 * (roomNo + 1);
                int min = Math.Min(roomPos, hallwaySlot);
                int max = Math.Max(roomPos, hallwaySlot);

                for (int i = min + 1; i < max; i++)
                {
                    int cur = _hallway[i];

                    if (cur != 0 && cur != 'X')
                    {
                        return false;
                    }
                }

                return true;
            }

            private bool ClearRoomToRoom(int fromRoom, int toRoom)
            {
                return ClearHallwayToRoom(2 * (fromRoom + 1), toRoom);
            }

            private int ScoreMove(char moving, int fromRoom, int fromSlot, int toRoom, int toSlot)
            {
                if (fromRoom == -1 && toRoom == -1)
                {
                    throw new InvalidOperationException();
                }

                if (fromRoom == -1)
                {
                    return HallwayToRoom(moving, fromSlot, toRoom, toSlot);
                }

                if (toRoom == -1)
                {
                    return HallwayToRoom(moving, toSlot, fromRoom, fromSlot);
                }

                return RoomToRoom(moving, fromRoom, fromSlot, toRoom, toSlot);
            }

            private int RoomToRoom(char moving, int fromRoom, int fromSlot, int toRoom, int toSlot)
            {
                int fromLoc = 2 * (fromRoom + 1);
                int part1 = HallwayToRoom(moving, fromLoc, fromRoom, fromSlot);
                int part2 = HallwayToRoom(moving, fromLoc, toRoom, toSlot);
                return part1 + part2;
            }

            private int HallwayToRoom(char moving, int hallwaySlot, int roomNo, int roomSlot)
            {
                int roomLoc = 2 * (roomNo + 1);
                int hallDistance = Math.Abs(roomLoc - hallwaySlot);
                int roomDistance = roomSlot + 1;

                int baseScore = hallDistance + roomDistance;
                return baseScore * s_scale[moving - 'A'];
            }

            private State Min(IEnumerable<State> states)
            {
                int score = int.MaxValue;
                State min = null;

                foreach (State state in states)
                {
                    if (state is null)
                    {
                        continue;
                    }

                    if (state.Score < score)
                    {
                        min = state;
                        score = state.Score;
                    }
                }

                return min;
            }

            public void PrintMoveLog()
            {
                foreach (var entry in _moveLog)
                {
                    Console.WriteLine(entry);
                }
            }
        }
    }
}

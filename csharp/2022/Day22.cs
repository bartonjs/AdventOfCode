using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class Day22
    {
        private static (MapSpot first, string secondPart) Load()
        {
            MapSpot[] topLinks;
            MapSpot[] bottomLinks;
            MapSpot first = null;
            int row = 1;
            bool firstPart = true;
            string secondPart = null;

            string prep = Data.Enumerate().First();
            topLinks = new MapSpot[prep.Length + 1];
            bottomLinks = new MapSpot[prep.Length + 1];

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    firstPart = false;
                    continue;
                }

                if (!firstPart)
                {
                    secondPart = s;
                    break;
                }

                int col = 0;
                MapSpot left = null;
                MapSpot right = null;
                MapSpot prev = null;

                foreach (char c in s)
                {
                    col++;

                    if (c == ' ')
                    {
                        continue;
                    }

                    MapSpot spot = new MapSpot()
                    {
                        Col = col,
                        Row = row,
                    };

                    first ??= spot;

                    if (left is null)
                    {
                        left = spot;
                    }
                    else
                    {
                        prev.Right = spot;
                        spot.Left = prev;
                    }

                    prev = spot;
                    right = spot;

                    if (topLinks[col] is null)
                    {
                        topLinks[col] = spot;
                    }
                    else
                    {
                        spot.Up = bottomLinks[col];
                        bottomLinks[col].Down = spot;
                    }

                    bottomLinks[col] = spot;
                    spot.Wall = (c == '#');
                }

                right.Right = left;
                left.Left = right;
                row++;
            }

            for (int i = 1; i < topLinks.Length; i++)
            {
                topLinks[i].Up = bottomLinks[i];
                bottomLinks[i].Down = topLinks[i];
            }

            return (first, secondPart);
        }

        [DebuggerDisplay("R: {Row} C:{Col}")]
        private class MapSpot
        {
            public MapSpot Up;
            public MapSpot Down;
            public MapSpot Left;
            public MapSpot Right;
            public bool Wall;
            public int Row;
            public int Col;
            public int UpRotation;
            public int DownRotation;
            public int LeftRotation;
            public int RightRotation;

            internal bool HasPeer(MapSpot other)
            {
                return Up == other || Down == other || Left == other || Right == other;
            }
        }

        private static int ReadNumber(ref ReadOnlySpan<char> text)
        {
            int idx = text.IndexOfAnyExcept("0123456789");
            int ret;

            if (idx < 0)
            {
                ret = int.Parse(text);
                text = default;
            }
            else
            {
                if (idx == 0)
                {
                }

                ret = int.Parse(text.Slice(0, idx));
                text = text.Slice(idx);
            }

            return ret;
        }

        internal static void Problem1()
        {
            (MapSpot pos, string directionsString) = Load();
            ReadOnlySpan<char> directions = directionsString;
            int facing = 0;
            bool move = true;

            while (!directions.IsEmpty)
            {
                if (move)
                {
                    int count = ReadNumber(ref directions);

                    switch (facing)
                    {
                        case 0:
                            for (int i = 0; i < count; i++)
                            {
                                MapSpot next = pos.Right;

                                if (next.Wall)
                                {
                                    break;
                                }

                                pos = next;
                            }

                            break;

                        case 1:
                            for (int i = 0; i < count; i++)
                            {
                                MapSpot next = pos.Down;

                                if (next.Wall)
                                {
                                    break;
                                }

                                pos = next;
                            }

                            break;

                        case 2:
                            for (int i = 0; i < count; i++)
                            {
                                MapSpot next = pos.Left;

                                if (next.Wall)
                                {
                                    break;
                                }

                                pos = next;
                            }

                            break;

                        case 3:
                            for (int i = 0; i < count; i++)
                            {
                                MapSpot next = pos.Up;

                                if (next.Wall)
                                {
                                    break;
                                }

                                pos = next;
                            }

                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    if (directions[0] == 'R')
                    {
                        facing = (facing + 1) % 4;
                    }
                    else
                    {
                        Debug.Assert(directions[0] == 'L');

                        facing--;

                        if (facing < 0)
                        {
                            facing = 3;
                        }
                    }

                    directions = directions.Slice(1);
                }

                move = !move;
            }

            Console.WriteLine($"Final Position: R:{pos.Row}, C:{pos.Col}, F:{facing}");
            Console.WriteLine(1000 * pos.Row + 4 * pos.Col + facing);
        }

        internal static void Problem2()
        {
            (MapSpot pos, string directionsString, List<MapSpot> fixup) = Load2();

            foreach (MapSpot toFix in fixup)
            {
                if (toFix.Row == 1)
                {
                    if (toFix.Col < 101)
                    {
                        // 1 up to 6, up becomes right.
                        int relativeCol = toFix.Col - 50;
                        int mappedRow = 150 + relativeCol;
                        int mappedCol = 1;

                        MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                        toFix.Up = other;
                        toFix.UpRotation = 1;
                    }
                    else
                    {
                        // 2 up to 6 (up stays up)
                        int relativeCol = toFix.Col - 100;
                        MapSpot other = fixup.Single(ms => ms.Row == 200 && ms.Col == relativeCol);

                        toFix.Up = other;
                    }
                }
                else if (toFix.Row == 50 && toFix.Col > 100)
                {
                    // 2 down to 3 (down becomes left)
                    int relativeCol = toFix.Col - 100;
                    int mappedRow = 50 + relativeCol;
                    int mappedCol = 100;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Down = other;
                    toFix.DownRotation = 1;
                }
                else if (toFix.Row == 101 && toFix.Col < 51)
                {
                    // 5 up to 3 (up becomes right)
                    int relativeCol = toFix.Col;
                    int mappedRow = 50 + relativeCol;
                    int mappedCol = 51;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Up = other;
                    toFix.UpRotation = 1;
                }
                else if (toFix.Row == 150 && toFix.Col > 50)
                {
                    // 4 down to 6 (down becomes left)
                    int relativeCol = toFix.Col - 50;
                    int mappedRow = 150 + relativeCol;
                    int mappedCol = 50;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Down = other;
                    toFix.DownRotation = 1;
                }
                else if (toFix.Row == 200)
                {
                    // 6 down to 2 (down is down)
                    int relativeCol = toFix.Col;
                    int mappedCol = 100 + relativeCol;
                    int mappedRow = 1;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Down = other;
                }

                if (toFix.Col == 51 && toFix.Row < 51)
                {
                    // 1 left to 5 (left becomes right)
                    int relativeRow = toFix.Row;
                    int mappedCol = 1;
                    int mappedRow = 151 - relativeRow;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Left = other;
                    toFix.LeftRotation = 2;
                }
                else if (toFix.Col == 51 && toFix.Row > 50 && toFix.Row < 101)
                {
                    // 3 left to 5 (left becomes down)
                    int relativeRow = toFix.Row - 50;
                    int mappedRow = 101;
                    int mappedCol = relativeRow;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Left = other;
                    toFix.LeftRotation = -1;
                }
                else if (toFix.Col == 1 && toFix.Row < 151)
                {
                    // 5 left to 1 (left becomes right)
                    int relativeRow = toFix.Row - 100;
                    int mappedRow = 51 - relativeRow;
                    int mappedCol = 51;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Left = other;
                    toFix.LeftRotation = 2;
                }
                else if (toFix.Col == 1 && toFix.Row > 150)
                {
                    // 6 left to 1 (left becomes down)
                    int relativeRow = toFix.Row - 150;
                    int mappedCol = 50 + relativeRow;
                    int mappedRow = 1;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Left = other;
                    toFix.LeftRotation = -1;
                }
                else if (toFix.Col == 150)
                {
                    // 2 right to 4 (right becomes left)
                    int relativeRow = toFix.Row;
                    int mappedCol = 100;
                    int mappedRow = 151 - relativeRow;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Right = other;
                    toFix.RightRotation = 2;
                }
                else if (toFix.Col == 100 && toFix.Row > 50 && toFix.Row < 101)
                {
                    // 3 right to 2 (right becomes up)
                    int relativeRow = toFix.Row - 50;
                    int mappedCol = 100 + relativeRow;
                    int mappedRow = 50;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Right = other;
                    toFix.RightRotation = -1;
                }
                else if (toFix.Col == 100 && toFix.Row > 100)
                {
                    // 4 right to 2 (right becomes left)
                    int relativeRow = toFix.Row - 100;
                    int mappedRow = 51 - relativeRow;
                    int mappedCol = 150;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Right = other;
                    toFix.RightRotation = 2;
                }
                else if (toFix.Col == 50 && toFix.Row > 150)
                {
                    // 6 right to 4 (right becomes up)
                    int relativeRow = toFix.Row - 150;
                    int mappedCol = 50 + relativeRow;
                    int mappedRow = 150;

                    MapSpot other = fixup.Single(ms => ms.Row == mappedRow && ms.Col == mappedCol);

                    toFix.Right = other;
                    toFix.RightRotation = -1;
                }
            }

            foreach (MapSpot spot in fixup)
            {
                Debug.Assert(spot.Up is not null);
                Debug.Assert(spot.Up.HasPeer(spot));

                Debug.Assert(spot.Right is not null);
                Debug.Assert(spot.Right.HasPeer(spot));

                Debug.Assert(spot.Down is not null);
                Debug.Assert(spot.Down.HasPeer(spot));

                Debug.Assert(spot.Left is not null);
                Debug.Assert(spot.Left.HasPeer(spot));
            }

            ReadOnlySpan<char> directions = directionsString;
            int facing = 0;
            bool move = true;

            while (!directions.IsEmpty)
            {
                if (move)
                {
                    int count = ReadNumber(ref directions);

                    for (int i = 0; i < count; i++)
                    {
                        MapSpot next;
                        int facingRotation;

                        switch (facing)
                        {
                            case 0:
                                next = pos.Right;
                                facingRotation = pos.RightRotation;

                                if (pos.Col % 50 == 0)
                                {
                                }

                                break;

                            case 1:
                                next = pos.Down;
                                facingRotation = pos.DownRotation;

                                if (pos.Row % 50 == 0)
                                {
                                }

                                break;

                            case 2:
                                next = pos.Left;
                                facingRotation = pos.LeftRotation;

                                if (pos.Col % 50 == 1)
                                {
                                }

                                break;

                            case 3:
                                next = pos.Up;
                                facingRotation = pos.UpRotation;

                                if (pos.Row % 50 == 1)
                                {
                                }

                                break;

                            default:
                                throw new InvalidOperationException();
                        }

                        if (next.Wall)
                        {
                            break;
                        }

                        facing = (facing + facingRotation + 4) % 4;
                        pos = next;
                    }
                }
                else
                {
                    if (directions[0] == 'R')
                    {
                        facing = (facing + 1) % 4;
                    }
                    else
                    {
                        Debug.Assert(directions[0] == 'L');

                        facing--;

                        if (facing < 0)
                        {
                            facing = 3;
                        }
                    }

                    directions = directions.Slice(1);
                }

                move = !move;
            }

            Console.WriteLine($"Final Position: R:{pos.Row}, C:{pos.Col}, F:{facing}");
            Console.WriteLine(1000 * pos.Row + 4 * pos.Col + facing);
        }

        private static (MapSpot first, string secondPart, List<MapSpot> fixup) Load2()
        {
            MapSpot[] topLinks;
            MapSpot[] bottomLinks;
            MapSpot first = null;
            int row = 1;
            bool firstPart = true;
            string secondPart = null;
            List<MapSpot> fixup = new List<MapSpot>();

            string prep = Data.Enumerate().First();
            topLinks = new MapSpot[prep.Length + 1];
            bottomLinks = new MapSpot[prep.Length + 1];

            foreach (string s in Data.Enumerate())
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    firstPart = false;
                    continue;
                }

                if (!firstPart)
                {
                    secondPart = s;
                    break;
                }

                int col = 0;
                MapSpot left = null;
                MapSpot right = null;
                MapSpot prev = null;

                foreach (char c in s)
                {
                    col++;

                    if (c == ' ')
                    {
                        continue;
                    }

                    MapSpot spot = new MapSpot()
                    {
                        Col = col,
                        Row = row,
                    };

                    first ??= spot;

                    if (row % 50 is 0 or 1 || col % 50 is 0 or 1)
                    {
                        fixup.Add(spot);
                    }

                    if (left is null)
                    {
                        left = spot;
                    }
                    else
                    {
                        prev.Right = spot;
                        spot.Left = prev;
                    }

                    prev = spot;
                    right = spot;

                    if (topLinks[col] is null)
                    {
                        topLinks[col] = spot;
                    }
                    else
                    {
                        spot.Up = bottomLinks[col];
                        bottomLinks[col].Down = spot;
                    }

                    bottomLinks[col] = spot;
                    spot.Wall = (c == '#');
                }

                right.Right = left;
                left.Left = right;
                row++;
            }

            for (int i = 1; i < topLinks.Length; i++)
            {
                topLinks[i].Up = bottomLinks[i];
                bottomLinks[i].Down = topLinks[i];
            }

            return (first, secondPart, fixup);
        }
    }
}
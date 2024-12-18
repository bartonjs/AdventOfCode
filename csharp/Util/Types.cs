using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AdventOfCode.Util
{
    [Flags]
    public enum Directions2D
    {
        None,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
    }

    public struct Point : IEquatable<Point>, IComparable<Point>
    {
        public const Directions2D AllDirections = (Directions2D)15;

        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int ManhattanDistance(Point other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public override string ToString() => $"({X}, {Y})";

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point a, Point b) => a.Equals(b);
        public static bool operator !=(Point a, Point b) => !a.Equals(b);

        public Point North()
        {
            return new Point(X, Y - 1);
        }

        public Point South()
        {
            return new Point(X, Y + 1);
        }

        public Point West()
        {
            return new Point(X - 1, Y);
        }

        public Point East()
        {
            return new Point(X + 1, Y);
        }

        public Point GetNeighbor(char direction)
        {
            return direction switch
            {
                '^' => North(),
                '>' => East(),
                'v' => South(),
                '<' => West(),
                _ => throw new ArgumentException("Exactly one direction bit must be set", nameof(direction)),
            };
        }

        public Point GetNeighbor(Directions2D direction)
        {
            return direction switch
            {
                Directions2D.North => North(),
                Directions2D.East => East(),
                Directions2D.South => South(),
                Directions2D.West => West(),
                _ => throw new ArgumentException("Exactly one direction bit must be set", nameof(direction)),
            };
        }

        public IEnumerable<Point> GetNeighbors(Directions2D directions)
        {
            if ((directions & ~AllDirections) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(directions), "Unknown direction(s) asserted.");
            }

            int rem = (int)directions;

            if ((rem & 1) != 0)
            {
                yield return North();
            }

            rem >>= 1;

            if ((rem & 1) != 0)
            {
                yield return East();
            }

            rem >>= 1;
            
            if ((rem & 1) != 0)
            {
                yield return South();
            }

            rem >>= 1;
            
            if ((rem & 1) != 0)
            {
                yield return West();
            }
        }

        public int CompareTo(Point other)
        {
            int xComparison = X.CompareTo(other.X);
            if (xComparison != 0) return xComparison;
            return Y.CompareTo(other.Y);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
    }

    public struct LongPoint : IEquatable<LongPoint>
    {
        public long X;
        public long Y;

        public LongPoint(long x, long y)
        {
            X = x;
            Y = y;
        }

        public long ManhattanDistance(LongPoint other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public override string ToString() => $"({X}, {Y})";

        public bool Equals(LongPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is LongPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(LongPoint a, LongPoint b) => a.Equals(b);
        public static bool operator !=(LongPoint a, LongPoint b) => !a.Equals(b);

        public static LongPoint operator +(LongPoint a, LongPoint b)
        {
            return new LongPoint(a.X + b.X, a.Y + b.Y);
        }

        public static LongPoint operator -(LongPoint a, LongPoint b)
        {
            return new LongPoint(a.X - b.X, a.Y - b.Y);
        }
    }

    public struct Point3 : IEquatable<Point3>
    {
        public int X;
        public int Y;
        public int Z;

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int ManhattanDistance(Point3 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Point3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Point3 a, Point3 b) => a.Equals(b);
        public static bool operator !=(Point3 a, Point3 b) => !a.Equals(b);
    }

    public struct LongPoint3 : IEquatable<LongPoint3>
    {
        public long X;
        public long Y;
        public long Z;

        public LongPoint3(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public long ManhattanDistance(LongPoint3 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public bool Equals(LongPoint3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is LongPoint3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(LongPoint3 a, LongPoint3 b) => a.Equals(b);
        public static bool operator !=(LongPoint3 a, LongPoint3 b) => !a.Equals(b);
    }

    public sealed class Plane
    {
        public static (DynamicPlane<char> Plane, Point Point) LoadCharPlane(
            IEnumerable<string> source,
            char needle)
        {
            ArgumentNullException.ThrowIfNull(source);

            Span<Point> locations = stackalloc Point[1];
            DynamicPlane<char> ret = LoadCharPlaneCore(source, [needle], locations);
            return (ret, locations[0]);
        }

        public static (DynamicPlane<char> Plane, Point Point1, Point Point2) LoadCharPlane(
            IEnumerable<string> source,
            char needle1,
            char needle2)
        {
            ArgumentNullException.ThrowIfNull(source);

            Span<Point> locations = stackalloc Point[2];
            DynamicPlane<char> ret = LoadCharPlaneCore(source, [needle1, needle2], locations);
            return (ret, locations[0], locations[1]);
        }

        public static DynamicPlane<char> LoadCharPlane(IEnumerable<string> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return LoadCharPlaneCore(source, default, default);
        }

        public static DynamicPlane<char> LoadCharPlane(
            IEnumerable<string> source,
            ReadOnlySpan<char> needles,
            Span<Point> locations)
        {
            if (needles.Length != locations.Length)
                throw new ArgumentException("Needles and lengths must have the same Length");

            return LoadCharPlaneCore(
                source,
                needles,
                locations);
        }

        private static DynamicPlane<char> LoadCharPlaneCore(
            IEnumerable<string> source,
            ReadOnlySpan<char> needles,
            Span<Point> locations)
        {
            SearchValues<char> searchValues = SearchValues.Create(needles);
            DynamicPlane<char> plane = null;
            int row = 0;

            foreach (string s in source)
            {
                char[] array = s.ToCharArray();
                ReadOnlySpan<char> line = array;

                int idx = line.IndexOfAny(searchValues);

                while (idx >= 0)
                {
                    int needleIdx = needles.IndexOf(line[idx]);
                    locations[needleIdx] = new Point(idx, row);

                    int nextRelativeIdx = line.Slice(idx + 1).IndexOfAny(searchValues);

                    if (nextRelativeIdx >= 0)
                    {
                        idx += nextRelativeIdx + 1;
                    }
                    else
                    {
                        idx = -1;
                    }
                }

                if (plane is null)
                {
                    plane = new DynamicPlane<char>(array);
                }
                else
                {
                    plane.PushY(array);
                }

                row++;
            }

            return plane;
        }
    }

    public abstract class Plane<T>
    {
        public abstract ref T this[Point point] { get; }
        public abstract bool ContainsPoint(Point point);
        public abstract IEnumerable<Point> AllPoints();
        public abstract Plane<T> Clone();

        public bool TryGetValue(Point point, out T value)
        {
            if (ContainsPoint(point))
            {
                value = this[point];
                return true;
            }

            value = default;
            return false;
        }
    }

    public sealed class FixedPlane<T> : Plane<T>
    {
        private readonly int _width;
        private readonly int _height;
        private readonly T[,] _data;

        public FixedPlane(int width, int height)
        {
            _data = new T[width, height];
            _width = width;
            _height = height;
        }

        public override ref T this[Point point] => ref _data[point.X, point.Y];

        public override bool ContainsPoint(Point point)
        {
            return point.X >= 0 && point.X < _width && point.Y >= 0 && point.Y < _height;
        }

        public override IEnumerable<Point> AllPoints()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        public override Plane<T> Clone()
        {
            FixedPlane<T> clone = new FixedPlane<T>(_width, _height);
            Array.Copy(_data, clone._data, _data.Length);
            return clone;
        }
    }

    public sealed class DynamicPlane<T> : Plane<T>
    {
        private readonly int _width;
        private readonly List<T[]> _data;

        public DynamicPlane(int width, int heightHint = 0)
        {
            _data = new List<T[]>(heightHint);
            _width = width;
        }

        public DynamicPlane(T[] row0, int heightHint = 0)
        {
            _data = new List<T[]>(heightHint);
            _width = row0.Length;

            _data.Add(row0);
        }

        public override ref T this[Point point] => ref _data[point.Y][point.X];

        public override bool ContainsPoint(Point point)
        {
            return point.X >= 0 && point.X < _width && point.Y >= 0 && point.Y < _data.Count;
        }

        public override IEnumerable<Point> AllPoints()
        {
            int height = Height;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        public int Height => _data.Count;
        public int Width => _width;

        public T[] PushY()
        {
            T[] row = new T[_width];
            _data.Add(row);

            return row;
        }

        public void PushY(T[] row)
        {
            if (row.Length != _width)
            {
                throw new ArgumentException("Bad width");
            }

            _data.Add(row);
        }

        public string Print(Func<T, char> display)
        {
            StringBuilder builder = new StringBuilder();

            foreach (T[] row in _data)
            {
                foreach (T val in row)
                {
                    builder.Append(display(val));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public override Plane<T> Clone()
        {
            DynamicPlane<T> clone = new(_width, _data.Count);

            foreach (T[] arr in _data)
            {
                clone._data.Add((T[])arr.Clone());
            }

            return clone;
        }

        public IEnumerable<T[]> AllRows()
        {
            foreach (T[] arr in _data)
            {
                yield return arr;
            }
        }
    }

    public sealed class KeyedSets<T>
    {
        private readonly Dictionary<T, HashSet<T>> _data = new();

        public bool Add(T key, T item)
        {
            ref HashSet<T> set = ref CollectionsMarshal.GetValueRefOrAddDefault(_data, key, out _);

            if (set is null)
            {
                set = new HashSet<T>();
            }

            return set.Add(item);
        }

        public bool Contains(T key, T item)
        {
            if (_data.TryGetValue(key, out HashSet<T> set))
            {
                return set.Contains(item);
            }

            return false;
        }
    }
}
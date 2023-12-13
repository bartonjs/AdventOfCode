using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day12
    {
        internal static void Problem1()
        {
            long ret = 0;

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                char[] status = parts[0].ToCharArray();
                int[] lengths = parts[1].Split(',').Select(int.Parse).ToArray();

                int count = MakeVariations(status, lengths).Count();
                Utils.TraceForSample($"{s} => {count}");
                ret += count;
            }

            Console.WriteLine(ret);
        }

        internal static void Problem2()
        {
            long ret = 0;
            int line = 1;
            Cache<State, long> cache = new Cache<State, long>();

            foreach (string s in Data.Enumerate())
            {
                string[] parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                char[] status = parts[0].ToCharArray();
                char[] unfoldedStatus = new char[(parts[0].Length + 1) * 5 - 1];
                unfoldedStatus.AsSpan().Fill('?');

                for (int i = 0; i < unfoldedStatus.Length; i += parts[0].Length + 1)
                {
                    parts[0].AsSpan().CopyTo(unfoldedStatus.AsSpan(i));
                }

                int[] lengths = parts[1].Split(',').Select(int.Parse).ToArray();
                int[] unfoldedLengths = new int[lengths.Length * 5];

                for (int i = 0; i < unfoldedLengths.Length; i += lengths.Length)
                {
                    lengths.AsSpan().CopyTo(unfoldedLengths.AsSpan(i));
                }

                Stopwatch sw = Stopwatch.StartNew();
                Console.WriteLine($"{line}: {s}");
                Console.WriteLine($"  {new string(unfoldedStatus)}");
                long count = CWD4(cache, unfoldedStatus, unfoldedLengths);
                Console.WriteLine($"    => {count} ({sw.Elapsed.TotalMilliseconds}ms)");
                ret += count;
                line++;
            }

            Console.WriteLine($"Cache hits: {cache.Hits}, misses: {cache.Misses}");
            Console.WriteLine(ret);
        }

        private static IEnumerable<char[]> MakeVariations(char[] start, int[] constraints)
        {
            ReadOnlySpan<char> startSpan = new ReadOnlySpan<char>(start);
            ReadOnlySpan<int> constraintSpan = new ReadOnlySpan<int>(constraints);

            if (FailsConstraints2(startSpan, constraintSpan))
            {
                yield break;
            }

            int idx = Array.IndexOf(start, '?');

            if (idx < 0)
            {
                //Utils.TraceForSample(new string(start));
                yield return start;
                yield break;
            }

            start[idx] = '.';

            foreach (var ret in MakeVariations(start, constraints))
            {
                yield return ret;
            }

            start[idx] = '#';

            foreach (var ret in MakeVariations(start, constraints))
            {
                yield return ret;
            }

            start[idx] = '?';
        }

        private static bool FailsConstraints(ReadOnlySpan<char> line, ReadOnlySpan<int> constraints)
        {
            if (line.Contains('?'))
            {
                return false;
            }

            int rangeStart = line.IndexOfAnyExcept('.');

            if (rangeStart < 0)
            {
                return !constraints.IsEmpty;
            }

            line = line.Slice(rangeStart);

            int length = line.IndexOf('.');

            if (length == -1)
            {
                length = line.Length;
            }

            if (constraints.IsEmpty)
            {
                return true;
            }

            if (length != constraints[0])
            {
                return true;
            }

            return FailsConstraints(line.Slice(length), constraints.Slice(1));
        }

        private static bool FailsConstraints2(ReadOnlySpan<char> line, ReadOnlySpan<int> constraints, bool prefix = false)
        {
            // All constraints exhausted means that everything left has to be blank.
            // (? is OK, it just has to end up as blank... which may end up meaning this needs
            // to change to say "only 1 from here out")
            if (constraints.IsEmpty)
            {
                return line.Contains('#');
            }

            // Move past leading blanks.
            int rangeStart = line.IndexOfAnyExcept('.');

            if (rangeStart > 0)
            {
                line = line.Slice(rangeStart);
            }
            else if (rangeStart < 0)
            {
                // Everything is whitespace (or it's empty).
                // If we're prefix matching, having constraints left is OK.
                // If we're not, it's not.
                return !prefix;
            }

            int nextConstraint = constraints[0];

            // If there's a ? left, see if everything before it is OK.
            int query = line.IndexOf('?');
            int tokenEnd = line.IndexOfOrLength('.');

            if (query >= 0)
            {
                int lastSpace = line.Slice(0, query).LastIndexOf('.');

                if (lastSpace >= 0)
                {
                    if (FailsConstraints2(line.Slice(0, lastSpace), constraints, true))
                    {
                        return true;
                    }
                }

                // Even if all the ?s were #s it's still too short.
                if (tokenEnd < nextConstraint)
                {
                    return true;
                }

                int minLen = 0;

                foreach (int constraint in constraints)
                {
                    if (minLen > 0)
                    {
                        minLen++;
                    }

                    minLen += constraint;
                }

                if (line.Length < minLen)
                {
                    return true;
                }

                return false;
            }

            if (tokenEnd != nextConstraint)
            {
                return true;
            }

            return FailsConstraints2(line.Slice(tokenEnd), constraints.Slice(1), prefix);
        }

        private static long CountWithoutDoing(
            ReadOnlySpan<char> line,
            ReadOnlySpan<int> constraints,
            int currentConstraint = -1)
        {
            // All constraints exhausted means that everything left has to be blank.
            if (constraints.IsEmpty && currentConstraint <= 0)
            {
                return line.Contains('#') ? 0 : 1;
            }

            bool floatingQuery = false;
            ReadOnlySpan<int> preservedConstraints = constraints;

            // Not working on a specific constraint yet
            if (currentConstraint < 0)
            {
                // Move past leading blanks.
                int rangeStart = line.IndexOfAnyExcept('.');

                if (rangeStart > 0)
                {
                    line = line.Slice(rangeStart);
                }
                else if (rangeStart < 0)
                {
                    // Everything is whitespace (or it's empty).
                    // If we have constraints left we've failed. Otherwise we've succeeded

                    return constraints.IsEmpty ? 1 : 0;
                }

                floatingQuery = line[0] == '?';
                
                int minLen = 0;

                foreach (int constraint in constraints)
                {
                    if (minLen > 0)
                    {
                        minLen++;
                    }

                    minLen += constraint;
                }

                if (line.Length < minLen)
                {
                    return 0;
                }

                currentConstraint = constraints[0];
                constraints = constraints.Slice(1);
            }

            if (line.IsEmpty)
            {
                if (currentConstraint > 0 || !constraints.IsEmpty)
                {
                    return 0;
                }

                return 1;
            }

            char cur = line[0];
            line = line.Slice(1);
            int ccM1 = currentConstraint - 1;

            if (floatingQuery)
            {
                Debug.Assert(line[0] == '?');
                Debug.Assert(currentConstraint != 0);

                long asSpace = CountWithoutDoing(line, preservedConstraints);
                long asPound = CountWithoutDoing(line, constraints, ccM1);

                return asSpace + asPound;
            }

            // 6 states remain:
            // '.' on cc0: proceed
            // '.' on cc > 0: fail
            // '#' on cc0: fail
            // '#' on cc > 0: proceed
            // '?' on cc0: proceed as space only
            // '?' on cc > 0: proceed as # only

            if (cur == '.' && currentConstraint > 0 ||
                cur == '#' && currentConstraint == 0)
            {
                return 0;
            }

            return CountWithoutDoing(line, constraints, ccM1);
        }
        
        private static long CWD4(
            Cache<State, long> cache,
            ReadOnlyMemory<char> line,
            ReadOnlyMemory<int> constraints,
            int currentConstraint = -1)
        {
            State state = new State(line, constraints, currentConstraint);

            if (cache.TryGetValue(state, out long knownValue))
            {
                return knownValue;
            }

            if (constraints.IsEmpty && currentConstraint < 0)
            {
                return cache[state] = line.Span.Contains('#') ? 0 : 1;
            }

            if (line.IsEmpty)
            {
                return cache[state] = (currentConstraint <= 0 && constraints.IsEmpty) ? 1 : 0;
            }

            char cur = line.Span[0];
            ReadOnlyMemory<char> cdr = line.Slice(1);
            int ccM1 = currentConstraint - 1;

            return (cur, currentConstraint) switch
            {
                ('.', 0 or -1) => cache[state] = CWD4(cache, cdr, constraints),
                ('.', _) => 0,
                ('#', 0) => 0,
                ('#', -1) => cache[state] = CWD4(cache, line, constraints.Slice(1), constraints.Span[0]),
                ('#', _) => cache[state] = CWD4(cache, cdr, constraints, ccM1),
                ('?', 0) => cache[state] = CWD4(cache, cdr, constraints), // force .
                ('?', -1) => cache[state] = (CWD4(cache, cdr, constraints) + CWD4(cache, cdr, constraints.Slice(1), constraints.Span[0] - 1)), // try both
                ('?', _) => cache[state] = CWD4(cache, cdr, constraints, ccM1), // force #
            };
        }

        private readonly struct State : IEquatable<State>
        {
            private readonly ReadOnlyMemory<char> _line;
            private readonly ReadOnlyMemory<int> _constraints;
            private readonly int _currentConstraint;

            public State(ReadOnlyMemory<char> line, ReadOnlyMemory<int> constraints, int currentConstraint)
            {
                _line = line;
                _constraints = constraints;
                _currentConstraint = currentConstraint;
            }

            public override bool Equals(object obj)
            {
                if (obj is State state4)
                {
                    return Equals(state4);
                }

                return false;
            }

            public bool Equals(State other)
            {
                return
                    other._currentConstraint == _currentConstraint &&
                    other._line.Span.SequenceEqual(_line.Span) &&
                    other._constraints.Span.SequenceEqual(_constraints.Span);
            }

            public override int GetHashCode()
            {
                HashCode hc = new HashCode();
                hc.AddBytes(MemoryMarshal.AsBytes(_line.Span));
                hc.AddBytes(MemoryMarshal.AsBytes(_constraints.Span));
                hc.Add(_currentConstraint);
                return hc.ToHashCode();
            }
        }
    }
}
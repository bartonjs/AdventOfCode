using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day14
    {
        internal static void Problem1()
        {
            Polymer p = new Polymer();
            p.Load();

            for (int i = 0; i < 10; i++)
            {
                p.Update();
            }

            Console.WriteLine(p.Part1Score());
        }

        internal static void Problem2()
        {
            Polymer2 p = new Polymer2();
            p.Load();

            Dictionary<short, long> tmp = null;

            for (int i = 0; i < 40; i++)
            {
#if SAMPLE
                Console.WriteLine($"Before update {i}");
                Console.WriteLine(p.Part1Score());
#endif
                p.Update(ref tmp);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Final");
            Console.WriteLine(p.Part1Score());
        }

        private class Polymer
        {
            private Dictionary<int, char> _rules;
            private char[] _cur;
            private string _asString;

            public override string ToString()
            {
                return (_asString ??= new string(_cur));
            }

            public void Load()
            {
                string start = null;
                var rules = new Dictionary<int, char>();

                foreach (string line in Data.Enumerate())
                {
                    if (start is null)
                    {
                        start = line;
                        continue;
                    }

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    int high = (line[0] - 'A');
                    int low = (line[1] - 'A');
                    int rule = high << 8 | low;

                    char dest = line[6];

                    rules.Add(rule, dest);
                }

                _cur = start.ToCharArray();
                _rules = rules;
            }

            public void Update()
            {
                char[] next = new char[_cur.Length * 2 - 1];

                next[0] = _cur[0];

                int writeAt = 1;
                short last = (short)(next[0] - 'A');

                for (int i = 1; i < _cur.Length; i++)
                {
                    last <<= 8;
                    last |= (short)(_cur[i] - 'A');
                    next[writeAt++] = _rules[last];
                    next[writeAt++] = _cur[i];
                }

                _cur = next;
                _asString = null;
            }

            public int Part1Score()
            {
                var list = _cur.Distinct().Select(c => (c, _cur.AsEnumerable().Count(x => x == c))).ToList();
                Console.WriteLine(string.Join(";", list));
                int max = list.Max(x => x.Item2);
                int min = list.Min(x => x.Item2);

                return max - min;
            }
        }

        private class Polymer2
        {
            private Dictionary<short, short> _rules;
            private Dictionary<short, long> _state;
            private short _firstCode;

            public void Load()
            {
                string start = null;
                var rules = new Dictionary<short, short>();

                foreach (string line in Data.Enumerate())
                {
                    if (start is null)
                    {
                        start = line;
                        continue;
                    }

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    int high = (line[0] - 'A');
                    int low = (line[1] - 'A');
                    int rule = high << 8 | low;

                    char dest = line[6];

                    rules.Add((short)rule, (short)(dest - 'A'));
                }

                Dictionary<short, long> state = new();

                {
                    short last = (short)(start[0] - 'A');
                    _firstCode = last;

                    for (int i = 1; i < start.Length; i++)
                    {
                        last <<= 8;
                        last |= (short)(start[i] - 'A');

                        if (!state.TryGetValue(last, out long cur))
                        {
                            cur = 0;
                        }

                        state[last] = cur + 1;
                    }
                }

                _rules = rules;
                _state = state;
            }

            public void Update(ref Dictionary<short, long> tmp)
            {
                tmp ??= new Dictionary<short, long>();
                tmp.Clear();

                foreach (var kvp in _state)
                {
                    short next = _rules[kvp.Key];
                    short key = (short)((short)(kvp.Key & 0xFF00) | next);

                    if (!tmp.TryGetValue(key, out var cur))
                    {
                        cur = 0;
                    }

                    tmp[key] = cur + kvp.Value;

                    key <<= 8;
                    key |= (short)(kvp.Key & 0xFF);

                    if (!tmp.TryGetValue(key, out cur))
                    {
                        cur = 0;
                    }

                    tmp[key] = cur + kvp.Value;
                }

                _state.Clear();

                foreach (var kvp in tmp)
                {
                    _state[kvp.Key] = kvp.Value;
                }
            }

            public long Part1Score()
            {
                long[] perLetter = new long[26];

                foreach (var kvp in _state)
                {
                    perLetter[kvp.Key & 0xFF] += kvp.Value;
                }

                perLetter[_firstCode]++;

                for (int i = 0; i < perLetter.Length; i++)
                {
                    if (perLetter[i] != 0)
                    {
                        Console.WriteLine($"{(char)(i + 'A')}: {perLetter[i]}");
                    }
                }

                long max = perLetter.Where(x => x != 0).Max();
                long min = perLetter.Where(x => x != 0).Min();

                return max - min;
            }
        }
    }
}
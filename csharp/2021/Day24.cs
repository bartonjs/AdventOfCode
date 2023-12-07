using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day24
    {
        internal static void Problem1Brute()
        {
            ALU alu = new ALU();
            //long serial = 99998665325165;
            long serial = 91599994399495;

            char[] serialStr = new char[14];
            serial.TryFormat(serialStr, out _);

            while (!alu.CheckSerial(serialStr))
            {
                serial--;
                serial.TryFormat(serialStr, out _);
            }

            Console.WriteLine(serial);
        }

        private class ALU
        {
            private readonly Dictionary<char, int> _registers = new Dictionary<char, int>(4);

            private Queue<int> _input = new Queue<int>();
            private IEnumerator<string> _data;

            public bool CheckSerial(IList<char> value)
            {
                if (value.Any(c => c == '0'))
                {
                    return false;
                }

                _registers['w'] = 0;
                _registers['x'] = 0;
                _registers['y'] = 0;
                _registers['z'] = 0;
                _data = Data.Enumerate().GetEnumerator();

                foreach (char c in value)
                {
                    _input.Enqueue(c - '0');
                }

                while (Next())
                {
                }

                return _registers['z'] == 0;
            }

            private bool Next()
            {
                if (!_data.MoveNext())
                {
                    return false;
                }

                string line = _data.Current;
                char register0 = line[4];

                // inp
                if (line[1] == 'n')
                {
                    _registers[register0] = _input.Dequeue();
                    return true;
                }

                int arg2;
                
                if (!int.TryParse(line.AsSpan(6), out arg2))
                {
                    Debug.Assert(line.Length == 7);
                    arg2 = _registers[line[6]];
                }

                switch (line[1])
                {
                    //add
                    case 'd':
                        _registers[register0] += arg2;
                        break;
                    //mul
                    case 'u':
                        _registers[register0] *= arg2;
                        break;
                    //div
                    case 'i':
                        _registers[register0] /= arg2;
                        break;
                    //mod
                    case 'o':
                        _registers[register0] %= arg2;
                        break;
                    //eql
                    case 'q':
                        _registers[register0] = (_registers[register0] == arg2) ? 1 : 0;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                return true;
            }

            private int Value(int? immediate, char register)
            {
                if (immediate.HasValue)
                {
                    return immediate.Value;
                }

                return _registers[register];
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode.Util
{
    public sealed class Print
    {
        public static bool VerboseEnabled { get; private set; }

        [Conditional("SAMPLE")]
        public static void ForSample(string message)
        {
            Console.WriteLine(message);
        }

        public static void Verbose(string message)
        {
            if (VerboseEnabled)
            {
                Console.WriteLine(message);
            }
        }

        public static void Verbose(ref VerboseMessageHandler message)
        {
            if (VerboseEnabled)
            {
                Console.WriteLine(message.ToStringAndClear());
            }
        }

        public static void EnableVerbose()
        {
            VerboseEnabled = true;
        }

        [InterpolatedStringHandler]
        public struct VerboseMessageHandler
        {
            private StringBuilder _builder;

            public VerboseMessageHandler(int literalLength, int formattedCount, out bool shouldAppend)
            {
                if (VerboseEnabled)
                {
                    _builder = new StringBuilder(literalLength + formattedCount * 8);
                    shouldAppend = true;
                }
                else
                {
                    shouldAppend = false;
                }
            }

            public void AppendLiteral(string value) => _builder.Append(value);

            public void AppendFormatted(object value) => _builder.Append(value);

            internal string ToStringAndClear()
            {
                string ret = _builder.ToString();
                _builder = null;
                return ret;
            }
        }
    }
}
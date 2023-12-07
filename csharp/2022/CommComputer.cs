using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode2022.Solutions
{
    internal partial class CommComputer
    {
        [GeneratedRegex(@"^(\w+)$")]
        private static partial Regex Arg0Regex();
        [GeneratedRegex(@"^(\w+) (-?\d+)")]
        private static partial Regex Arg1LRegex();

        internal static IEnumerable<Instruction> ReadInstructions()
        {
            Regex arg0Regex = Arg0Regex();
            Regex arg1Regex = Arg1LRegex();

            foreach (string s in Data.Enumerate())
            {
                Match m = arg1Regex.Match(s);

                if (m.Success)
                {
                    yield return new Instruction(
                        m.Groups[1].Value,
                        int.Parse(m.Groups[2].ValueSpan));

                    continue;
                }

                m = arg0Regex.Match(s);

                if (!m.Success)
                {
                    throw new InvalidDataException();
                }

                yield return new Instruction(s);
            }
        }
    }
}
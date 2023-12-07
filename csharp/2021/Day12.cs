using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    internal class Day12
    {
        internal static void Problem1()
        {
            CaveComplex complex = CaveComplex.Load();
            List<CavePath> allPaths = new List<CavePath>();

            TryAllPaths(complex.NewPath(), allPaths);

            foreach (var path in allPaths)
            {
                Console.WriteLine(path);
            }

            Console.WriteLine(allPaths.Count);
        }

        internal static void Problem2()
        {
            CavePath.Exceptions = 1;
            CaveComplex complex = CaveComplex.Load();
            List<CavePath> allPaths = new List<CavePath>();

            TryAllPaths(complex.NewPath(), allPaths);

#if SAMPLE
            foreach (var path in allPaths)
            {
                Console.WriteLine(path);
            }
#endif

            Console.WriteLine(allPaths.Count);
        }

        private static void TryAllPaths(CavePath path, List<CavePath> allPaths)
        {
            bool first = true;
            string holdBack = null;

            foreach (string nextDestination in path.NextDestinations())
            {
                if (first)
                {
                    holdBack = nextDestination;
                    first = false;
                    continue;
                }

                TryAllPaths(path.CloneAndVisit(nextDestination), allPaths);
            }

            if (holdBack != null)
            {
                path.Visit(holdBack);
                TryAllPaths(path, allPaths);
            }
            else if (path.Ended)
            {
                allPaths.Add(path);
            }
        }

        private class CaveComplex
        {
            internal readonly Dictionary<string, List<string>> _connections = new Dictionary<string, List<string>>();

            internal static CaveComplex Load()
            {
                CaveComplex complex = new CaveComplex();

                foreach (string line in Data.Enumerate())
                {
                    int hyphen = line.IndexOf('-');
                    string car = line.Substring(0, hyphen);
                    string cdr = line.Substring(hyphen + 1);

                    if (!complex._connections.TryGetValue(car, out var list))
                    {
                        list = new List<string>();
                        complex._connections[car] = list;
                    }

                    if (!"start".Equals(cdr))
                    {
                        list.Add(cdr);
                    }

                    if (!complex._connections.TryGetValue(cdr, out list))
                    {
                        list = new List<string>();
                        complex._connections[cdr] = list;
                    }

                    if (!"start".Equals(car))
                    {
                        list.Add(car);
                    }
                }

                return complex;
            }

            internal CavePath NewPath()
            {
                return new CavePath(this);
            }
        }

        private class CavePath
        {
            public const int Limit = 1;
            public static int Exceptions = 0;
            private readonly Dictionary<string, int> _smallCaveCounts = new Dictionary<string, int>();
            private readonly List<string> _route = new List<string>();
            private readonly CaveComplex _complex;
            private int _exceptionCount;

            public bool Ended { get; private set; }

            private CavePath(CavePath seed)
            {
                _complex = seed._complex;

                foreach (string place in seed._route)
                {
                    Visit(place);
                }
            }

            internal CavePath(CaveComplex complex)
            {
                _complex = complex;
                Visit("start");
            }

            internal IEnumerable<string> NextDestinations()
            {
                if (Ended)
                {
                    yield break;
                }

                string curPos = _route[^1];
                var allDestinations = _complex._connections[curPos];

                foreach (string candidate in allDestinations)
                {
                    if (char.IsLower(candidate[0]))
                    {
                        if (_smallCaveCounts.TryGetValue(candidate, out int visited) && visited >= Limit && _exceptionCount >= Exceptions)
                        {
                            continue;
                        }
                    }

                    yield return candidate;
                }
            }

            public void Visit(string nextDestination)
            {
                _route.Add(nextDestination);

                if (char.IsLower(nextDestination[0]))
                {
                    if (_smallCaveCounts.TryGetValue(nextDestination, out int cur))
                    {
                        _smallCaveCounts[nextDestination] = cur + 1;

                        if (cur >= Limit)
                        {
                            _exceptionCount++;
                        }
                    }
                    else if ("start".Equals(nextDestination))
                    {
                        _smallCaveCounts[nextDestination] = Limit;
                    }
                    else if (nextDestination.Equals("end"))
                    {
                        Ended = true;
                    }
                    else
                    {
                        _smallCaveCounts[nextDestination] = 1;
                    }
                }
            }

            public CavePath CloneAndVisit(string nextDestination)
            {
                CavePath clone = new CavePath(this);
                clone.Visit(nextDestination);
                return clone;
            }

            public override string ToString()
            {
                return string.Join("->", _route);
            }
        }
    }
}
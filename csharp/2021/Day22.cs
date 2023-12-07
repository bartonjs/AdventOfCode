using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventOfCode.Util;

namespace AdventOfCode2021.Solutions
{
    public class Day22
    {
        internal static void Problem1()
        {
            bool[][][] reactor = new bool[101][][];

            for (int i = 0; i < reactor.Length; i++)
            {
                var plane = new bool[101][];
                reactor[i] = plane;

                for (int j = 0; j < plane.Length; j++)
                {
                    plane[j] = new bool[101];
                }
            }

            Regex regex = new Regex(@"=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)");

            foreach (string line in Data.Enumerate())
            {
                bool setState = line[1] == 'n';

                Match match = regex.Match(line);

                int xLow = Math.Max(-50, int.Parse(match.Groups[1].Value));
                int xHigh = Math.Min(50, int.Parse(match.Groups[2].Value));
                int yLow = Math.Max(-50, int.Parse(match.Groups[3].Value));
                int yHigh = Math.Min(50, int.Parse(match.Groups[4].Value));
                int zLow = Math.Max(-50, int.Parse(match.Groups[5].Value));
                int zHigh = Math.Min(50, int.Parse(match.Groups[6].Value));

                for (int x = xLow; x <= xHigh; x++)
                {
                    bool[][] plane = reactor[x + 50];

                    for (int y = yLow; y <= yHigh; y++)
                    {
                        bool[] ray = plane[y + 50];

                        for (int z = zLow; z <= zHigh; z++)
                        {
                            ray[z + 50] = setState;
                        }
                    }
                }
            }

            Console.WriteLine(reactor.SelectMany(x => x).SelectMany(x => x).Count(x => x));
        }

        internal static void Problem2()
        {
            List<Prism> setStates = new List<Prism>();

            Regex regex = new Regex(@"=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)");

            foreach (string line in Data.Enumerate())
            {
                bool setState = line[1] == 'n';

                Match match = regex.Match(line);

                int xLow = int.Parse(match.Groups[1].Value);
                int xHigh = int.Parse(match.Groups[2].Value);
                int yLow = int.Parse(match.Groups[3].Value);
                int yHigh = int.Parse(match.Groups[4].Value);
                int zLow = int.Parse(match.Groups[5].Value);
                int zHigh = int.Parse(match.Groups[6].Value);

                if (xHigh < xLow || yHigh < yLow || zHigh < zLow)
                {
                    continue;
                }

                Prism thisPrism = new Prism(xLow, xHigh, yLow, yHigh, zLow, zHigh);
                
                if (setState)
                {
                    List<Prism> remaining = new List<Prism> { thisPrism };

                    foreach (Prism cur in setStates)
                    {
                        for (int i = remaining.Count - 1; i >= 0; i--)
                        {
                            Prism target = remaining[i];

                            if (cur.Intersects(target))
                            {
                                remaining.RemoveAt(i);
                                remaining.AddRange(target.Remove(cur));
                            }
                        }
                    }

                    setStates.AddRange(remaining);
                }
                else
                {
                    for (int i = setStates.Count - 1; i >= 0; i--)
                    {
                        Prism target = setStates[i];

                        if (thisPrism.Intersects(target))
                        {
                            setStates.RemoveAt(i);
                            setStates.AddRange(target.Remove(thisPrism));
                        }
                    }
                }
            }

            Console.WriteLine($"Final solution tracked {setStates.Count} prism(s).");
            Console.WriteLine(setStates.Sum(p => p.Volume));
        }

        private record Prism(int XLow, int XHigh, int YLow, int YHigh, int ZLow, int ZHigh)
        {
            public long Volume => (long)(XHigh - XLow + 1) * (YHigh - YLow + 1) * (ZHigh - ZLow + 1);

            public bool Intersects(Prism other)
            {
                return
                    XLow <= other.XHigh && XHigh >= other.XLow &&
                    YLow <= other.YHigh && YHigh >= other.YLow &&
                    ZLow <= other.ZHigh && ZHigh >= other.ZLow;
            }

            public Prism Intersect(Prism other)
            {
                int xLow = Math.Max(XLow, other.XLow);
                int xHigh = Math.Min(XHigh, other.XHigh);
                int yLow = Math.Max(YLow, other.YLow);
                int yHigh = Math.Min(YHigh, other.YHigh);
                int zLow = Math.Max(ZLow, other.ZLow);
                int zHigh = Math.Min(ZHigh, other.ZHigh);

                if (xLow <= xHigh && yLow <= yHigh && zLow <= zHigh)
                {
                    return new Prism(xLow, xHigh, yLow, yHigh, zLow, zHigh);
                }

                return null;
            }

            public IEnumerable<Prism> Remove(Prism other)
            {
                Prism intersection = Intersect(other);

                if (intersection is null)
                {
                    yield return this;
                    yield break;
                }

                // The prism that is to the right of the intersection. (9/27)
                if (XHigh > intersection.XHigh)
                {
                    yield return new Prism(
                        intersection.XHigh + 1,
                        XHigh,
                        YLow,
                        YHigh,
                        ZLow,
                        ZHigh);
                }

                // To the left. (9/27 => 18/27)
                if (XLow < intersection.XLow)
                {
                    yield return new Prism(
                        XLow,
                        intersection.XLow - 1,
                        YLow,
                        YHigh,
                        ZLow,
                        ZHigh);
                }

                // Middle (L-R) top (3/27 => 21/27)
                if (YHigh > intersection.YHigh)
                {
                    yield return new Prism(
                        intersection.XLow,
                        intersection.XHigh,
                        intersection.YHigh + 1,
                        YHigh,
                        ZLow,
                        ZHigh);
                }

                // Middle bottom (3/27 => 24/27)
                if (YLow < intersection.YLow)
                {
                    yield return new Prism(
                        intersection.XLow,
                        intersection.XHigh,
                        YLow,
                        intersection.YLow - 1,
                        ZLow,
                        ZHigh);
                }

                // Middle/middle deep (1/27 => 25/27)
                if (ZHigh > intersection.ZHigh)
                {
                    yield return new Prism(
                        intersection.XLow,
                        intersection.XHigh,
                        intersection.YLow,
                        intersection.YHigh,
                        intersection.ZHigh + 1,
                        ZHigh);
                }

                // Middle/middle shallow (1/27 => 26/27)
                if (ZLow < intersection.ZLow)
                {
                    yield return new Prism(
                        intersection.XLow,
                        intersection.XHigh,
                        intersection.YLow,
                        intersection.YHigh,
                        ZLow,
                        intersection.ZLow - 1);
                }

                // The middle/middle/middle is what we removed. Done.
            }
        }
    }
}

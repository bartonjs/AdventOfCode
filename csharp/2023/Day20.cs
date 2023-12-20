using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode2023
{
    internal class Day20
    {
        internal static void Problem1()
        {
            Queue<(string From, string To, bool High)> queue = new();
            Dictionary<string, Thingy> world = Load();

            foreach (Thingy thingy in world.Values)
            {
                foreach (string targetName in thingy.Destinations)
                {
                    if (world.TryGetValue(targetName, out Thingy target))
                    {
                        if (target is Thingy.Conjunction conj)
                        {
                            conj.TrackSource(thingy.Name);
                        }
                    }
                }
            }

            for (int i = 0; i < 1000; i++)
            {
                queue.Enqueue(("button", "broadcaster", false));

                while (queue.Count > 0)
                {
                    (string srcName, string targetName, bool high) = queue.Dequeue();
#if SAMPLE
                    if (i < 2)
                    {
                        Console.WriteLine($"{srcName} -{(high ? "high" : "low")}-> {targetName}");
                    }
#endif
                    if (!world.TryGetValue(targetName, out Thingy target))
                    {
                        Console.WriteLine($"   --- creating synthetic drain '{targetName}'");
                        world[targetName] = target = new Thingy.Drain(targetName);
                    }

                    foreach (var response in target.Process(srcName, high))
                    {
                        queue.Enqueue(response);
                    }
                }
            }

            Console.WriteLine($"Low: {Thingy.LowPulses}");
            Console.WriteLine($"High: {Thingy.HighPulses}");

            Console.WriteLine(Thingy.HighPulses * Thingy.LowPulses);
        }

        internal static void Problem2()
        {
            Queue<(string From, string To, bool High)> queue = new();
            Dictionary<string, Thingy> world = Load();

            foreach (Thingy thingy in world.Values)
            {
                foreach (string targetName in thingy.Destinations)
                {
                    if (world.TryGetValue(targetName, out Thingy target))
                    {
                        if (target is Thingy.Conjunction conj)
                        {
                            conj.TrackSource(thingy.Name);
                        }
                    }
                }
            }

            int buttonPresses = 0;
            Thingy.Receiver rx = new Thingy.Receiver();
            world.Add(rx.Name, rx);

            Thingy.Conjunction trigger = (Thingy.Conjunction)world.Values.Single(t => t.Destinations.Contains(rx.Name));
            HashSet<string> pending = new(trigger.GetInputs());
            List<long> presses = new();

            foreach (string input in pending)
            {
                Console.WriteLine($"Tracking input {input}");
            }

            while (!rx.Done && pending.Count > 0)
            {
                buttonPresses++;
                queue.Enqueue(("button", "broadcaster", false));

                while (!rx.Done && queue.Count > 0)
                {
                    (string srcName, string targetName, bool high) = queue.Dequeue();
                    Thingy target = world[targetName];

                    if (buttonPresses == 3847)
                    {
                        Console.WriteLine($"{srcName} -{(high ? "high" : "low")}-> {targetName}");
                    }

                    foreach (var response in target.Process(srcName, high))
                    {
                        if (response.Item3 && pending.Remove(response.Item1))
                        {
                            // I'm not sure that this should work, but it did, because he's nice and never
                            // seems to do offset cycling.
                            //
                            // It's also way assuming, since every tracked node ends up going back to the
                            // low state before the button press ends, so there's not even a guarantee that
                            // they'd all be true at the same time...
                            // but, it's a speed-coding competition, so assumptions, assumptions, assumptions...
                            Console.WriteLine($"Input {response.Item1} sent high on press {buttonPresses}");
                            presses.Add(buttonPresses);
                        }

                        queue.Enqueue(response);
                    }
                }

                if (rx.Done)
                {
                    Console.WriteLine($"Shouldn't happen, but we finished on button press {buttonPresses}");
                    return;
                }
            }

            Console.WriteLine($"Exited after {buttonPresses}...");
            Console.WriteLine(Utils.LeastCommonMultiple(presses));
        }

        private static Dictionary<string, Thingy> Load()
        {
            Dictionary<string, Thingy> ret = new();

            foreach (string s in Data.Enumerate())
            {
                Thingy t = Thingy.Parse(s);
                ret[t.Name] = t;
            }

            return ret;
        }

        private abstract class Thingy
        {
            public static long HighPulses { get; private set; }
            public static long LowPulses { get; private set; }

            public string Name { get; private set; }
            public List<string> Destinations { get; } = new();

            public static Thingy Parse(string s)
            {
                int space = s.IndexOf(' ');
                char type = s[0];
                int start = 1;

                if (type is not ('%' or '&'))
                {
                    type = '\0';
                    start = 0;
                }

                string name = s.Substring(start, space - start);
                string cdr = s.Substring(space + 4);

                Thingy ret = type switch
                {
                    '%' => new FlipFlop(),
                    '&' => new Conjunction(),
                    '\0' => new Broadcaster(),
                };

                ret.Name = name;
                ret.Destinations.AddRange(cdr.Split(',').Select(s => s.Trim()));
                return ret;
            }

            public IEnumerable<(string, string, bool)> Process(string from, bool high)
            {
                if (high)
                {
                    HighPulses++;
                }
                else
                {
                    LowPulses++;
                }

                foreach (var response in ProcessCore(from, high))
                {
                    yield return response;
                }
            }

            protected abstract IEnumerable<(string, string, bool)> ProcessCore(string from, bool high);

            private class Broadcaster : Thingy
            {
                protected override IEnumerable<(string, string, bool)> ProcessCore(string from, bool high)
                {
                    foreach (string destination in Destinations)
                    {
                        yield return (Name, destination, high);
                    }
                }
            }

            private class FlipFlop : Thingy
            {
                private bool _state;

                protected override IEnumerable<(string, string, bool)> ProcessCore(string from, bool high)
                {
                    if (high)
                    {
                        yield break;
                    }

                    _state = !_state;
                    bool pulse = _state;

                    foreach (string destination in Destinations)
                    {
                        yield return (Name, destination, pulse);
                    }
                }
            }

            internal class Conjunction : Thingy
            {
                private readonly Dictionary<string, bool> _state = new();

                protected override IEnumerable<(string, string, bool)> ProcessCore(string from, bool high)
                {
                    _state[from] = high;
                    bool pulse = true;

                    if (high)
                    {
                        pulse = !_state.Values.All(v => v);
                    }

                    foreach (string destination in Destinations)
                    {
                        yield return (Name, destination, pulse);
                    }
                }

                public void TrackSource(string thingyName)
                {
                    _state[thingyName] = false;
                }

                public IEnumerable<string> GetInputs()
                {
                    return _state.Keys;
                }
            }

            public class Drain : Thingy
            {
                public Drain(string name)
                {
                    Name = name;
                }

                protected override IEnumerable<(string, string, bool)> ProcessCore(string from, bool high)
                {
                    yield break;
                }
            }

            public class Receiver : Thingy
            {
                public bool Done { get; private set; }

                public Receiver()
                {
                    Name = "rx";
                }

                protected override IEnumerable<(string, string, bool)> ProcessCore(string from, bool high)
                {
                    if (!high)
                    {
                        Done = true;
                    }

                    yield break;
                }
            }
        }
    }
}
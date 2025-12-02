using AdventOfCode.Util;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Program
    {
        private const string Year = "2025";
        private static readonly Type TargetType = typeof(AdventOfCode2025.Day01);

        private static bool s_perfRun;

        private static string ParseArgs(string[] args)
        {
            string dayArg = null;

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-perf":
                        s_perfRun = true;
                        Console.Error.WriteLine("Executing repeatedly for 5 seconds...");
                        break;
                    case "-verbose":
                        Print.EnableVerbose();
                        break;
                    default:
                        if (dayArg is not null)
                        {
                            Console.Error.WriteLine("Enter the day number, optionally with a -1 for part 1 (e.g. 14-1)");
                            throw new InvalidDataException();
                        }

                        dayArg = arg;
                        break;
                }
            }

            return dayArg;
        }

        private static async Task Main(string[] args)
        {
            MethodInfo method;
            string dayArg;
            int day;

            try
            {
                dayArg = ParseArgs(args);
            }
            catch (InvalidDataException)
            {
                return;
            }

            try
            {
                method = GetTarget(dayArg, out day);

                if (method == null)
                {
                    Console.Error.WriteLine($"No runnable problem found on day {day}.");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error determining what part to run.");
                Console.Error.WriteLine(e);
                return;
            }

            try
            {
                LoadData(day);
            }
            catch (Exception e)
            {
#if SAMPLE
                Console.Error.WriteLine($"Error loading sample data for day {day}.");
#else
                Console.Error.WriteLine($"Error loading data for day {day}.");
#endif
                Console.Error.WriteLine(e);
                return;
            }

            TimeSpan allowed = s_perfRun ? TimeSpan.FromSeconds(5) : TimeSpan.Zero;
            int runCount = 0;
            Stopwatch sw = new Stopwatch();

            do
            {
                try
                {
                    runCount++;
                    sw.Start();
                    object resp = method.Invoke(null, null);

                    if (resp is Task t)
                    {
                        await t.ConfigureAwait(false);
                    }

                    sw.Stop();
                }
                catch (TargetInvocationException tie)
                {
                    sw.Stop();
                    Console.WriteLine(tie.InnerException);
                }
            } while (sw.Elapsed < allowed);

#if SAMPLE
            Console.WriteLine("**SAMPLE ANSWER**");
#endif
            Console.Error.WriteLine($"Executed in {sw.Elapsed.TotalMilliseconds}ms.");

            if (s_perfRun)
            {
                Console.Error.WriteLine($"Ran {runCount} iterations, {sw.Elapsed.TotalMilliseconds / runCount:F4}ms each.");
            }
        }

        private static void LoadData(int day)
        {
#if SAMPLE
            string filename = $"Day{day:D2}_sample.txt";
#else
            string filename = $"Day{day:D2}.txt";
#endif

            string dataDirParent = AppContext.BaseDirectory;
            string dataDir = Path.Join(dataDirParent, "Data");

            for (int i = 0; i <= 6; i++)
            {
                if (!Directory.Exists(dataDir))
                {
                    dataDirParent = Path.Combine(dataDirParent, "..");
                    dataDir = Path.Join(dataDirParent, "Data");
                }
            }

            string[] lines = File.ReadAllLines(Path.Join(dataDir, Year, filename));
            typeof(Data).GetField("s_lines", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, lines);
        }

        [RequiresUnreferencedCode("")]
        private static MethodInfo GetTarget(string dayArg, out int day)
        {
            const BindingFlags Flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            string rootNS = TargetType.Namespace;

            if (dayArg is not null)
            {
                int hyphen = dayArg.IndexOf('-');
                int? part = null;

                if (hyphen >= 0)
                {
                    day = int.Parse(dayArg.AsSpan(0, hyphen));
                    part = int.Parse(dayArg.AsSpan(hyphen + 1));
                }
                else
                {
                    day = int.Parse(dayArg);
                }

                Type t = TargetType.Assembly.GetType($"{rootNS}.Day{day:D2}", true);

                if (part.HasValue)
                {
                    return t.GetMethod("Problem" + part, Flags);
                }

                return t.GetMethod("Problem2", Flags) ?? t.GetMethod("Problem1", Flags);
            }

            for (int i = 25; i > 0; i--)
            {
                Type t = TargetType.Assembly.GetType($"{rootNS}.Day{i:D2}", false);

                if (t != null)
                {
                    day = i;
                    return t.GetMethod("Problem2", Flags) ?? t.GetMethod("Problem1", Flags);
                }
            }

            throw new InvalidOperationException("Could not find a runnable day.");
        }
    }
}

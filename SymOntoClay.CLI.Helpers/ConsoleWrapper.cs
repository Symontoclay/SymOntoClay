using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI.Helpers
{
    public static class ConsoleWrapper
    {
        static ConsoleWrapper()
        {
            _defaultForegroundColor = Console.ForegroundColor;
        }

        private readonly static ConsoleColor _defaultForegroundColor;
        private readonly static object _lockObj = new object();

#if DEBUG
        public static bool WriteOutputToTextFileAsParallel
        {
            get
            {
                lock (_lockObj)
                {
                    return _writeOutputToTextFileAsParallel;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    if (_writeOutputToTextFileAsParallel == value)
                    {
                        return;
                    }

                    _writeOutputToTextFileAsParallel = value;

                    if (_writeOutputToTextFileAsParallel)
                    {
                        var now = DateTime.Now;
                        _parallelOutputTextFileName = Path.Combine(Directory.GetCurrentDirectory(), $"parallelOutputTextFile_{now:ddMMyyyy-HHmmss}.log");
                    }
                }
            }
        }

        private static bool _writeOutputToTextFileAsParallel;
        private static string _parallelOutputTextFileName;
#endif
        public static void WriteText(string text)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = _defaultForegroundColor;
                Console.WriteLine(text);
            }
        }

        public static void WriteOutput(string text)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(text);
#if DEBUG
                if (_writeOutputToTextFileAsParallel)
                {
                    File.AppendAllLines(_parallelOutputTextFileName, new List<string>() { text });
                }
#endif
                Console.ForegroundColor = _defaultForegroundColor;
            }
        }

        public static void WriteError(string text)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ForegroundColor = _defaultForegroundColor;
            }
        }
    }
}

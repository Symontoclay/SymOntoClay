/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.CLI
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
        public static bool WriteLogChannelToTextFileAsParallel 
        { 
            get
            {
                lock (_lockObj)
                {
                    return _writeLogChannelToTextFileAsParallel;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    if(_writeLogChannelToTextFileAsParallel == value)
                    {
                        return;
                    }

                    _writeLogChannelToTextFileAsParallel = value;

                    if(_writeLogChannelToTextFileAsParallel)
                    {
                        var now = DateTime.Now;
                        _parallelLogChannelTextFileName = Path.Combine(Directory.GetCurrentDirectory(), $"parallelLogChannelTextFile_{now:ddMMyyyy-HHmmss}.log");
                    }
                }
            }
        }

        private static bool _writeLogChannelToTextFileAsParallel;
        private static string _parallelLogChannelTextFileName;
#endif
        public static void WriteText(string text)
        {
            lock(_lockObj)
            {
                Console.ForegroundColor = _defaultForegroundColor;
                Console.WriteLine(text);
            }
        }

        public static void WriteLogChannel(string text)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(text);
#if DEBUG
                if(_writeLogChannelToTextFileAsParallel)
                {
                    File.AppendAllLines(_parallelLogChannelTextFileName, new List<string>() { text });
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

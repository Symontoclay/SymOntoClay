/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

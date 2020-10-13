/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SymOntoClay.CLI
{
    public class CLIApp : IDisposable
    {
#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void Run(string[] args)
        {
            PrintHeader();

            var command = CLICommandParser.Parse(args);

            if(!command.IsValid)
            {
                PrintAboutWrongCommandLine(command);
                PrintHowToExitAndWait();
                return;
            }

            var kindOfComand = command.Kind;

            switch(kindOfComand)
            {
                case KindOfCLICommand.Run:
                    {
                        using var handler = new CLIRunHandler();
                        handler.Run(command);
                        return;
                    }

                case KindOfCLICommand.New:
                    {
                        var handler = new CLINewHandler();
                        handler.Run(command);
                        return;
                    }

                case KindOfCLICommand.Version:
                    {
                        ConsoleWrapper.WriteText(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        PrintHowToExitAndWait();
                        return;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfComand), kindOfComand, null);
            }
        }

        private void PrintHeader()
        {
            ConsoleWrapper.WriteText("Copyright © 2020 Sergiy Tolkachov aka metatypeman");
        }

        private void PrintHowToExitAndWait()
        {
            ConsoleWrapper.WriteText("Press any key for exit.");
            Console.ReadKey();
        }

        private void PrintAboutWrongCommandLine(CLICommand command)
        {
            ConsoleWrapper.WriteError("Unknown command!");
        }

        public void Dispose()
        {
        }
    }
}

/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if(!args.Any())
            {
                PrintHelp();
                PrintHowToExitAndWait();
                return;
            }

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
                case KindOfCLICommand.Help:
                    PrintHelp();
                    PrintHowToExitAndWait();
                    return;

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

        private void PrintHelp()
        {
            ConsoleWrapper.WriteText("Help - SymOntoClay CLI:");
            ConsoleWrapper.WriteText("");
            ConsoleWrapper.WriteText("Running CLI without arguments prints help.");
            ConsoleWrapper.WriteText("");
            ConsoleWrapper.WriteText("CLI arguments:");
            ConsoleWrapper.WriteText("h - prints help.");
            ConsoleWrapper.WriteText("help - prints help.");
            ConsoleWrapper.WriteText("run <NPC file name> - runs NPC and waits when you write 'exit' for exit.");
            ConsoleWrapper.WriteText("exit - ends running NPC.");
            ConsoleWrapper.WriteText("new <NPC name> - creates new NPC in new or existing worldspace. For creating NPC in existing worldspace runs command 'new' in worldspace directory.");
            ConsoleWrapper.WriteText("n - alias of 'new' command.");
            ConsoleWrapper.WriteText("version - prints current version of SymOntoClay.");
            ConsoleWrapper.WriteText("v - alias of 'version' command.");
        }

        public void Dispose()
        {
        }
    }
}

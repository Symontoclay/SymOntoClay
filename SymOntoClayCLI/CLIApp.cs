/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using Spectre.Console;
using SymOntoClay.CLI.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SymOntoClay.CLI
{
    public class CLIApp : IDisposable
    {
        public void Run(string[] args, AppSettings appSettings)
        {
            if(!args.Any())
            {
                PrintHeader();
                PrintHelp();
                PrintHowToExitAndWait();
                return;
            }

            var command = CLICommandParser.Parse(args);

            if (!command.IsValid)
            {
                PrintHeader();
                PrintAboutWrongCommandLine(command);
                PrintHowToExitAndWait();
                return;
            }

            if(!command.NoLogo)
            {
                PrintHeader();
            }

            var kindOfCommand = command.Kind;

            switch(kindOfCommand)
            {
                case KindOfCLICommand.Help:
                    PrintHelp();
                    if (command.Timeout.HasValue)
                    {
                        if (command.Timeout.Value > 0)
                        {
                            Thread.Sleep(command.Timeout.Value);
                        }
                    }
                    else
                    {
                        PrintHowToExitAndWait();
                    }
                    return;

                case KindOfCLICommand.Run:
                    {
                        using var handler = new CLIRunHandler(appSettings);
                        handler.Run(command);
                        return;
                    }

                case KindOfCLICommand.New:
                    {
                        var handler = new CLINewHandler();
                        handler.Run(command);
                        return;
                    }

                case KindOfCLICommand.NewWorlspace:
                    {
                        var handler = new CLINewWorldspaceHandler();
                        handler.Run(command);
                        return;
                    }

                case KindOfCLICommand.Version:
                    {
                        ConsoleWrapper.WriteText(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        if(command.Timeout.HasValue)
                        {
                            if(command.Timeout.Value > 0)
                            {
                                Thread.Sleep(command.Timeout.Value);
                            }                            
                        }
                        else
                        {
                            PrintHowToExitAndWait();
                        }
                        
                        return;
                    }

                case KindOfCLICommand.Install:
                    {
                        var handler = new CLIInstallHandler();
                        handler.Run(command);
                        return;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommand), kindOfCommand, "1B52E392-CBD4-47C3-8B9F-C687B31CA059");
            }
        }

        private void PrintHeader()
        {
            CLIHelper.ShowCopyrightSplash(2000);
        }

        private void PrintHowToExitAndWait()
        {
            AnsiConsole.MarkupLine("[grey]Press any key for exit.[/]");
            Console.ReadKey();
        }

        private void PrintAboutWrongCommandLine(CLICommand command)
        {
            ConsoleWrapper.WriteError("Unknown command!");
        }

        private void PrintHelp()
        {
            CLIHelper.PrintHelp();
        }

        public void Dispose()
        {
        }
    }
}

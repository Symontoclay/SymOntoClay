/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SymOntoClay.CLI
{
    public class CLIApp : IDisposable
    {
        public void Run(string[] args)
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

            var kindOfComand = command.Kind;

            switch(kindOfComand)
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
                    throw new ArgumentOutOfRangeException(nameof(kindOfComand), kindOfComand, null);
            }
        }

        private void PrintHeader()
        {
            ConsoleWrapper.WriteCopyright();
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
            ConsoleWrapper.WriteText("run - runs current NPC and waits when you write 'exit' for exit. The current NPC is detected by locators.");
            ConsoleWrapper.WriteText("exit - ends running NPC.");
            ConsoleWrapper.WriteText("new [-npc] <NPC name> - creates new NPC in new or existing worldspace. For creating NPC in existing worldspace runs command 'new' in worldspace directory.");
            ConsoleWrapper.WriteText("n - alias of 'new' command.");
            ConsoleWrapper.WriteText("new -thing <NPC name> - creates new Thing in new or existing worldspace. For creating NPC in existing worldspace runs command 'new' in worldspace directory.");
            ConsoleWrapper.WriteText("new -world <NPC name> - creates new worldspace.");
            ConsoleWrapper.WriteText("new -w <NPC name> - creates new worldspace.");
            ConsoleWrapper.WriteText("new -lib <Lib name> - creates new library.");
            ConsoleWrapper.WriteText("new -l <Lib name> - creates new library.");
            ConsoleWrapper.WriteText("new -nav <Nav object name> - creates new navigation object.");
            ConsoleWrapper.WriteText("new -player <Player name> - creates new player.");
            ConsoleWrapper.WriteText("new -p <Player name> - creates new player.");
            ConsoleWrapper.WriteText("install <Lib name> - installes library.");
            ConsoleWrapper.WriteText("version - prints current version of SymOntoClay.");
            ConsoleWrapper.WriteText("v - alias of 'version' command.");
            ConsoleWrapper.WriteText("");
            ConsoleWrapper.WriteText("Additional CLI arguments:");
            ConsoleWrapper.WriteText("-nologo - supresses printing header.");
            ConsoleWrapper.WriteText("-timeout <milleseconds> - terminates CLI after <milliseconds>. CLI will return 0.");
            ConsoleWrapper.WriteText("-nlp - enables NLP (natural language processing).");
        }

        public void Dispose()
        {
        }
    }
}

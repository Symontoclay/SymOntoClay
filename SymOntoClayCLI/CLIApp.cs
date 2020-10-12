﻿using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        private void PrintAboutWrongCommandLine(CLICommand command)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}

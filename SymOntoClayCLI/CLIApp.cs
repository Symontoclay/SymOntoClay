using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CLI
{
    public class CLIApp : IDisposable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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

#if DEBUG
            _logger.Info($"command = {command}");
#endif

            Console.ReadLine();
        }

        private void PrintHeader()
        {
            Console.WriteLine("Copyright © 2020 Sergiy Tolkachov aka metatypeman");
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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CLI
{
    public static class CLICommandParser
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static CLICommand Parse(string[] args)
        {
#if DEBUG
            //_logger.Info($"args = {JsonConvert.SerializeObject(args, Formatting.Indented)}");
#endif

            var firstParam = args[0].ToLower();

            switch(firstParam)
            {
                case "run":
                    return ParseRunCommand(args);

                default:
                    throw new ArgumentOutOfRangeException(nameof(firstParam), firstParam, null);
            }
        }

        private static CLICommand ParseRunCommand(string[] args)
        {
            var command = new CLICommand() { Kind = KindOfCLICommand.Run };

            switch(args.Length)
            {
                case 2:
                    {
                        var inputFile = args[1];

                        command.InputFile = EVPath.Normalize(inputFile);
                        command.IsValid = true;
                        return command;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(args.Length), args.Length, null);
            }
        }
    }
}

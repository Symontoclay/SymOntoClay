/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

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

                case "new":
                case "n":
                    return ParseNewCommand(args);

                case "version":
                case "v":
                    return new CLICommand() { Kind = KindOfCLICommand.Version, IsValid = true };

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

        private static CLICommand ParseNewCommand(string[] args)
        {
            var command = new CLICommand() { Kind = KindOfCLICommand.New };

            switch (args.Length)
            {
                case 2:
                    {
                        command.ProjectName = args[1];
                        command.IsValid = true;
                        return command;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(args.Length), args.Length, null);
            }
        }
    }
}

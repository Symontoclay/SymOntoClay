/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.CLI
{
    public static class CLICommandParser
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static CLICommand Parse(string[] args)
        {
#if DEBUG
            //_logger.Info($"args = {JsonConvert.SerializeObject(args, Formatting.Indented)}");
#endif

            var firstParam = args[0].ToLower();

            switch(firstParam)
            {
                case "h":
                case "help":
                    return new CLICommand() { Kind = KindOfCLICommand.Help, IsValid = true };

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
                case 1:
                    command.InputDir = Directory.GetCurrentDirectory();
                    command.IsValid = true;
                    return command;

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

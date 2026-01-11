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

using SymOntoClay.CLI.Helpers.CommandLineParsing;
using SymOntoClay.CLI.Helpers.CommandLineParsing.Options;
using SymOntoClay.CLI.Helpers.CommandLineParsing.Options.TypeCheckers;
using System.Collections.Generic;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileBuilderAppCommandLineParser: CommandLineParser
    {
        public LogFileBuilderAppCommandLineParser(bool initWithoutExceptions)
            : base(new List<BaseCommandLineArgument>()
            {
                new CommandLineMutuallyExclusiveSet()
                {
                    IsRequired = true,
                    SubItems = new List<BaseCommandLineArgument>
                    {
                        new CommandLineArgument()
                        {
                            Name = "--help",
                            Aliases = new List<string>
                            {
                                "h",
                                "-h",
                                "--h",
                                "-help",
                                "help"
                            },
                            Kind = KindOfCommandLineArgument.Flag,
                            UseIfCommandLineIsEmpty = true
                        },
                        new CommandLineGroup()
                        {
                            SubItems = new List<BaseCommandLineArgument>
                            {
                                new CommandLineArgument()
                                {
                                    Name = "--input",
                                    Aliases = new List<string>()
                                    {
                                        "--i",
                                        "-input",
                                        "-i"
                                    },
                                    Kind = KindOfCommandLineArgument.SingleValue,
                                    Index = 0
                                },
                                new CommandLineArgument
                                {
                                    Name = "--output",
                                    Aliases = new List<string>
                                    {
                                        "--o",
                                        "-output",
                                        "-o"
                                    },
                                    Kind = KindOfCommandLineArgument.SingleValue,
                                    Index = 1
                                },
                                new CommandLineArgument
                                {
                                    Name = "--nologo",
                                    Aliases = new List<string>
                                    {
                                        "-nologo"
                                    },
                                    Kind = KindOfCommandLineArgument.Flag
                                },
                                new CommandLineArgument
                                {
                                    Name = "--target-nodeid",
                                    Aliases = new List<string>
                                    {
                                        "-target-nodeid"
                                    },
                                    Kind = KindOfCommandLineArgument.SingleValue
                                },
                                new CommandLineArgument
                                {
                                    Name = "--target-threadid",
                                    Aliases = new List<string>
                                    {
                                        "-target-threadid"
                                    },
                                    Kind = KindOfCommandLineArgument.SingleValue
                                },
                                new CommandLineArgument
                                {
                                    Name = "--split-by-nodes",
                                    Aliases = new List<string>
                                    {
                                        "-split-by-nodes"
                                    },
                                    Kind = KindOfCommandLineArgument.Flag
                                },
                                new CommandLineArgument
                                {
                                    Name = "--split-by-threads",
                                    Aliases = new List<string>
                                    {
                                        "-split-by-threads"
                                    },
                                    Kind = KindOfCommandLineArgument.Flag
                                },
                                new CommandLineArgument
                                {
                                    Name = "--configuration",
                                    Aliases = new List<string>
                                    {
                                        "--c",
                                        "--cfg",
                                        "--config",
                                        "-configuration",
                                        "-c",
                                        "-cfg",
                                        "-config"
                                    },
                                    Kind = KindOfCommandLineArgument.SingleValue
                                },
                                new CommandLineArgument
                                {
                                    Name = "--html",
                                    Aliases = new List<string>
                                    {
                                        "-html"
                                    },
                                    Kind = KindOfCommandLineArgument.Flag
                                },
                                new CommandLineArgument
                                {
                                    Name = "--abs-url",
                                    Aliases = new List<string>
                                    {
                                        "-abs-url"
                                    },
                                    Kind = KindOfCommandLineArgument.Flag,
                                    Requires = new List<string>
                                    {
                                        "--html"
                                    }
                                },
                                new CommandLineArgument
                                {
                                    Name = "--mode",
                                    Kind = KindOfCommandLineArgument.SingleValue,
                                    TypeChecker = new EnumChecker<LogFileBuilderMode>()
                                }
                            }
                        }
                    }
                }
            }, initWithoutExceptions)
        {
        }
    }
}

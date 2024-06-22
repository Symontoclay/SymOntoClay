using SymOntoClay.CLI.Helpers.CommandLineParsing;
using SymOntoClay.CLI.Helpers.CommandLineParsing.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

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

using NLog;
using SymOntoClay.CLI.Helpers;
using SymOntoClay.CLI.Helpers.CommandLineParsing;
using SymOntoClay.CLI.Helpers.CommandLineParsing.Options;
using SymOntoClay.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileBuilderApp
    {
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void Run(string[] args, string defaultConfigurationFileName)
        {
#if DEBUG
            //_logger.Info($"args = {args.WritePODListToString()}");
            //_logger.Info($"defaultConfigurationFileName = {defaultConfigurationFileName}");
#endif

            var defaultConfiguration = string.IsNullOrWhiteSpace(defaultConfigurationFileName) ? null : InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(EVPath.Normalize(defaultConfigurationFileName));

            Run(args, defaultConfiguration);
        }

        public void Run(string[] args, LogFileCreatorInheritableOptions defaultConfiguration)
        {
#if DEBUG
            //_logger.Info($"args = {args.WritePODListToString()}");
            //_logger.Info($"defaultConfiguration = {defaultConfiguration}");
#endif

            var logFileBuilderOptions = ParseArgs(args);

#if DEBUG
            //_logger.Info($"logFileBuilderOptions = {JsonConvert.SerializeObject(logFileBuilderOptions, Formatting.Indented)}");
#endif

            if(logFileBuilderOptions.IsHelp)
            {
                throw new NotImplementedException();
            }

            if(!logFileBuilderOptions.NoLogo)
            {
                PrintHeader();
            }

            if(string.IsNullOrWhiteSpace(logFileBuilderOptions.Output))
            {
                logFileBuilderOptions.Output = Directory.GetCurrentDirectory();
            }

            if (string.IsNullOrWhiteSpace(logFileBuilderOptions.Input))
            {
                logFileBuilderOptions.Input = Directory.GetCurrentDirectory();
            }

#if DEBUG
            //_logger.Info($"logFileBuilderOptions (2) = {JsonConvert.SerializeObject(logFileBuilderOptions, Formatting.Indented)}");
#endif

            var options = LoadOptions(defaultConfiguration, logFileBuilderOptions.ConfigurationFileName);

#if DEBUG
            //_logger.Info($"options = {options}");
#endif

            if (logFileBuilderOptions.Input != null)
            {
                options.SourceDirectoryName = logFileBuilderOptions.Input;
            }

            if (logFileBuilderOptions.Output != null)
            {
                options.OutputDirectory = logFileBuilderOptions.Output;
            }

            if(logFileBuilderOptions.TargetNodeId != null)
            {
                options.TargetNodes = new List<string>() { logFileBuilderOptions.TargetNodeId };
            }

            if (logFileBuilderOptions.TargetThreadId != null)
            {
                options.TargetThreads = new List<string>() { logFileBuilderOptions.TargetThreadId };
            }

            if (logFileBuilderOptions.SplitByNodes.HasValue)
            {
                options.SeparateOutputByNodes = logFileBuilderOptions.SplitByNodes;
            }

            if (logFileBuilderOptions.SplitByThreads.HasValue)
            {
                options.SeparateOutputByThreads = logFileBuilderOptions.SplitByThreads;
            }

            if (logFileBuilderOptions.ToHtml.HasValue)
            {
                options.ToHtml = logFileBuilderOptions.ToHtml;
            }

            if (logFileBuilderOptions.DotAppPath != null)
            {
                options.DotAppPath = logFileBuilderOptions.DotAppPath;
            }

            if (logFileBuilderOptions.IsAbsUrl.HasValue)
            {
                options.IsAbsUrl = logFileBuilderOptions.IsAbsUrl;
            }

#if DEBUG
            //_logger.Info($"options (2) = {options}");
#endif

            LogFileCreator.Run(options, _logger);
        }

        private LogFileBuilderOptions ParseArgs(string[] args)
        {
            var parser = CreateAndInitParser();

            var parsedArgs = parser.Parse(args);

#if DEBUG
            //_logger.Info($"parsedArgs = {JsonConvert.SerializeObject(parsedArgs, Formatting.Indented)}");
#endif

            var logFileBuilderOptions = new LogFileBuilderOptions()
            {
                IsHelp = (parsedArgs.TryGetValue("--help", out var isHelp) ? (bool)isHelp : default(bool)),
                Input = (parsedArgs.TryGetValue("--input", out var input) ? (string)input : default(string)),
                Output = (parsedArgs.TryGetValue("--output", out var output) ? (string)output : default(string)),
                NoLogo = (parsedArgs.TryGetValue("--nologo", out var nologo) ? (bool)nologo : default(bool)),
                TargetNodeId = (parsedArgs.TryGetValue("--target-nodeid", out var targetNodeId) ? (string)targetNodeId : default(string)),
                TargetThreadId = (parsedArgs.TryGetValue("--target-threadid", out var targetThreadId) ? (string)targetThreadId : default(string)),
                SplitByNodes = (parsedArgs.TryGetValue("--split-by-nodes", out var splitByNodes) ? (bool)splitByNodes : null),
                SplitByThreads = (parsedArgs.TryGetValue("--split-by-threads", out var splitByThreads) ? (bool)splitByThreads : null),
                ConfigurationFileName = (parsedArgs.TryGetValue("--configuration", out var configurationFileName) ? (string)configurationFileName : default(string)),
                ToHtml = (parsedArgs.TryGetValue("--html", out var toHtml) ? (bool)toHtml : null),
                IsAbsUrl = (parsedArgs.TryGetValue("--abs-url", out var isAbsUrl) ? (bool)isAbsUrl : null)
            };

            return logFileBuilderOptions;
        }

        private CommandLineParser CreateAndInitParser()
        {
            var parser = new CommandLineParser();

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--help",
                Aliases = new List<string>
                {
                    "--?",
                    "--h"
                },
                Kind = KindOfCommandLineArgument.Flag
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--input",
                Aliases = new List<string>
                {
                    "--i"
                },
                Kind = KindOfCommandLineArgument.SingleValue,
                IsDefault = true
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--output",
                Aliases = new List<string>
                {
                    "--o"
                },
                Kind = KindOfCommandLineArgument.SingleValue
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--nologo",
                Kind = KindOfCommandLineArgument.Flag
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--target-nodeid",
                Kind = KindOfCommandLineArgument.SingleValue
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--target-threadid",
                Kind = KindOfCommandLineArgument.SingleValue
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--split-by-nodes",
                Kind = KindOfCommandLineArgument.Flag
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--split-by-threads",
                Kind = KindOfCommandLineArgument.Flag
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--configuration",
                Aliases = new List<string>
                {
                    "--c",
                    "--cfg",
                    "--config"
                },
                Kind = KindOfCommandLineArgument.SingleValue
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--html",
                Kind = KindOfCommandLineArgument.Flag
            });

            parser.RegisterArgument(new CommandLineArgumentOptions
            {
                Name = "--abs-url",
                Kind = KindOfCommandLineArgument.Flag
            });

            return parser;
        }

        private void PrintHeader()
        {
            ConsoleWrapper.WriteText($"Copyright © 2020 - {DateTime.Today.Year:####} Sergiy Tolkachov aka metatypeman");
        }

        private void PrintHelp()
        {
            throw new NotImplementedException();
        }

        private LogFileCreatorOptions LoadOptions(LogFileCreatorInheritableOptions defaultConfiguration, string configurationFileName)
        {
#if DEBUG
            //_logger.Info($"defaultConfiguration = {defaultConfiguration}");
            //_logger.Info($"configurationFileName = {configurationFileName}");
#endif
            
            if(defaultConfiguration == null)
            {
                if(string.IsNullOrWhiteSpace(configurationFileName))
                {
                    return LogFileCreatorInheritableOptions.DefaultOptions;
                }
                else
                {
                    return InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(configurationFileName);
                }
            }

            if(string.IsNullOrWhiteSpace(configurationFileName))
            {
                return defaultConfiguration;
            }

            return InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(configurationFileName, defaultConfiguration);
        }
    }
}

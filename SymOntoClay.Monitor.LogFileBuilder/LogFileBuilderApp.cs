﻿using Newtonsoft.Json;
using NLog;
using SymOntoClay.CLI.Helpers;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _logger.Info($"args = {args.WritePODListToString()}");
            _logger.Info($"defaultConfigurationFileName = {defaultConfigurationFileName}");
#endif

            var defaultConfiguration = string.IsNullOrWhiteSpace(defaultConfigurationFileName) ? null : InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(EVPath.Normalize(defaultConfigurationFileName));

            Run(args, defaultConfiguration);
        }

        public void Run(string[] args, LogFileCreatorInheritableOptions defaultConfiguration)
        {
#if DEBUG
            _logger.Info($"args = {args.WritePODListToString()}");
            _logger.Info($"defaultConfiguration = {defaultConfiguration}");
#endif

            var logFileBuilderOptions = ParseArgs(args);

#if DEBUG
            _logger.Info($"logFileBuilderOptions = {JsonConvert.SerializeObject(logFileBuilderOptions, Formatting.Indented)}");
#endif

            if(logFileBuilderOptions.IsHelp)
            {
                throw new NotImplementedException();
            }

            if(string.IsNullOrWhiteSpace(logFileBuilderOptions.Output))
            {
                logFileBuilderOptions.Output = Directory.GetCurrentDirectory();
            }

            if (string.IsNullOrWhiteSpace(logFileBuilderOptions.Input))
            {
                logFileBuilderOptions.Input = Directory.GetCurrentDirectory();
            }

            throw new NotImplementedException();
        }

        private LogFileBuilderOptions ParseArgs(string[] args)
        {
            var parser = CreateAndInitParser();

            var parsedArgs = parser.Parse(args);

#if DEBUG
            _logger.Info($"parsedArgs = {JsonConvert.SerializeObject(parsedArgs, Formatting.Indented)}");
#endif

            var logFileBuilderOptions = new LogFileBuilderOptions()
            {
                IsHelp = (parsedArgs.TryGetValue("--help", out var isHelp) ? (bool)isHelp : default(bool)),
                Input = (parsedArgs.TryGetValue("--input", out var input) ? (string)input : default(string)),
                Output = (parsedArgs.TryGetValue("--output", out var output) ? (string)output : default(string)),
                NoLogo = (parsedArgs.TryGetValue("--nologo", out var nologo) ? (bool)nologo : default(bool)),
                TargetNodeId = (parsedArgs.TryGetValue("--target-nodeid", out var targetNodeId) ? (string)targetNodeId : default(string)),
                TargetThreadId = (parsedArgs.TryGetValue("--target-threadid", out var targetThreadId) ? (string)targetThreadId : default(string)),
                SplitByNodes = (parsedArgs.TryGetValue("--split-by-nodes", out var splitByNodes) ? (bool)splitByNodes : default(bool)),
                SplitByThreads = (parsedArgs.TryGetValue("--split-by-threads", out var splitByThreads) ? (bool)splitByThreads : default(bool)),
                ConfigurationFileName = (parsedArgs.TryGetValue("--configuration", out var configurationFileName) ? (string)configurationFileName : default(string))
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
    }
}

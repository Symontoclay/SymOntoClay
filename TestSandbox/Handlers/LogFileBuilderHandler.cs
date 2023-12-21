using NLog;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems;
using SymOntoClay.Monitor.LogFileBuilder;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
using SymOntoClay.CoreHelper.DebugHelpers;
using Newtonsoft.Json;
using SymOntoClay.CLI.Helpers;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace TestSandbox.Handlers
{
    public class LogFileBuilderHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case13();
            //Case12();
            //Case11();
            //Case10();
            //Case9();
            //Case8a();
            //Case8();
            //Case7();
            //Case6();
            //Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case13()
        {
            RunLogFileBuilderProgramMain(@"--i c:\Users\Acer\AppData\Roaming\SymOntoClayAsset\NpcLogMessages\2023_12_21_21_30_06\ --o c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesLogsOutputDir\ --target-nodeid #BlueSoldier");
        }

        private void Case12()
        {
            RunLogFileBuilderProgramMain(string.Empty);
        }

        private void Case11()
        {
            var parser = CreateAndInitParser();

            var cmdArg = @"--i c:\Users\sergiy.tolkachov\source\repos\ --nologo";

            _logger.Info($"cmdArg = '{cmdArg}'");

            var cmdStrList = cmdArg.Split(' ');

            _logger.Info($"cmdStrList = '{cmdStrList.WritePODListToString()}'");

            var parsedCmd = parser.Parse(cmdStrList);

            _logger.Info($"parsedCmd = {JsonConvert.SerializeObject(parsedCmd, Formatting.Indented)}");

            var logFileBuilderOptions = new LogFileBuilderOptions()
            {
                IsHelp = (parsedCmd.TryGetValue("--help", out var isHelp) ? (bool)isHelp: default(bool)),
                Input = (parsedCmd.TryGetValue("--input", out var input) ? (string)input : default(string)),
                Output = (parsedCmd.TryGetValue("--output", out var output) ? (string)output : default(string)),
                NoLogo = (parsedCmd.TryGetValue("--nologo", out var nologo) ? (bool)nologo : default(bool)),
                TargetNodeId = (parsedCmd.TryGetValue("--target-nodeid", out var targetNodeId) ? (string)targetNodeId : default(string)),
                TargetThreadId = (parsedCmd.TryGetValue("--target-threadid", out var targetThreadId) ? (string)targetThreadId : default(string)),
                SplitByNodes = (parsedCmd.TryGetValue("--split-by-nodes", out var splitByNodes) ? (bool)splitByNodes : null),
                SplitByThreads = (parsedCmd.TryGetValue("--split-by-threads", out var splitByThreads) ? (bool)splitByThreads : null),
                ConfigurationFileName = (parsedCmd.TryGetValue("--configuration", out var configurationFileName) ? (string)configurationFileName : default(string))
            };

            _logger.Info($"logFileBuilderOptions = {JsonConvert.SerializeObject(logFileBuilderOptions, Formatting.Indented)}");
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

        private void Case10()
        {
            var cmdArg = @"--i c:\Users\sergiy.tolkachov\source\repos\ --nologo";

            _logger.Info($"cmdArg = '{cmdArg}'");

            var cmdStrList = cmdArg.Split(' ');

            _logger.Info($"cmdStrList = '{cmdStrList.WritePODListToString()}'");

            var parser = new CommandLineParser();
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
                Name = "--nologo",
                Kind = KindOfCommandLineArgument.Flag
            });

            var result = parser.Parse(cmdStrList);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");
        }

        private void Case9()
        {
            RunLogFileBuilderProgramMain(@"--i c:\Users\sergiy.tolkachov\source\repos\ --nologo");
        }

        private void RunLogFileBuilderProgramMain(string args)
        {
            _logger.Info($"args = '{args}'");

            var cmdStrList = args.Split(' ');

            _logger.Info($"cmdStrList = {cmdStrList.WritePODListToString()}");

            SymOntoClay.Monitor.LogFileBuilder.Program.Main(cmdStrList.ToArray());
        }

        private void Case8a()
        {
            var defaultConfigFileName = Path.Combine(Directory.GetCurrentDirectory(), "default-LogFileCreatorOptions.json");

            _logger.Info($"defaultConfigFileName = {defaultConfigFileName}");

            var defaultCfg = InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(defaultConfigFileName);

            _logger.Info($"defaultCfg = {defaultCfg}");

            var configFileName = Path.Combine(Directory.GetCurrentDirectory(), "logFileCreatorOptions.json");

            _logger.Info($"configFileName = {configFileName}");

            var cfg = InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(configFileName, defaultCfg);

            _logger.Info($"cfg = {cfg}");
        }

        private void Case8()
        {
            var configFileName = Path.Combine(Directory.GetCurrentDirectory(), "logFileCreatorOptions.json");

            _logger.Info($"configFileName = {configFileName}");

            var cfg = InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(configFileName);

            _logger.Info($"cfg = {cfg}");
        }

        private void Case7()
        {
            var cfg = new LogFileCreatorInheritableOptions();

            //cfg.Write(LogFileCreatorOptions.DefaultOptions);

            var sourceDirectoryName = @"c:\Users\sergiy.tolkachov\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_01_11_38_52\";

            _logger.Info($"sourceDirectoryName = {sourceDirectoryName}");

            cfg.ParentCfg = "parent-LogFileCreatorOptions.json";

            var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tst_logs");

            var sourceOptions = new LogFileCreatorOptions()
            {
                TargetNodes = new List<string>
                {
                    "soldier 1"
                },
                TargetThreads = new List<string>
                {
                    "f5f7ed91-77e5-45f5-88f5-b7530d111bd5"
                },
                SourceDirectoryName = sourceDirectoryName,
                OutputDirectory = logsOutputDirectory
            };

            _logger.Info($"sourceOptions = {sourceOptions}");

            cfg.Write(sourceOptions);

            _logger.Info($"cfg = {cfg}");

            //var fileName = Path.Combine(Directory.GetCurrentDirectory(), "parent-LogFileCreatorOptions.json");
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "logFileCreatorOptions.json");

            _logger.Info($"fileName = {fileName}");

            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            File.WriteAllText(fileName, JsonConvert.SerializeObject(cfg, Formatting.Indented, jsonSerializerSettings));
        }

        private void Case6()
        {
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_02_19_58_24\soldier 1\";
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_08_16_34_10\soldier 1\";
            var sourceDirectoryName = @"c:\Users\sergiy.tolkachov\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_01_11_38_52\";

            _logger.Info($"sourceDirectoryName = {sourceDirectoryName}");

            var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tst_logs");

            Directory.CreateDirectory(logsOutputDirectory);

            var options = LogFileCreatorOptions.DefaultOptions;

            _logger.Info($"options = {options}");

            var sourceOptions = new LogFileCreatorOptions()
            {
                TargetNodes = new List<string>
                {
                    "soldier 1"
                },
                TargetThreads = new List<string>
                {
                    "f5f7ed91-77e5-45f5-88f5-b7530d111bd5"
                },
                SourceDirectoryName = sourceDirectoryName,
                OutputDirectory = logsOutputDirectory
            };

            _logger.Info($"sourceOptions = {sourceOptions}");

            options.Write(sourceOptions);

            _logger.Info($"options (2) = {options}");
        }

        private void Case5()
        {
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_02_19_58_24\soldier 1\";
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_08_16_34_10\soldier 1\";
            var sourceDirectoryName = @"c:\Users\sergiy.tolkachov\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_01_11_38_52\";

            _logger.Info($"sourceDirectoryName = {sourceDirectoryName}");

            var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tst_logs");

            Directory.CreateDirectory(logsOutputDirectory);

            var options = new LogFileCreatorOptions()
            {
                FileNameTemplate = new List<BaseFileNameTemplateOptionItem>()
                {
                    new NodeIdFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = "_",
                        IfNodeIdExists = true
                    },
                    new ThreadIdFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = "_",
                        IfThreadIdExists = true
                    },
                    new LongDateTimeFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = ".log"
                    }
                },
                SeparateOutputByNodes = true,
                SeparateOutputByThreads = false,
                KindOfMessages = new List<KindOfMessage>()
                {
                    //KindOfMessage.Info
                },
                Layout = new List<BaseMessageTextRowOptionItem>
                {
                    new LongDateTimeStampTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new MessagePointIdTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new ClassFullNameTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new MemberNameTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new ThreadIdTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new KindOfMessageTextRowOptionItem
                    {
                        TextTransformation = TextTransformations.UpperCase
                    },
                    new SpaceTextRowOptionItem(),
                    new MessageContentTextRowOptionItem
                    {
                        EnableCallMethodIdOfMethodLabel = true,
                        EnableMethodSignatureArguments = true,
                        EnableTypesListOfMethodSignatureArguments = true,
                        EnableDefaultValueOfMethodSignatureArguments = true,
                        EnablePassedVauesOfMethodLabel = true,
                        EnableChainOfProcessInfo = true
                    }
                }
            };

            _logger.Info($"options = {options}");

            var sourceOptions = new LogFileCreatorOptions()
            {
                TargetNodes = new List<string>
                {
                    "soldier 1"
                },
                TargetThreads = new List<string>
                {
                    "f5f7ed91-77e5-45f5-88f5-b7530d111bd5"
                },
                SourceDirectoryName = sourceDirectoryName,
                OutputDirectory = logsOutputDirectory
            };

            _logger.Info($"sourceOptions = {sourceOptions}");

            options.Write(sourceOptions);

            _logger.Info($"options (2) = {options}");
        }

        private void Case4()
        {
            var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tst_logs");

            Directory.CreateDirectory(logsOutputDirectory);

            var options = new FileStreamsStorageOptions()
            {
                OutputDirectory = logsOutputDirectory,
                FileNameTemplate = new List<BaseFileNameTemplateOptionItem>()
                {
                    new NodeIdFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = "_",
                        IfNodeIdExists = true
                    },
                    new ThreadIdFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = "_",
                        IfThreadIdExists = true
                    },
                    new LongDateTimeFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = ".log"
                    }
                },
                SeparateOutputByNodes = true,
                SeparateOutputByThreads = true
            };

            _logger.Info($"options = {options}");

            using var fileStreamsStorage = new FileStreamsStorage(options);

            var sw = fileStreamsStorage.GetStreamWriter(string.Empty, string.Empty);
            sw.WriteLine("Hello world!");

            var sw1 = fileStreamsStorage.GetStreamWriter("1", string.Empty);
            sw1.WriteLine("Hi!");

            var sw2 = fileStreamsStorage.GetStreamWriter("1", "1");
            sw2.WriteLine("The Beatles!");

            var sw3 = fileStreamsStorage.GetStreamWriter(string.Empty, "1");
            sw3.WriteLine("Ups!");
        }

        private void Case3()
        {
            var fileNameTemplate = new List<BaseFileNameTemplateOptionItem>()
            {
                new NodeIdFileNameTemplateOptionItem(),
                new TextFileNameTemplateOptionItem()
                {
                    Text = "_",
                    IfNodeIdExists = true
                },
                new ThreadIdFileNameTemplateOptionItem(),
                new TextFileNameTemplateOptionItem()
                {
                    Text = "_",
                    IfThreadIdExists = true
                },
                new LongDateTimeFileNameTemplateOptionItem(),
                new TextFileNameTemplateOptionItem()
                {
                    Text = ".log"
                }
            };

            _logger.Info($"fileNameTemplate = {fileNameTemplate.WriteListToString()}");

            var nodeId = "1";
            var threadId = "2";

            var sb = new StringBuilder();

            foreach (var item in fileNameTemplate)
            {
                sb.Append(item.GetText(nodeId, threadId));

                //_logger.Info($"sb = {sb}");
            }

            _logger.Info($"sb = {sb}");
        }

        private void Case2()
        {
            var fileNameTemplate = new List<BaseFileNameTemplateOptionItem>()
            {
                new NodeIdFileNameTemplateOptionItem(),
                new ThreadIdFileNameTemplateOptionItem(),
                new SpaceTextFileNameTemplateOptionItem(),
                new LongDateTimeFileNameTemplateOptionItem(),
                new SpaceTextFileNameTemplateOptionItem(),
                new ShortDateTimeFileNameTemplateOptionItem(),
                new TextFileNameTemplateOptionItem()
                {
                    Text = "Hello world!",
                    IfNodeIdExists = true
                }
            };

            _logger.Info($"fileNameTemplate = {fileNameTemplate.WriteListToString()}");

            var nodeId = string.Empty;
            var threadId = string.Empty;

            var sb = new StringBuilder();

            foreach(var item in fileNameTemplate)
            {
                sb.Append(item.GetText(nodeId, threadId));

                //_logger.Info($"sb = {sb}");
            }

            _logger.Info($"sb = {sb}");
        }

        private void Case1()
        {
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_02_19_58_24\soldier 1\";
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_08_16_34_10\soldier 1\";
            var sourceDirectoryName = @"c:\Users\sergiy.tolkachov\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_01_11_38_52\";

            _logger.Info($"sourceDirectoryName = {sourceDirectoryName}");

            var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tst_logs");

            Directory.CreateDirectory(logsOutputDirectory);

            var options = new LogFileCreatorOptions()
            {
                TargetNodes = new List<string>
                {
                    "soldier 1"
                },
                TargetThreads = new List<string>
                {
                    "f5f7ed91-77e5-45f5-88f5-b7530d111bd5"
                },
                SourceDirectoryName = sourceDirectoryName,
                OutputDirectory = logsOutputDirectory,
                FileNameTemplate = new List<BaseFileNameTemplateOptionItem>()
                {
                    new NodeIdFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = "_",
                        IfNodeIdExists = true
                    },
                    new ThreadIdFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = "_",
                        IfThreadIdExists = true
                    },
                    new LongDateTimeFileNameTemplateOptionItem(),
                    new TextFileNameTemplateOptionItem()
                    {
                        Text = ".log"
                    }
                },
                SeparateOutputByNodes = true,
                SeparateOutputByThreads = false,
                KindOfMessages = new List<KindOfMessage>()
                {
                    //KindOfMessage.Info
                },
                Layout = new List<BaseMessageTextRowOptionItem>
                {
                    new LongDateTimeStampTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new MessagePointIdTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new ThreadIdTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new ClassFullNameTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new MemberNameTextRowOptionItem(),
                    new SpaceTextRowOptionItem(),
                    new KindOfMessageTextRowOptionItem
                    {
                        TextTransformation = TextTransformations.UpperCase
                    },
                    new SpaceTextRowOptionItem(),
                    new MessageContentTextRowOptionItem
                    {
                        EnableCallMethodIdOfMethodLabel = true,
                        EnableMethodSignatureArguments = true,
                        EnableTypesListOfMethodSignatureArguments = true,
                        EnableDefaultValueOfMethodSignatureArguments = true,
                        EnablePassedVauesOfMethodLabel = true,
                        EnableChainOfProcessInfo = true
                    }
                }
            };

            _logger.Info($"options = {options}");

            LogFileCreator.Run(options);
        }
    }
}

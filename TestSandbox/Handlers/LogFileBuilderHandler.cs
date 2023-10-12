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

namespace TestSandbox.Handlers
{
    public class LogFileBuilderHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case5()
        {
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_02_19_58_24\soldier 1\";
            //var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_08_16_34_10\soldier 1\";
            var sourceDirectoryName = @"c:\Users\sergiy.tolkachov\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_01_11_38_52\";

            _logger.Info($"sourceDirectoryName = {sourceDirectoryName}");

            var logFileName = Path.Combine(Directory.GetCurrentDirectory(), "mylog.txt");

            _logger.Info($"logFileName = {logFileName}");

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
                    new MessageContentTextRowOptionItem()
                }
            };

            _logger.Info($"options = {options}");

            var sourceOptions = new LogFileCreatorOptions()
            {
                SourceDirectoryName = sourceDirectoryName,
                OutputFileName = logFileName,
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

            var logFileName = Path.Combine(Directory.GetCurrentDirectory(), "mylog.txt");

            _logger.Info($"logFileName = {logFileName}");

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
                OutputFileName = logFileName,
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
                    new MessageContentTextRowOptionItem()
                }
            };

            _logger.Info($"options = {options}");

            LogFileCreator.Run(options);
        }
    }
}

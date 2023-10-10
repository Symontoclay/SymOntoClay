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

            Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case3()
        {

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
            var sourceDirectoryName = @"c:\Users\Acer\source\repos\SymOntoClay\TestSandbox\bin\Debug\net7.0\MessagesDir\2023_09_08_16_34_10\soldier 1\";

            _logger.Info($"sourceDirectoryName = {sourceDirectoryName}");

            var logFileName = Path.Combine(Directory.GetCurrentDirectory(), "mylog.txt");

            _logger.Info($"logFileName = {logFileName}");

            var options = new LogFileCreatorOptions()
            {
                SourceDirectoryName = sourceDirectoryName,
                OutputFileName = logFileName,
                OutputDirectory = Directory.GetCurrentDirectory(),
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

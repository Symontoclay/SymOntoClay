using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public static class StatLogFileCreator
    {
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static void Run(LogFileCreatorOptions options, ILogger logger)
        {
#if DEBUG
            //_logger.Info($"options = {options}");
#endif

            options = options.Clone();

            LogFileCreatorOptionsHelper.PrepareOptions(options, logger);

#if DEBUG
            //_logger.Info($"options (after) = {options}");
#endif

            var toHtml = options.ToHtml ?? false;

            var logFileCreatorContext = new LogFileCreatorContext(
                dotAppPath: options.DotAppPath,
                outputDirectory: options.OutputDirectory,
                toHtml: toHtml,
                isAbsUrl: options.IsAbsUrl ?? false,
                fileNameTemplate: options.FileNameTemplate);

            var fileStreamsStorageOptions = new FileStreamsStorageOptions()
            {
                OutputDirectory = options.OutputDirectory,
                FileNameTemplate = options.FileNameTemplate,
                SeparateOutputByNodes = options.SeparateOutputByNodes ?? false,
                SeparateOutputByThreads = options.SeparateOutputByThreads ?? false,
                ToHtml = toHtml,
                LogFileCreatorContext = logFileCreatorContext
            };

#if DEBUG
            //_logger.Info($"fileStreamsStorageOptions = {fileStreamsStorageOptions}");
#endif

            var showStages = (!(options.Silent ?? false)) && (logger != null);

#if DEBUG
            //_logger.Info($"showStages = {showStages}");
#endif

            var mode = options.Mode;

            if (showStages)
            {
                switch(mode)
                {
                    case LogFileBuilderMode.Stat:
                        logger.Info("It may take about a minute.");
                        break;

                    case LogFileBuilderMode.StatAndFiles:
                        logger.Info("It may take from few minutes to about 15 minutes.");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }

                logger.Info("Fetching file names");
            }

            using var fileStreamsStorage = new FileStreamsStorage(fileStreamsStorageOptions);

            var outputSw = fileStreamsStorage.GetStreamWriter("Stat", string.Empty);

            var now = DateTime.Now;

            outputSw.WriteLine(logFileCreatorContext.BeginParagraph());
            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine("Statistic:"));
            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine(now.ToString(LogFileBuilderConstants.LongDateTimeFormat)));
            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Source: '{options.SourceDirectoryName}'"));            
            outputSw.WriteLine(logFileCreatorContext.EndParagraph());

            var fileNamesList = MessageFilesReader.GetFileNames(options.SourceDirectoryName, null, null, null);

#if DEBUG
            //_logger.Info($"fileNamesList.Count = {fileNamesList.Count}");
#endif

            if (showStages)
            {
                logger.Info($"Fetched {fileNamesList.Count} file names");
            }

            var fileNamesDictByNodes = fileNamesList.GroupBy(p => p.Item1.NodeId).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
            //_logger.Info($"fileNamesDictByNodes.Count = {fileNamesDictByNodes.Count}");
#endif

            outputSw.WriteLine(logFileCreatorContext.BeginParagraph());
            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Messages: {fileNamesList.Count}"));
            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Nodes: {fileNamesDictByNodes.Count}"));
            outputSw.WriteLine(logFileCreatorContext.EndParagraph());

            outputSw.WriteLine(logFileCreatorContext.LineSeparator());

            foreach (var fileNamesByNodesKvpItem in fileNamesDictByNodes.OrderByDescending(p => p.Value.Count))
            {
#if DEBUG
                //_logger.Info($"fileNamesByNodesKvpItem.Key = {fileNamesByNodesKvpItem.Key}");
                //_logger.Info($"fileNamesByNodesKvpItem.Value.Count = {fileNamesByNodesKvpItem.Value.Count}");
                //_logger.Info($"fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal) = {fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal)}");
#endif

                var nodeId = fileNamesByNodesKvpItem.Key;
                var itemFileNamesList = fileNamesByNodesKvpItem.Value;

                (string AbsoluteName, string RelativeName) itemLogFileName = (string.Empty, string.Empty);

                switch(mode)
                {
                    case LogFileBuilderMode.Stat:
                        break;

                    case LogFileBuilderMode.StatAndFiles:
                        {
                            var itemOptions = options.Clone();
                            itemOptions.Mode = LogFileBuilderMode.LogFile;
                            itemOptions.TargetNodes = new List<string> { nodeId };

#if DEBUG
                            //_logger.Info($"itemOptions = {itemOptions}");
#endif

                            LogFileCreator.RunWithPreparedOptions(itemOptions, logger, fileStreamsStorage, logFileCreatorContext, itemFileNamesList);

                            itemLogFileName = fileStreamsStorage.GetFileComplexName(nodeId, string.Empty);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }

#if DEBUG
                //_logger.Info($"itemLogFileName = {itemLogFileName}");
#endif

                outputSw.WriteLine(logFileCreatorContext.BeginParagraph());
                switch(mode)
                {
                    case LogFileBuilderMode.Stat:
                        outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Node Id: {nodeId}"));
                        break;

                    case LogFileBuilderMode.StatAndFiles:
                        if(string.IsNullOrWhiteSpace(itemLogFileName.AbsoluteName))
                        {
                            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Node Id: {nodeId}"));
                        }
                        else
                        {
                            outputSw.Write(logFileCreatorContext.DecorateAsText("Node Id: "));
                            outputSw.Write(logFileCreatorContext.CreateLink(itemLogFileName.AbsoluteName, itemLogFileName.RelativeName, nodeId));
                            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine());
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
                
                outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Messages: {itemFileNamesList.Count}"));
                outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Errors: {itemFileNamesList.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal)}"));
                outputSw.WriteLine(logFileCreatorContext.EndParagraph());

#if DEBUG
                //break;//tmp
#endif
            }
        }
    }
}

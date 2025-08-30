using NLog;
using System;
using System.IO;
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
            _logger.Info($"options = {options}");
#endif

            options = options.Clone();

            LogFileCreatorOptionsHelper.PrepareOptions(options, logger);

#if DEBUG
            _logger.Info($"options (after) = {options}");
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
            _logger.Info($"fileStreamsStorageOptions = {fileStreamsStorageOptions}");
#endif

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

            var fileNamesDictByNodes = fileNamesList.GroupBy(p => p.Item1.NodeId).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
            //_logger.Info($"fileNamesDictByNodes.Count = {fileNamesDictByNodes.Count}");
#endif

            outputSw.WriteLine(logFileCreatorContext.BeginParagraph());
            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Messages: {fileNamesList.Count}"));
            outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Nodes: {fileNamesDictByNodes.Count}"));
            outputSw.WriteLine(logFileCreatorContext.EndParagraph());

            foreach (var fileNamesByNodesKvpItem in fileNamesDictByNodes.OrderByDescending(p => p.Value.Count))
            {
#if DEBUG
                //_logger.Info($"fileNamesByNodesKvpItem.Key = {fileNamesByNodesKvpItem.Key}");
                //_logger.Info($"fileNamesByNodesKvpItem.Value.Count = {fileNamesByNodesKvpItem.Value.Count}");
                //_logger.Info($"fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal) = {fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal)}");
#endif

                outputSw.WriteLine(logFileCreatorContext.BeginParagraph());
                outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Node Id: {fileNamesByNodesKvpItem.Key}"));
                outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Messages: {fileNamesByNodesKvpItem.Value.Count}"));
                outputSw.WriteLine(logFileCreatorContext.DecorateAsPreTextLine($"Errors: {fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal)}"));
                outputSw.WriteLine(logFileCreatorContext.EndParagraph());
            }
        }
    }
}

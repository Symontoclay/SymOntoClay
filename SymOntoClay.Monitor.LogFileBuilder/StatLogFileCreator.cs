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
            //_logger.Info($"options = {options}");
#endif

            options = options.Clone();

            LogFileCreatorOptionsHelper.PrepareOptions(options, logger);

#if DEBUG
            //_logger.Info($"options (after) = {options}");
#endif

            var now = DateTime.Now;

            var outputFileName = Path.Combine(options.OutputDirectory, $"Stat_{now.ToString(LogFileBuilderConstants.LongDateTimeFormat).Replace(':', '_').Trim()}.txt");

            var outputSw = new StreamWriter(outputFileName);
            outputSw.AutoFlush = true;
            outputSw.WriteLine("Statistic:");
            outputSw.WriteLine(now.ToString(LogFileBuilderConstants.LongDateTimeFormat));
            outputSw.WriteLine($"Source: '{options.SourceDirectoryName}'");            
            outputSw.WriteLine();

            var fileNamesList = MessageFilesReader.GetFileNames(options.SourceDirectoryName, null, null, null);

#if DEBUG
            //_logger.Info($"fileNamesList.Count = {fileNamesList.Count}");
#endif

            var fileNamesDictByNodes = fileNamesList.GroupBy(p => p.Item1.NodeId).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
            //_logger.Info($"fileNamesDictByNodes.Count = {fileNamesDictByNodes.Count}");
#endif

            outputSw.WriteLine($"Messages: {fileNamesList.Count}");
            outputSw.WriteLine($"Nodes: {fileNamesDictByNodes.Count}");
            outputSw.WriteLine();

            foreach (var fileNamesByNodesKvpItem in fileNamesDictByNodes.OrderByDescending(p => p.Value.Count))
            {
#if DEBUG
                //_logger.Info($"fileNamesByNodesKvpItem.Key = {fileNamesByNodesKvpItem.Key}");
                //_logger.Info($"fileNamesByNodesKvpItem.Value.Count = {fileNamesByNodesKvpItem.Value.Count}");
                //_logger.Info($"fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal) = {fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal)}");
#endif

                outputSw.WriteLine($"Node Id: {fileNamesByNodesKvpItem.Key}");
                outputSw.WriteLine($"Messages: {fileNamesByNodesKvpItem.Value.Count}");
                outputSw.WriteLine($"Errors: {fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal)}");
                outputSw.WriteLine();
            }
        }
    }
}

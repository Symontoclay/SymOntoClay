using NLog;
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

            var fileNamesList = MessageFilesReader.GetFileNames(options.SourceDirectoryName, null, null, null);

#if DEBUG
            _logger.Info($"fileNamesList.Count = {fileNamesList.Count}");
#endif

            var fileNamesDictByNodes = fileNamesList.GroupBy(p => p.Item1.NodeId).ToDictionary(p => p.Key, p => p.ToList());

#if DEBUG
            _logger.Info($"fileNamesDictByNodes.Count = {fileNamesDictByNodes.Count}");
#endif

            foreach(var fileNamesByNodesKvpItem in fileNamesDictByNodes)
            {
#if DEBUG
                _logger.Info($"fileNamesByNodesKvpItem.Key = {fileNamesByNodesKvpItem.Key}");
                _logger.Info($"fileNamesByNodesKvpItem.Value.Count = {fileNamesByNodesKvpItem.Value.Count}");
                _logger.Info($"fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal) = {fileNamesByNodesKvpItem.Value.Count(p => p.Item1.KindOfMessage == Common.Data.KindOfMessage.Error || p.Item1.KindOfMessage == Common.Data.KindOfMessage.Fatal)}");
#endif
            }
        }
    }
}

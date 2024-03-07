using NLog;
using SymOntoClay.CoreHelper;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public static class LogFileCreator
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

            PrepareOptions(options, logger);

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

            var showStages = (!(options.Silent ?? false)) && (logger != null);

#if DEBUG
            //_logger.Info($"showStages = {showStages}");
#endif

            if(showStages)
            {
                logger.Info("Fetching file names");
            }

            var fileNamesList = MessageFilesReader.GetFileNames(options.SourceDirectoryName, options.KindOfMessages).OrderBy(p => p.Item1.GlobalMessageNumber).ToList();

            var fileNamesListCount = fileNamesList.Count;

            if (showStages)
            {
                logger.Info($"Fetched {fileNamesListCount} file names");
            }

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

            using var fileStreamsStorage = new FileStreamsStorage(fileStreamsStorageOptions);

            //using var sw = new StreamWriter(options.OutputFileName);

            var rowOptionsList = options.Layout;

            var n = 0;

            string text = null;
            BaseMessage message = null;

            foreach (var fileName in fileNamesList)
            {
                if(showStages)
                {
                    n++;
                    logger.Info($"{n} from {fileNamesListCount}");
                }

#if DEBUG
                //_logger.Info($"fileName = {fileName}");
#endif

                try
                {
                    text = File.ReadAllText(fileName.Item2);

#if DEBUG
                    //_logger.Info($"text = {text}");
#endif

                    message = MessagesFactory.ReadMessage(text, fileName.Item1.KindOfMessage);

#if DEBUG
                    //_logger.Info($"message = {message}");
#endif

                    if (!options.TargetNodes?.Any(p => p.Equals(message.NodeId, StringComparison.OrdinalIgnoreCase)) ?? false)
                    {
                        continue;
                    }

                    if (!options.TargetThreads?.Any(p => p.Equals(message.ThreadId, StringComparison.OrdinalIgnoreCase)) ?? false)
                    {
                        continue;
                    }

                    var sw = fileStreamsStorage.GetStreamWriter(message.NodeId, message.ThreadId);

#if DEBUG
                    //_logger.Info($"message.GlobalMessageNumber = {message.GlobalMessageNumber}");
#endif

                    var targetFileName = fileStreamsStorage.GetFileName(message.NodeId, message.ThreadId).Replace("#", string.Empty);

#if DEBUG
                    //_logger.Info($"targetFileName = {targetFileName}");
#endif

                    var rowSb = new StringBuilder();

                    foreach (var rowOption in rowOptionsList)
                    {
                        rowSb.Append(rowOption.GetText(message, logFileCreatorContext, targetFileName));
                    }

#if DEBUG
                    //_logger.Info($"rowSb = {rowSb}");
#endif

                    sw.WriteLine(logFileCreatorContext.DecorateItem(message.GlobalMessageNumber, rowSb.ToString()));
                }
                catch (Exception e)
                {
                    _logger.Info($"text = '{text}'");
                    _logger.Info($"message = {message}");
                    _logger.Info($"e = {e}");
                }
            }

            if (showStages)
            {
                logger.Info($"All fetched {fileNamesListCount} file names processed");
            }
        }

        public static void PrepareOptions(LogFileCreatorOptions options, ILogger logger)
        {
#if DEBUG
            //_logger.Info($"options = {options}");
#endif

            if (!string.IsNullOrWhiteSpace(options.SourceDirectoryName))
            {
                options.SourceDirectoryName = EVPath.Normalize(options.SourceDirectoryName);
            }

            if (!string.IsNullOrWhiteSpace(options.OutputDirectory))
            {
                options.OutputDirectory = EVPath.Normalize(options.OutputDirectory);
            }

            if (!string.IsNullOrWhiteSpace(options.DotAppPath))
            {
                options.DotAppPath = EVPath.Normalize(options.DotAppPath);
            }
        }
    }
}

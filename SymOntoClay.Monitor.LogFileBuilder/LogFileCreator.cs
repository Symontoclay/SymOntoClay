using NLog;
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
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static void Run(LogFileCreatorOptions options)
        {
#if DEBUG
            //_logger.Info($"options = {options}");
#endif

            var fileNamesList = MessageFilesReader.GetFileNames(options.SourceDirectoryName, options.KindOfMessages).OrderBy(p => p.Item1.GlobalMessageNumber).ToList();

            var fileStreamsStorageOptions = new FileStreamsStorageOptions()
            {
                OutputDirectory = options.OutputDirectory,
                FileNameTemplate = options.FileNameTemplate,
                SeparateOutputByNodes = options.SeparateOutputByNodes ?? false,
                SeparateOutputByThreads = options.SeparateOutputByThreads ?? false
            };

#if DEBUG
            //_logger.Info($"fileStreamsStorageOptions = {fileStreamsStorageOptions}");
#endif

            using var fileStreamsStorage = new FileStreamsStorage(fileStreamsStorageOptions);

            //using var sw = new StreamWriter(options.OutputFileName);

            var rowOptionsList = options.Layout;

            foreach (var fileName in fileNamesList)
            {
#if DEBUG
                //_logger.Info($"fileName = {fileName}");
#endif

                var text = File.ReadAllText(fileName.Item2);

#if DEBUG
                //_logger.Info($"text = {text}");
#endif

                var message = MessagesFactory.ReadMessage(text, fileName.Item1.KindOfMessage);

#if DEBUG
                //_logger.Info($"message = {message}");
#endif

                if(!options.TargetNodes?.Any(p => p.Equals(message.NodeId, StringComparison.OrdinalIgnoreCase)) ?? false)
                {
                    continue;
                }

                if(!options.TargetThreads?.Any(p => p.Equals(message.ThreadId, StringComparison.OrdinalIgnoreCase)) ?? false)
                {
                    continue;
                }

                var sw = fileStreamsStorage.GetStreamWriter(message.NodeId, message.ThreadId);

                var rowSb = new StringBuilder();

                foreach (var rowOption in rowOptionsList)
                {
                    rowSb.Append(rowOption.GetText(message));
                }

#if DEBUG
                //_logger.Info($"rowSb = {rowSb}");
#endif

                sw.WriteLine(rowSb);
            }
        }
    }
}

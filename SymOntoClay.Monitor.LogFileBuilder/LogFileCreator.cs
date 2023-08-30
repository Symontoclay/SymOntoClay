using SymOntoClay.Monitor.Common.Data;
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
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static void Run(LogFileCreatorOptions options)
        {
#if DEBUG
            //_globalLogger.Info($"options = {options}");
#endif

            var fileNamesList = MessageFilesReader.GetFileNames(options.SourceDirectoryName, options.KindOfMessages).OrderBy(p => p.Item1.GlobalMessageNumber).ToList();

            using var sw = new StreamWriter(options.OutputFileName);

            var rowOptionsList = options.Layout;

            foreach (var fileName in fileNamesList)
            {
#if DEBUG
                //_globalLogger.Info($"fileName = {fileName}");
#endif

                var text = File.ReadAllText(fileName.Item2);

#if DEBUG
                //_globalLogger.Info($"text = {text}");
#endif

                var message = MessagesFactory.ReadMessage(text, fileName.Item1.KindOfMessage);

#if DEBUG
                //_globalLogger.Info($"message = {message}");
#endif

                var rowSb = new StringBuilder();

                foreach (var rowOption in rowOptionsList)
                {
                    rowSb.Append(rowOption.GetText(message));
                }

#if DEBUG
                //_globalLogger.Info($"rowSb = {rowSb}");
#endif

                sw.WriteLine(rowSb);
            }
        }
    }
}

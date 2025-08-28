using NLog;
using SymOntoClay.Common;
using System;
using System.IO;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public static class LogFileCreatorOptionsHelper
    {
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

            if (options.Mode == LogFileBuilderMode.StatAndFiles)
            {
                options.ToHtml = true;
                options.OutputDirectory = Path.Combine(options.OutputDirectory, DateTime.Now.ToString(LogFileBuilderConstants.LongDateTimeFormat).Replace(':', '_').Trim());
            }

            if (!Directory.Exists(options.OutputDirectory))
            {
                Directory.CreateDirectory(options.OutputDirectory);
            }
        }
    }
}

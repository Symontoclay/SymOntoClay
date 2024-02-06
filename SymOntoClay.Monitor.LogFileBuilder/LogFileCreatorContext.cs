using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileCreatorContext: ILogFileCreatorContext
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public LogFileCreatorContext(string dotAppPath, string outputDirectory) 
        {
            _dotAppPath = dotAppPath;
            _outputDirectory = outputDirectory;

            _tempDir = Environment.GetEnvironmentVariable("TEMP");

#if DEBUG
            _gbcLogger.Info($"_tempDir = {_tempDir}");
#endif
        }

        private readonly string _tempDir;
        private readonly string _dotAppPath;
        private readonly string _outputDirectory;

        /// <inheritdoc/>
        public string ConvertDotStrToImg(string dotStr)
        {
            var initialName = $"query_{Guid.NewGuid().ToString("D").Substring(0, 8)}";

#if DEBUG
            _gbcLogger.Info($"initialName = {initialName}");
#endif

            var dotFileName = Path.Combine(_tempDir, $"{initialName}.dot");

#if DEBUG
            _gbcLogger.Info($"dotFileName = {dotFileName}");
#endif

            File.WriteAllText(dotFileName, dotStr);

#if DEBUG
            _gbcLogger.Info($"_dotAppPath = {_dotAppPath}");
#endif

            var svgFile = Path.Combine(_outputDirectory, $"{initialName}.svg");

#if DEBUG
            _gbcLogger.Info($"svgFile = {svgFile}");
#endif

            var cmdArgs = $"-Tsvg \"{dotFileName}\" -o \"{svgFile}\"";

#if DEBUG
            _gbcLogger.Info($"cmdArgs = {cmdArgs}");
#endif

            using (var proc = Process.Start(_dotAppPath, cmdArgs))
            {
                proc.WaitForExit();

#if DEBUG
                _gbcLogger.Info($"proc.ExitCode = {proc.ExitCode}");
#endif
            }

            File.Delete(dotFileName);

            return svgFile;
        }
    }
}

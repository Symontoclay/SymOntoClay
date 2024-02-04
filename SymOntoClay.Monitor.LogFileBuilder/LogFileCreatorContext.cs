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

        public LogFileCreatorContext() 
        {
            _tempDir = Directory.GetCurrentDirectory();//tmp

#if DEBUG
            _gbcLogger.Info($"_tempDir = {_tempDir}");
#endif


        }

        private readonly string _tempDir;
        private readonly string _appPath = @"c:\Users\Acer\Downloads\Graphviz\bin\dot.exe";//tmp

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
            _gbcLogger.Info($"_appPath = {_appPath}");
#endif

            var svgFile = Path.Combine(_tempDir, $"{initialName}.svg");

#if DEBUG
            _gbcLogger.Info($"svgFile = {svgFile}");
#endif

            var cmdArgs = $"-Tsvg \"{dotFileName}\" -O\"{svgFile}\"";

#if DEBUG
            _gbcLogger.Info($"cmdArgs = {cmdArgs}");
#endif

            using (var proc = Process.Start(_appPath, cmdArgs))
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

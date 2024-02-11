using NLog;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
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
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public LogFileCreatorContext(string dotAppPath, string outputDirectory, bool toHtml, IEnumerable<BaseFileNameTemplateOptionItem> fileNameTemplate) 
        {
            _toHtml = toHtml;
            _fileNameTemplate = fileNameTemplate;

#if DEBUG
            //_gbcLogger.Info($"_toHtml = {_toHtml}");
#endif

            _dotAppPath = dotAppPath;
            _outputDirectory = outputDirectory;

            _tempDir = Environment.GetEnvironmentVariable("TEMP");

#if DEBUG
            //_gbcLogger.Info($"_tempDir = {_tempDir}");
#endif

            _fileExtension = GetFileExtension();
        }

        private readonly string _tempDir;
        private readonly string _dotAppPath;
        private readonly string _outputDirectory;
        private readonly bool _toHtml;
        private readonly IEnumerable<BaseFileNameTemplateOptionItem> _fileNameTemplate;
        private readonly string _fileExtension;

        public string GetFileExtension()
        {
            var sb = new StringBuilder();

            foreach (var item in _fileNameTemplate.Where(p => p.IsExtension))
            {
                sb.Append(item.GetText(string.Empty, string.Empty));
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string DecorateFileHeader()
        {
            if(_toHtml)
            {
                return """
                    <!doctype html>
                    <html>
                    <body>
                    """;
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string DecorateFileFooter()
        {
            if (_toHtml)
            {
                return """
                    </body>
                    </html>
                    """;
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string DecorateItem(string content)
        {
            if(_toHtml)
            {
                return $"<p>{content}</p>";
            }

            return content;
        }

        /// <inheritdoc/>
        public (string AbsoluteName, string RelativeName) ConvertDotStrToImg(string dotStr, string targetFileName)
        {
#if DEBUG
            //_gbcLogger.Info($"_toHtml = {_toHtml}");
            //_gbcLogger.Info($"targetFileName = {targetFileName}");
#endif

            var initialName = $"query_{Guid.NewGuid().ToString("D").Substring(0, 8)}";

#if DEBUG
            //_gbcLogger.Info($"initialName = {initialName}");
#endif

            var dotFileName = Path.Combine(_tempDir, $"{initialName}.dot");

#if DEBUG
            //_gbcLogger.Info($"dotFileName = {dotFileName}");
#endif

            File.WriteAllText(dotFileName, dotStr);

#if DEBUG
            //_gbcLogger.Info($"_dotAppPath = {_dotAppPath}");
#endif

            var imgOutputDirectoryResult = GetImgOutputDirectory(targetFileName);

            var relativeSvgFileName = Path.Combine(imgOutputDirectoryResult.RelativeName, $"{initialName}.svg");

            var svgFile = Path.Combine(imgOutputDirectoryResult.AbsoluteName, $"{initialName}.svg");

#if DEBUG
            //_gbcLogger.Info($"svgFile = {svgFile}");
#endif

            var cmdArgs = $"-Tsvg \"{dotFileName}\" -o \"{svgFile}\"";

#if DEBUG
            //_gbcLogger.Info($"cmdArgs = {cmdArgs}");
#endif

            using (var proc = Process.Start(_dotAppPath, cmdArgs))
            {
                proc.WaitForExit();

#if DEBUG
                //_gbcLogger.Info($"proc.ExitCode = {proc.ExitCode}");
#endif
            }

            File.Delete(dotFileName);

            return (svgFile, relativeSvgFileName);
        }

        private (string AbsoluteName, string RelativeName) GetImgOutputDirectory(string targetFileName)
        {
#if DEBUG
            //_gbcLogger.Info($"_toHtml = {_toHtml}");
            //_gbcLogger.Info($"targetFileName = {targetFileName}");
#endif

            if (_toHtml)
            {
                targetFileName = targetFileName.Replace(_fileExtension, string.Empty);

                var outputDirectory = Path.Combine(_outputDirectory, targetFileName);
                Directory.CreateDirectory(outputDirectory);
                return (outputDirectory, targetFileName);
            }

            return (_outputDirectory, string.Empty);
        }

        /// <inheritdoc/>
        public string CreateImgLink(string imgFileName, string relativeFileName)
        {
#if DEBUG
            //_gbcLogger.Info($"imgFileName = {imgFileName}");
            //_gbcLogger.Info($"relativeFileName = {relativeFileName}");
#endif

            if (_toHtml)
            {
                return $"<a href=\"{relativeFileName}\">{imgFileName}</a>";
            }

            return imgFileName;
        }
    }
}

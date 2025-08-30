/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileCreatorContext: ILogFileCreatorContext
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public LogFileCreatorContext(string dotAppPath, string outputDirectory, bool toHtml, bool isAbsUrl, IEnumerable<BaseFileNameTemplateOptionItem> fileNameTemplate) 
        {
            _toHtml = toHtml;
            _isAbsUrl = isAbsUrl;
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
        private readonly bool _isAbsUrl;
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
        public string DecorateItemAsParagraph(ulong globalMessageNumber, string content)
        {
            if(_toHtml)
            {
                return $"""
                <a name="id_{globalMessageNumber}"/>
                <p>{PrepareHtmlContent(content)}</p>
                """;
            }

            return content;
        }

        /// <inheritdoc/>
        public string DecorateItemAsParagraph(string content)
        {
            if (_toHtml)
            {
                return $"<p>{PrepareHtmlContent(content)}</p>";
            }

            return content;
        }

        /// <inheritdoc/>
        public string BeginParagraph()
        {
            if (_toHtml)
            {
                return "<p>";
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string EndParagraph()
        {
            if (_toHtml)
            {
                return "</p>";
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string DecorateAsText(string content)
        {
            if (_toHtml)
            {
                return $"{PrepareHtmlContent(content)}";
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string DecorateAsPreTextLine(string content)
        {
            if (_toHtml)
            {
                return $"{PrepareHtmlContent(content)}<br/>";
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string DecorateAsPreTextLine()
        {
            if (_toHtml)
            {
                return "<br/>";
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string LineSeparator()
        {
            if (_toHtml)
            {
                return "<hr>";
            }

            return "------------------------------------------------------------------------------------------------------";
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
                var url = PrepareUrl(_isAbsUrl ? imgFileName : relativeFileName);

                return $"<a href=\"{url}\">{imgFileName}</a>";
            }

            return imgFileName;
        }

        /// <inheritdoc/>
        public string CreateLink(string absoluteFileName, string relativeFileName, string title)
        {
#if DEBUG
            //_gbcLogger.Info($"absoluteFileName = {absoluteFileName}");
            //_gbcLogger.Info($"relativeFileName = {relativeFileName}");
            //_gbcLogger.Info($"title = {title}");
#endif

            if (_toHtml)
            {
                var url = PrepareUrl(_isAbsUrl ? absoluteFileName : relativeFileName);

                return $"<a href=\"{url}\">{title}</a>";
            }

            return absoluteFileName;
        }

        private string PrepareUrl(string url)
        {
            return url.Replace("#", "%23");
        }

        /// <inheritdoc/>
        public string ResolveMessagesRefs(string content)
        {
#if DEBUG
            //_gbcLogger.Info($"content = {content}");
#endif

            if(!content.Contains("msg::ref"))
            {
                return content;
            }

            int pos = 0;

            while((pos = content.IndexOf("msg::ref", pos)) != -1)
            {
#if DEBUG
                //_gbcLogger.Info($"pos = {pos}");
#endif

                var openBracketPoint = content.IndexOf("(", pos);

#if DEBUG
                //_gbcLogger.Info($"openBracketPoint = {openBracketPoint}");
#endif

                var closeBracketPoint = content.IndexOf(")", pos);

#if DEBUG
                //_gbcLogger.Info($"closeBracketPoint = {closeBracketPoint}");
#endif

                var expr = content.Substring(pos, closeBracketPoint - pos + 1);

#if DEBUG
                //_gbcLogger.Info($"expr = `{expr}`");
#endif

                var messageGlobalNumberStr = content.Substring(openBracketPoint + 1, closeBracketPoint - openBracketPoint - 1);

#if DEBUG
                //_gbcLogger.Info($"messageGlobalNumberStr = `{messageGlobalNumberStr}`");
#endif

                content = content.Replace(expr, ResolveMessagesRef(messageGlobalNumberStr));
            }

#if DEBUG
            //_gbcLogger.Info($"content (after) = {content}");
#endif

            return content;
        }

        private string ResolveMessagesRef(string messageGlobalNumberStr)
        {
            if(_toHtml)
            {
                return $"`<a href=\"#id_{messageGlobalNumberStr}\">{messageGlobalNumberStr}</a>`";
            }

            return $"`{messageGlobalNumberStr}`";
        }

        /// <inheritdoc/>
        public string NormalizeText(string content)
        {
            if(string.IsNullOrEmpty(content))
            {
                return content;
            }

            if (_toHtml)
            {
                return PrepareHtmlContent(content);
            }
            
            return content;
        }

        private string PrepareHtmlContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            return content.Replace("\n", "<br/>").Replace(" ", "&nbsp;");
        }
    }
}

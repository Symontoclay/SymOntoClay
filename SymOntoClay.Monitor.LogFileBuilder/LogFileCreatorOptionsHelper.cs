/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

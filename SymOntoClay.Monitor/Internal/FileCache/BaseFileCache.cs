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

using SymOntoClay.Serialization;
using System.IO;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    [SocSerialization]
    public abstract partial class BaseFileCache : IFileCache
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        protected BaseFileCache(string absoluteDirectory, string relativeDirectory)
        {
            _absoluteDirectory = absoluteDirectory;
            _relativeDirectory = relativeDirectory;

            Directory.CreateDirectory(absoluteDirectory);
        }

        protected string _absoluteDirectory;
        protected string _relativeDirectory;

        /// <inheritdoc/>
        public string AbsoluteDirectoryName => _absoluteDirectory;

        /// <inheritdoc/>
        public string RelativeDirectoryName => _relativeDirectory;

        /// <inheritdoc/>
        public void WriteFile(string fileName, string messageText)
        {
#if DEBUG
            //_globalLogger.Info($"messageText = {messageText}");
            //_globalLogger.Info($"messageText = {messageText}");
#endif

            var fullFileName = Path.Combine(_absoluteDirectory, fileName);

#if DEBUG
            //_globalLogger.Info($"fullFileName = {fullFileName}");
#endif

            File.WriteAllText(fullFileName, messageText);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    public abstract class BaseFileCache : IFileCache
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

        protected readonly string _absoluteDirectory;
        protected readonly string _relativeDirectory;

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

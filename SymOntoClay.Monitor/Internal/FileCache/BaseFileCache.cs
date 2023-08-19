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

        protected BaseFileCache(string directory)
        {
            _directory = directory;

            Directory.CreateDirectory(directory);
        }

        protected readonly string _directory;

        /// <inheritdoc/>
        public void WriteFile(string fileName, string messageText)
        {
#if DEBUG
            _globalLogger.Info($"messageText = {messageText}");
            _globalLogger.Info($"messageText = {messageText}");
#endif

            var fullFileName = Path.Combine(_directory, fileName);

#if DEBUG
            _globalLogger.Info($"fullFileName = {fullFileName}");
#endif

            File.WriteAllText(fullFileName, messageText);
        }
    }
}

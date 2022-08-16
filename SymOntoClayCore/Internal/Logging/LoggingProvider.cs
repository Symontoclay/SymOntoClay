using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.Core.Internal.Logging
{
    public class LoggingProvider: BaseComponent, ILoggingProvider
    {
        public LoggingProvider(IMainStorageContext context, BaseStorageSettings settings)
            : base(context.Logger)
        {
            _id = context.Id;
            KindOfLogicalSearchExplain = settings.KindOfLogicalSearchExplain;
            _logicalSearchExplainDumpDir = settings.LogicalSearchExplainDumpDir;
            EnableAddingRemovingFactLoggingInStorages = settings.EnableAddingRemovingFactLoggingInStorages;

#if DEBUG
            //Log($"_id = {_id}");
            //Log($"_logicalSearchExplainDumpDir = {_logicalSearchExplainDumpDir}");
            //Log($"KindOfLogicalSearchExplain = {KindOfLogicalSearchExplain}");
#endif
        }

        private readonly string _logicalSearchExplainDumpDir;
        private readonly string _id;

        /// <inheritdoc/>
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; private set; }

        /// <inheritdoc/>
        public bool EnableAddingRemovingFactLoggingInStorages { get; private set; }

        /// <inheritdoc/>
        public string DumpToFile(LogicalSearchExplainNode explainNode)
        {
            var fileName = Path.Combine(_logicalSearchExplainDumpDir, $"{_id}_query_{Guid.NewGuid().ToString("D").Substring(0, 8)}.dot");

#if DEBUG
            //Log($"fileName = {fileName}");
#endif

            var dotStr = DebugHelperForLogicalSearchExplainNode.ToDot(explainNode);

            //Log($"dotStr = '{dotStr}'");

            File.WriteAllText(fileName, dotStr);

            return fileName;
        }
    }
}

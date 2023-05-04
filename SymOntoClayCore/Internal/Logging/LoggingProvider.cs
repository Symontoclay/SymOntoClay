/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

            var dotStr = DebugHelperForLogicalSearchExplainNode.ToDot(explainNode);

            File.WriteAllText(fileName, dotStr);

            return fileName;
        }
    }
}

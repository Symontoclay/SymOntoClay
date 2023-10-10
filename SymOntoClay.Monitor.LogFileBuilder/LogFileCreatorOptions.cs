﻿using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
using SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileCreatorOptions : IObjectToString
    {
        public string SourceDirectoryName { get; set; }
        public IEnumerable<string> TargetNodes { get; set; }
        public IEnumerable<string> TargetThreads { get; set; }
        [Obsolete]
        public string OutputFileName { get; set; }
        public string OutputDirectory { get; set; }
        public IEnumerable<BaseFileNameTemplateOptionItem> FileNameTemplate { get; set; }
        public bool SeparateOutputByNodes { get; set; }
        public bool SeparateOutputByThreads { get; set; }
        public IEnumerable<KindOfMessage> KindOfMessages { get; set; }
        public IEnumerable<BaseMessageTextRowOptionItem> Layout { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SourceDirectoryName)} = {SourceDirectoryName}");
            sb.PrintPODList(n, nameof(TargetNodes), TargetNodes);
            sb.PrintPODList(n, nameof(TargetThreads), TargetThreads);
            sb.AppendLine($"{spaces}{nameof(OutputFileName)} = {OutputFileName}");
            sb.AppendLine($"{spaces}{nameof(OutputDirectory)} = {OutputDirectory}");
            sb.PrintObjListProp(n, nameof(FileNameTemplate), FileNameTemplate);
            sb.AppendLine($"{spaces}{nameof(SeparateOutputByNodes)} = {SeparateOutputByNodes}");
            sb.AppendLine($"{spaces}{nameof(SeparateOutputByThreads)} = {SeparateOutputByThreads}");
            sb.PrintPODList(n, nameof(KindOfMessages), KindOfMessages);
            sb.PrintObjListProp(n, nameof(Layout), Layout);
            //sb.AppendLine($"{spaces}{nameof()} = {}");
            return sb.ToString();
        }
    }
}

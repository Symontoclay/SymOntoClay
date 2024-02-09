using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class FileStreamsStorageOptions : IObjectToString
    {
        public string OutputDirectory { get; set; }
        public IEnumerable<BaseFileNameTemplateOptionItem> FileNameTemplate { get; set; }
        public bool SeparateOutputByNodes { get; set; }
        public bool SeparateOutputByThreads { get; set; }
        public bool ToHtml { get; set; }
        public ILogFileCreatorContext LogFileCreatorContext { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(OutputDirectory)} = {OutputDirectory}");
            sb.PrintObjListProp(n, nameof(FileNameTemplate), FileNameTemplate);
            sb.AppendLine($"{spaces}{nameof(SeparateOutputByNodes)} = {SeparateOutputByNodes}");
            sb.AppendLine($"{spaces}{nameof(SeparateOutputByThreads)} = {SeparateOutputByThreads}");
            sb.AppendLine($"{spaces}{nameof(ToHtml)} = {ToHtml}");
            sb.PrintExisting(n, nameof(LogFileCreatorContext), LogFileCreatorContext);
            return sb.ToString();
        }
    }
}

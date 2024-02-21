using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileBuilderOptions : IObjectToString
    {
        public bool IsHelp { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public bool NoLogo { get; set; }
        public string TargetNodeId { get; set; }
        public string TargetThreadId { get; set; }
        public bool? SplitByNodes { get; set; }
        public bool? SplitByThreads { get; set; }
        public string ConfigurationFileName { get; set; }
        public bool? ToHtml { get; set; }
        public string DotAppPath { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(IsHelp)} = {IsHelp}");
            sb.AppendLine($"{spaces}{nameof(Input)} = {Input}");
            sb.AppendLine($"{spaces}{nameof(Output)} = {Output}");
            sb.AppendLine($"{spaces}{nameof(NoLogo)} = {NoLogo}");
            sb.AppendLine($"{spaces}{nameof(TargetNodeId)} = {TargetNodeId}");
            sb.AppendLine($"{spaces}{nameof(TargetThreadId)} = {TargetThreadId}");
            sb.AppendLine($"{spaces}{nameof(SplitByNodes)} = {SplitByNodes}");
            sb.AppendLine($"{spaces}{nameof(SplitByThreads)} = {SplitByThreads}");
            sb.AppendLine($"{spaces}{nameof(ConfigurationFileName)} = {ConfigurationFileName}");
            sb.AppendLine($"{spaces}{nameof(ToHtml)} = {ToHtml}");
            sb.AppendLine($"{spaces}{nameof(DotAppPath)} = {DotAppPath}");
            return sb.ToString();
        }
    }
}

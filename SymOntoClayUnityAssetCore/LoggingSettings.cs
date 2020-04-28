using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public class LoggingSettings: IObjectToString
    {
        public string LogDir { get; set; }
        public string RootContractName { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableRemoteConnection { get; set; }
        public IList<IPlatformLogger> PlatformLoggers { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(LogDir)} = {LogDir}");
            sb.AppendLine($"{spaces}{nameof(RootContractName)} = {RootContractName}");
            sb.AppendLine($"{spaces}{nameof(EnableLogging)} = {EnableLogging}");
            sb.AppendLine($"{spaces}{nameof(EnableRemoteConnection)} = {EnableRemoteConnection}");
            var platformLoggersMark = PlatformLoggers == null ? "No" : PlatformLoggers.Any() ? "Yes" : "No";
            sb.AppendLine($"{spaces}{nameof(PlatformLoggers)} = {platformLoggersMark}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
    }
}

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor
{
    public class BaseMonitorSettings : IObjectToString
    {
        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        public bool Enable { get; set; }
        public bool EnableRemoteConnection { get; set; }

        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; set; } = KindOfLogicalSearchExplain.None;
        public string LogicalSearchExplainDumpDir { get; set; }
        public bool EnableAddingRemovingFactLoggingInStorages { get; set; }

        public bool EnableFullCallInfo { get; set; }

        public MonitorFeatures Features { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public BaseMonitorSettings Clone()
        {
            var result = new BaseMonitorSettings();

            result.Enable = Enable;
            result.EnableRemoteConnection = EnableRemoteConnection;
            result.KindOfLogicalSearchExplain = KindOfLogicalSearchExplain;
            result.LogicalSearchExplainDumpDir = LogicalSearchExplainDumpDir;
            result.EnableAddingRemovingFactLoggingInStorages = EnableAddingRemovingFactLoggingInStorages;
            result.EnableFullCallInfo = EnableFullCallInfo;
            result.Features = Features?.Clone();

            return result;
        }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Enable)} = {Enable}");
            sb.AppendLine($"{spaces}{nameof(EnableRemoteConnection)} = {EnableRemoteConnection}");
            sb.AppendLine($"{spaces}{nameof(KindOfLogicalSearchExplain)} = {KindOfLogicalSearchExplain}");
            sb.AppendLine($"{spaces}{nameof(LogicalSearchExplainDumpDir)} = {LogicalSearchExplainDumpDir}");
            sb.AppendLine($"{spaces}{nameof(EnableAddingRemovingFactLoggingInStorages)} = {EnableAddingRemovingFactLoggingInStorages}");
            sb.AppendLine($"{spaces}{nameof(EnableFullCallInfo)} = {EnableFullCallInfo}");
            sb.PrintObjProp(n, nameof(Features), Features);
            return sb.ToString();
        }
    }
}

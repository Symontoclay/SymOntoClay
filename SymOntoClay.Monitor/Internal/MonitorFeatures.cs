using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorFeatures : IMonitorFeatures, IObjectToString
    {
        /// <inheritdoc/>
        public bool EnableCallMethod { get; set; }

        /// <inheritdoc/>
        public bool EnableParameter { get; set; }

        /// <inheritdoc/>
        public bool EnableEndCallMethod { get; set; }

        /// <inheritdoc/>
        public bool EnableMethodResolving { get; set; }

        /// <inheritdoc/>
        public bool EnableEndMethodResolving { get; set; }

        /// <inheritdoc/>
        public bool EnableActionResolving { get; set; }

        /// <inheritdoc/>
        public bool EnableEndActionResolving { get; set; }

        /// <inheritdoc/>
        public bool EnableHostMethodResolving { get; set; }

        /// <inheritdoc/>
        public bool EnableEndHostMethodResolving { get; set; }

        /// <inheritdoc/>
        public bool EnableHostMethodActivation { get; set; }

        /// <inheritdoc/>
        public bool EnableEndHostMethodActivation { get; set; }

        /// <inheritdoc/>
        public bool EnableHostMethodStarting { get; set; }

        /// <inheritdoc/>
        public bool EnableEndHostMethodStarting { get; set; }

        /// <inheritdoc/>
        public bool EnableHostMethodExecution { get; set; }

        /// <inheritdoc/>
        public bool EnableEndHostMethodExecution { get; set; }

        /// <inheritdoc/>
        public bool EnableSystemExpr { get; set; }

        /// <inheritdoc/>
        public bool EnableCodeFrame { get; set; }

        /// <inheritdoc/>
        public bool EnableLeaveThreadExecutor { get; set; }

        /// <inheritdoc/>
        public bool EnableGoBackToPrevCodeFrame { get; set; }

        /// <inheritdoc/>
        public bool EnableOutput { get; set; }

        /// <inheritdoc/>
        public bool EnableTrace { get; set; }

        /// <inheritdoc/>
        public bool EnableDebug { get; set; }

        /// <inheritdoc/>
        public bool EnableInfo { get; set; }

        /// <inheritdoc/>
        public bool EnableWarn { get; set; }

        /// <inheritdoc/>
        public bool EnableError { get; set; }

        /// <inheritdoc/>
        public bool EnableFatal { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public MonitorFeatures Clone()
        {
            var result = new MonitorFeatures();
            result.EnableCallMethod = EnableCallMethod;
            result.EnableParameter = EnableParameter;
            result.EnableEndCallMethod = EnableEndCallMethod;
            result.EnableMethodResolving = EnableMethodResolving;
            result.EnableEndMethodResolving = EnableEndMethodResolving;
            result.EnableActionResolving = EnableActionResolving;
            result.EnableEndActionResolving = EnableEndActionResolving;
            result.EnableHostMethodResolving = EnableHostMethodResolving;
            result.EnableEndHostMethodResolving = EnableEndHostMethodResolving;
            result.EnableHostMethodActivation = EnableHostMethodActivation;
            result.EnableEndHostMethodActivation = EnableEndHostMethodActivation;
            result.EnableHostMethodStarting = EnableHostMethodStarting;
            result.EnableEndHostMethodStarting = EnableEndHostMethodStarting;
            result.EnableHostMethodExecution = EnableHostMethodExecution;
            result.EnableEndHostMethodExecution = EnableEndHostMethodExecution;
            result.EnableSystemExpr = EnableSystemExpr;
            result.EnableCodeFrame = EnableCodeFrame;
            result.EnableLeaveThreadExecutor = EnableLeaveThreadExecutor;
            result.EnableGoBackToPrevCodeFrame = EnableGoBackToPrevCodeFrame;
            result.EnableOutput = EnableOutput;
            result.EnableTrace = EnableTrace;
            result.EnableDebug = EnableDebug;
            result.EnableInfo = EnableInfo;
            result.EnableWarn = EnableWarn;
            result.EnableError = EnableError;
            result.EnableFatal = EnableFatal;

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(EnableCallMethod)} = {EnableCallMethod}");
            sb.AppendLine($"{spaces}{nameof(EnableParameter)} = {EnableParameter}");
            sb.AppendLine($"{spaces}{nameof(EnableEndCallMethod)} = {EnableEndCallMethod}");
            sb.AppendLine($"{spaces}{nameof(EnableMethodResolving)} = {EnableMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(EnableEndMethodResolving)} = {EnableEndMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(EnableActionResolving)} = {EnableActionResolving}");
            sb.AppendLine($"{spaces}{nameof(EnableEndActionResolving)} = {EnableEndActionResolving}");
            sb.AppendLine($"{spaces}{nameof(EnableHostMethodResolving)} = {EnableHostMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(EnableEndHostMethodResolving)} = {EnableEndHostMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(EnableHostMethodActivation)} = {EnableHostMethodActivation}");
            sb.AppendLine($"{spaces}{nameof(EnableEndHostMethodActivation)} = {EnableEndHostMethodActivation}");
            sb.AppendLine($"{spaces}{nameof(EnableHostMethodStarting)} = {EnableHostMethodStarting}");
            sb.AppendLine($"{spaces}{nameof(EnableEndHostMethodStarting)} = {EnableEndHostMethodStarting}");
            sb.AppendLine($"{spaces}{nameof(EnableHostMethodExecution)} = {EnableHostMethodExecution}");
            sb.AppendLine($"{spaces}{nameof(EnableEndHostMethodExecution)} = {EnableEndHostMethodExecution}");
            sb.AppendLine($"{spaces}{nameof(EnableSystemExpr)} = {EnableSystemExpr}");
            sb.AppendLine($"{spaces}{nameof(EnableCodeFrame)} = {EnableCodeFrame}");
            sb.AppendLine($"{spaces}{nameof(EnableLeaveThreadExecutor)} = {EnableLeaveThreadExecutor}");
            sb.AppendLine($"{spaces}{nameof(EnableGoBackToPrevCodeFrame)} = {EnableGoBackToPrevCodeFrame}");
            sb.AppendLine($"{spaces}{nameof(EnableOutput)} = {EnableOutput}");
            sb.AppendLine($"{spaces}{nameof(EnableTrace)} = {EnableTrace}");
            sb.AppendLine($"{spaces}{nameof(EnableDebug)} = {EnableDebug}");
            sb.AppendLine($"{spaces}{nameof(EnableInfo)} = {EnableInfo}");
            sb.AppendLine($"{spaces}{nameof(EnableWarn)} = {EnableWarn}");
            sb.AppendLine($"{spaces}{nameof(EnableError)} = {EnableError}");
            sb.AppendLine($"{spaces}{nameof(EnableFatal)} = {EnableFatal}");
            return sb.ToString();
        }
    }
}

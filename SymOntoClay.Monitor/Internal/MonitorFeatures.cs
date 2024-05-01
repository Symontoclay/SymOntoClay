/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
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
        public bool EnableStartProcessInfo { get; set; }

        /// <inheritdoc/>
        public bool EnableCancelProcessInfo { get; set; }

        /// <inheritdoc/>
        public bool EnableWeakCancelProcessInfo { get; set; }

        /// <inheritdoc/>
        public bool EnableCancelInstanceExecution { get; set; }

        /// <inheritdoc/>
        public bool EnableSetExecutionCoordinatorStatus { get; set; }

        /// <inheritdoc/>
        public bool EnableSetProcessInfoStatus { get; set; }

        /// <inheritdoc/>
        public bool EnableWaitProcessInfo { get; set; }

        /// <inheritdoc/>
        public bool EnableRunLifecycleTrigger { get; set; }

        /// <inheritdoc/>
        public bool EnableDoTriggerSearch { get; set; }

        /// <inheritdoc/>
        public bool EnableEndDoTriggerSearch { get; set; }

        /// <inheritdoc/>
        public bool EnableSetConditionalTrigger { get; set; }

        /// <inheritdoc/>
        public bool EnableResetConditionalTrigger { get; set; }

        /// <inheritdoc/>
        public bool EnableRunSetExprOfConditionalTrigger { get; set; }

        /// <inheritdoc/>
        public bool EnableEndRunSetExprOfConditionalTrigger { get; set; }

        /// <inheritdoc/>
        public bool EnableRunResetExprOfConditionalTrigger { get; set; }

        /// <inheritdoc/>
        public bool EnableEndRunResetExprOfConditionalTrigger { get; set; }

        /// <inheritdoc/>
        public bool IsEnabledAnyConditionalTriggerFeature => EnableDoTriggerSearch || EnableEndDoTriggerSearch || EnableSetConditionalTrigger
            || EnableResetConditionalTrigger || EnableRunSetExprOfConditionalTrigger || EnableEndRunSetExprOfConditionalTrigger ||
            EnableRunResetExprOfConditionalTrigger || EnableEndRunResetExprOfConditionalTrigger;

        /// <inheritdoc/>
        public bool EnableActivateIdleAction { get; set; }

        /// <inheritdoc/>
        public bool EnableTasks { get; set; }

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
            result.EnableStartProcessInfo = EnableStartProcessInfo;
            result.EnableCancelProcessInfo = EnableCancelProcessInfo;
            result.EnableWeakCancelProcessInfo = EnableWeakCancelProcessInfo;
            result.EnableCancelInstanceExecution = EnableCancelInstanceExecution;
            result.EnableSetExecutionCoordinatorStatus = EnableSetExecutionCoordinatorStatus;
            result.EnableSetProcessInfoStatus = EnableSetProcessInfoStatus;
            result.EnableWaitProcessInfo = EnableWaitProcessInfo;
            result.EnableRunLifecycleTrigger = EnableRunLifecycleTrigger;
            result.EnableDoTriggerSearch = EnableDoTriggerSearch;
            result.EnableEndDoTriggerSearch = EnableEndDoTriggerSearch;
            result.EnableSetConditionalTrigger = EnableSetConditionalTrigger;
            result.EnableResetConditionalTrigger = EnableResetConditionalTrigger;
            result.EnableRunSetExprOfConditionalTrigger = EnableRunSetExprOfConditionalTrigger;
            result.EnableEndRunSetExprOfConditionalTrigger = EnableEndRunSetExprOfConditionalTrigger;
            result.EnableRunResetExprOfConditionalTrigger = EnableRunResetExprOfConditionalTrigger;
            result.EnableEndRunResetExprOfConditionalTrigger = EnableEndRunResetExprOfConditionalTrigger;
            result.EnableActivateIdleAction = EnableActivateIdleAction;
            result.EnableTasks = EnableTasks;
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
            sb.AppendLine($"{spaces}{nameof(EnableStartProcessInfo)} = {EnableStartProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(EnableCancelProcessInfo)} = {EnableCancelProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(EnableWeakCancelProcessInfo)} = {EnableWeakCancelProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(EnableCancelInstanceExecution)} = {EnableCancelInstanceExecution}");
            sb.AppendLine($"{spaces}{nameof(EnableSetExecutionCoordinatorStatus)} = {EnableSetExecutionCoordinatorStatus}");
            sb.AppendLine($"{spaces}{nameof(EnableSetProcessInfoStatus)} = {EnableSetProcessInfoStatus}");
            sb.AppendLine($"{spaces}{nameof(EnableWaitProcessInfo)} = {EnableWaitProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(EnableRunLifecycleTrigger)} = {EnableRunLifecycleTrigger}");
            sb.AppendLine($"{spaces}{nameof(EnableDoTriggerSearch)} = {EnableDoTriggerSearch}");
            sb.AppendLine($"{spaces}{nameof(EnableEndDoTriggerSearch)} = {EnableEndDoTriggerSearch}");
            sb.AppendLine($"{spaces}{nameof(EnableSetConditionalTrigger)} = {EnableSetConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(EnableResetConditionalTrigger)} = {EnableResetConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(EnableRunSetExprOfConditionalTrigger)} = {EnableRunSetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(EnableEndRunSetExprOfConditionalTrigger)} = {EnableEndRunSetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(EnableRunResetExprOfConditionalTrigger)} = {EnableRunResetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(EnableEndRunResetExprOfConditionalTrigger)} = {EnableEndRunResetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(IsEnabledAnyConditionalTriggerFeature)} = {IsEnabledAnyConditionalTriggerFeature}");
            sb.AppendLine($"{spaces}{nameof(EnableActivateIdleAction)} = {EnableActivateIdleAction}");
            sb.AppendLine($"{spaces}{nameof(EnableTasks)} = {EnableTasks}");
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

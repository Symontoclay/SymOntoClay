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
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common
{
    public interface IMonitorFeatures : IObjectToString
    {
        bool EnableCallMethod { get; }
        bool EnableParameter { get; }
        bool EnableEndCallMethod { get; }
        bool EnableMethodResolving { get; }
        bool EnableEndMethodResolving { get; }
        bool EnableActionResolving { get; }
        bool EnableEndActionResolving { get; }
        bool EnableHostMethodResolving { get; }
        bool EnableEndHostMethodResolving { get; }
        bool EnableHostMethodActivation { get; }
        bool EnableEndHostMethodActivation { get; }
        bool EnableHostMethodStarting { get; }
        bool EnableEndHostMethodStarting { get; }
        bool EnableHostMethodExecution { get; }
        bool EnableEndHostMethodExecution { get; }
        bool EnableSystemExpr { get; }
        bool EnableCodeFrame { get; }
        bool EnableLeaveThreadExecutor { get; }
        bool EnableGoBackToPrevCodeFrame { get; }
        bool EnableStartProcessInfo { get; }
        bool EnableCancelProcessInfo { get; }
        bool EnableWeakCancelProcessInfo { get; }
        bool EnableCancelInstanceExecution { get; }
        bool EnableSetExecutionCoordinatorStatus { get; }
        bool EnableSetProcessInfoStatus { get; }
        bool EnableWaitProcessInfo { get; }
        bool EnableRunLifecycleTrigger { get; }
        bool EnableDoTriggerSearch { get; }
        bool EnableEndDoTriggerSearch { get; }
        bool EnableSetConditionalTrigger { get; }
        bool EnableResetConditionalTrigger { get; }
        bool EnableRunSetExprOfConditionalTrigger { get; }
        bool EnableEndRunSetExprOfConditionalTrigger { get; }
        bool EnableRunResetExprOfConditionalTrigger { get; }
        bool EnableEndRunResetExprOfConditionalTrigger { get; }
        bool IsEnabledAnyConditionalTriggerFeature { get; }
        bool EnableActivateIdleAction { get; }
        bool EnableTasks { get; }
        bool EnableOutput { get; }
        bool EnableTrace { get; }
        bool EnableDebug { get; }
        bool EnableInfo { get; }
        bool EnableWarn { get; }
        bool EnableError { get; }
        bool EnableFatal { get; }
    }
}

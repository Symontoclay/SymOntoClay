/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

namespace SymOntoClay.Monitor.Helpers
{
    public static class MonitorFeaturesHelper
    {
        public static void SetAllFeaturesEnabled(MonitorFeatures monitorFeatures)
        {
            monitorFeatures.EnableAddEndpoint = true;
            monitorFeatures.EnableCallMethod = true;
            monitorFeatures.EnableParameter = true;
            monitorFeatures.EnableEndCallMethod = true;
            monitorFeatures.EnableMethodResolving = true;
            monitorFeatures.EnableEndMethodResolving = true;
            monitorFeatures.EnableActionResolving = true;
            monitorFeatures.EnableEndActionResolving = true;
            monitorFeatures.EnableHostMethodResolving = true;
            monitorFeatures.EnableEndHostMethodResolving = true;
            monitorFeatures.EnableHostMethodActivation = true;
            monitorFeatures.EnableEndHostMethodActivation = true;
            monitorFeatures.EnableHostMethodStarting = true;
            monitorFeatures.EnableEndHostMethodStarting = true;
            monitorFeatures.EnableHostMethodExecution = true;
            monitorFeatures.EnableEndHostMethodExecution = true;
            monitorFeatures.EnableSystemExpr = true;
            monitorFeatures.EnableCodeFrame = true;
            monitorFeatures.EnableLeaveThreadExecutor = true;
            monitorFeatures.EnableGoBackToPrevCodeFrame = true;
            monitorFeatures.EnableStartProcessInfo = true;
            monitorFeatures.EnableCancelProcessInfo = true;
            monitorFeatures.EnableWeakCancelProcessInfo = true;
            monitorFeatures.EnableCancelInstanceExecution = true;
            monitorFeatures.EnableSetExecutionCoordinatorStatus = true;
            monitorFeatures.EnableSetProcessInfoStatus = true;
            monitorFeatures.EnableWaitProcessInfo = true;
            monitorFeatures.EnableRunLifecycleTrigger = true;
            monitorFeatures.EnableDoTriggerSearch = true;
            monitorFeatures.EnableEndDoTriggerSearch = true;
            monitorFeatures.EnableSetConditionalTrigger = true;
            monitorFeatures.EnableResetConditionalTrigger = true;
            monitorFeatures.EnableRunSetExprOfConditionalTrigger = true;
            monitorFeatures.EnableEndRunSetExprOfConditionalTrigger = true;
            monitorFeatures.EnableRunResetExprOfConditionalTrigger = true;
            monitorFeatures.EnableEndRunResetExprOfConditionalTrigger = true;
            monitorFeatures.EnableActivateIdleAction = true;
            monitorFeatures.EnableLogicalSearchExplain = true;
            monitorFeatures.EnableAddFactOrRuleTriggerResult = true;
            monitorFeatures.EnableAddFactToLogicalStorage = true;
            monitorFeatures.EnableRemoveFactFromLogicalStorage = true;
            monitorFeatures.EnableRefreshLifeTimeInLogicalStorage = true;
            monitorFeatures.EnablePutFactForRemovingFromLogicalStorage = true;
            monitorFeatures.EnableThreadTask = true;
            monitorFeatures.EnableHtn = true;
            monitorFeatures.EnableBuildPlan = true;
            monitorFeatures.EnableVisionFrame = true;
            monitorFeatures.EnableCatchPublicFactsInVisionFrame = true;
            monitorFeatures.EnableBecomeInvisible = true;
            monitorFeatures.EnableBecomeVisible = true;
            monitorFeatures.EnableChangedAddFocus = true;
            monitorFeatures.EnableChangedRemoveFocus = true;
            monitorFeatures.EnableChangedDistance = true;
            monitorFeatures.EnableDumpVisionFrame = true;
            monitorFeatures.EnableOutput = true;
            monitorFeatures.EnableTrace = true;
            monitorFeatures.EnableDebug = true;
            monitorFeatures.EnableInfo = true;
            monitorFeatures.EnableWarn = true;
            monitorFeatures.EnableError = true;
            monitorFeatures.EnableFatal = true;
        }

        public static void SetAllFeaturesDisabled(MonitorFeatures monitorFeatures)
        {
            monitorFeatures.EnableAddEndpoint = false;
            monitorFeatures.EnableCallMethod = false;
            monitorFeatures.EnableParameter = false;
            monitorFeatures.EnableEndCallMethod = false;
            monitorFeatures.EnableMethodResolving = false;
            monitorFeatures.EnableEndMethodResolving = false;
            monitorFeatures.EnableActionResolving = false;
            monitorFeatures.EnableEndActionResolving = false;
            monitorFeatures.EnableHostMethodResolving = false;
            monitorFeatures.EnableEndHostMethodResolving = false;
            monitorFeatures.EnableHostMethodActivation = false;
            monitorFeatures.EnableEndHostMethodActivation = false;
            monitorFeatures.EnableHostMethodStarting = false;
            monitorFeatures.EnableEndHostMethodStarting = false;
            monitorFeatures.EnableHostMethodExecution = false;
            monitorFeatures.EnableEndHostMethodExecution = false;
            monitorFeatures.EnableSystemExpr = false;
            monitorFeatures.EnableCodeFrame = false;
            monitorFeatures.EnableLeaveThreadExecutor = false;
            monitorFeatures.EnableGoBackToPrevCodeFrame = false;
            monitorFeatures.EnableStartProcessInfo = false;
            monitorFeatures.EnableCancelProcessInfo = false;
            monitorFeatures.EnableWeakCancelProcessInfo = false;
            monitorFeatures.EnableCancelInstanceExecution = false;
            monitorFeatures.EnableSetExecutionCoordinatorStatus = false;
            monitorFeatures.EnableSetProcessInfoStatus = false;
            monitorFeatures.EnableWaitProcessInfo = false;
            monitorFeatures.EnableRunLifecycleTrigger = false;
            monitorFeatures.EnableDoTriggerSearch = false;
            monitorFeatures.EnableEndDoTriggerSearch = false;
            monitorFeatures.EnableSetConditionalTrigger = false;
            monitorFeatures.EnableResetConditionalTrigger = false;
            monitorFeatures.EnableRunSetExprOfConditionalTrigger = false;
            monitorFeatures.EnableEndRunSetExprOfConditionalTrigger = false;
            monitorFeatures.EnableRunResetExprOfConditionalTrigger = false;
            monitorFeatures.EnableEndRunResetExprOfConditionalTrigger = false;
            monitorFeatures.EnableActivateIdleAction = false;
            monitorFeatures.EnableLogicalSearchExplain = false;
            monitorFeatures.EnableAddFactOrRuleTriggerResult = false;
            monitorFeatures.EnableAddFactToLogicalStorage = false;
            monitorFeatures.EnableRemoveFactFromLogicalStorage = false;
            monitorFeatures.EnableRefreshLifeTimeInLogicalStorage = false;
            monitorFeatures.EnablePutFactForRemovingFromLogicalStorage = false;
            monitorFeatures.EnableThreadTask = false;
            monitorFeatures.EnableHtn = false;
            monitorFeatures.EnableBuildPlan = false;
            monitorFeatures.EnableVisionFrame = false;
            monitorFeatures.EnableCatchPublicFactsInVisionFrame = false;
            monitorFeatures.EnableBecomeInvisible = false;
            monitorFeatures.EnableBecomeVisible = false;
            monitorFeatures.EnableChangedAddFocus = false;
            monitorFeatures.EnableChangedRemoveFocus = false;
            monitorFeatures.EnableChangedDistance = false;
            monitorFeatures.EnableDumpVisionFrame = false;
            monitorFeatures.EnableOutput = false;
            monitorFeatures.EnableTrace = false;
            monitorFeatures.EnableDebug = false;
            monitorFeatures.EnableInfo = false;
            monitorFeatures.EnableWarn = false;
            monitorFeatures.EnableError = false;
            monitorFeatures.EnableFatal = false;
        }
    }
}

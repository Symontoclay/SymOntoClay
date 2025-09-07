namespace SymOntoClay.Monitor.Helpers
{
    public static class MonitorFeaturesHelper
    {
        public static void SetAllFeaturesEnabled(MonitorFeatures monitorFeatures)
        {
            monitorFeatures.EnableCallMethod = true;
            monitorFeatures.EnableParameter = true,
            monitorFeatures.EnableEndCallMethod = true,
            monitorFeatures.EnableMethodResolving = true,
            monitorFeatures.EnableEndMethodResolving = true,
            monitorFeatures.EnableActionResolving = true,
            monitorFeatures.EnableEndActionResolving = true,
            monitorFeatures.EnableHostMethodResolving = true,
            monitorFeatures.EnableEndHostMethodResolving = true,
            monitorFeatures.EnableHostMethodActivation = true,
            monitorFeatures.EnableEndHostMethodActivation = true,
            monitorFeatures.EnableHostMethodStarting = true,
            monitorFeatures.EnableEndHostMethodStarting = true,
            monitorFeatures.EnableHostMethodExecution = true,
            monitorFeatures.EnableEndHostMethodExecution = true,
            monitorFeatures.EnableSystemExpr = true,
            monitorFeatures.EnableCodeFrame = true,
            monitorFeatures.EnableLeaveThreadExecutor = true,
            monitorFeatures.EnableGoBackToPrevCodeFrame = true,
            monitorFeatures.EnableStartProcessInfo = true,
            monitorFeatures.EnableCancelProcessInfo = true,
            monitorFeatures.EnableWeakCancelProcessInfo = true,
            monitorFeatures.EnableCancelInstanceExecution = true,
                    EnableSetExecutionCoordinatorStatus = true,
                    EnableSetProcessInfoStatus = true,
                    EnableWaitProcessInfo = true,
                    EnableRunLifecycleTrigger = true,
                    EnableDoTriggerSearch = true,
                    EnableEndDoTriggerSearch = true,
                    EnableSetConditionalTrigger = true,
                    EnableResetConditionalTrigger = true,
                    EnableRunSetExprOfConditionalTrigger = true,
                    EnableEndRunSetExprOfConditionalTrigger = true,
                    EnableRunResetExprOfConditionalTrigger = true,
                    EnableEndRunResetExprOfConditionalTrigger = true,
                    EnableActivateIdleAction = true,
                    EnableLogicalSearchExplain = true,
                    EnableAddFactOrRuleTriggerResult = true,
                    EnableAddFactToLogicalStorage = true,
                    EnableRemoveFactFromLogicalStorage = true,
                    EnableRefreshLifeTimeInLogicalStorage = true,
                    EnablePutFactForRemovingFromLogicalStorage = true,
                    EnableThreadTask = true,
                    EnableHtn = true,
                    EnableBuildPlan = true,
                    EnableOutput = true,
                    EnableTrace = true,
                    EnableDebug = true,
                    EnableInfo = true,
                    EnableWarn = true,
                    EnableError = true,
                    EnableFatal = true
        }

        public static void SetAllFeaturesDisabled(MonitorFeatures monitorFeatures)
        {

        }
    }
}

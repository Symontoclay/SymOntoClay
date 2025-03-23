namespace SymOntoClay.BaseTestLib
{
    public interface INewBehaviorTestEngineInstanceBuilder
    {
        INewBehaviorTestEngineInstanceBuilder UseDefaultRootDirectory();
        INewBehaviorTestEngineInstanceBuilder UseCustomRootDirectory(string rootDir);
        INewBehaviorTestEngineInstanceBuilder DontUseStandardLibrary();
        INewBehaviorTestEngineInstanceBuilder SetUsingStandardLibrary(KindOfUsingStandardLibrary useStandardLibrary);
        INewBehaviorTestEngineInstanceBuilder DontUseTimeoutToEnd();
        INewBehaviorTestEngineInstanceBuilder UseDefaultTimeoutToEnd();
        INewBehaviorTestEngineInstanceBuilder UseTimeoutToEnd(int timeoutToEnd);
        INewBehaviorTestEngineInstanceBuilder DisableHtnPlanExecution();
        INewBehaviorTestEngineInstanceBuilder EnableHtnPlanExecution();
        INewBehaviorTestEngineInstanceBuilder SethHtnIterationsMaxCount(int htnIterationsMaxCount);
        INewBehaviorTestEngineInstanceBuilder DontUsePlatformListener();
        INewBehaviorTestEngineInstanceBuilder UsePlatformListener(object platformListener);
    }
}

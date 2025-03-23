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
        INewBehaviorTestEngineInstanceBuilder EnableNlp();
        INewBehaviorTestEngineInstanceBuilder DisableNlp();
        INewBehaviorTestEngineInstanceBuilder DisableCategories();
        INewBehaviorTestEngineInstanceBuilder UseCategories(List<string> categories);
        INewBehaviorTestEngineInstanceBuilder DontUsePlatformListener();
        INewBehaviorTestEngineInstanceBuilder UsePlatformListener(object platformListener);
        INewBehaviorTestEngineInstanceBuilder TestedCode(string fileContent);
        INewBehaviorTestEngineInstanceBuilder LogHandler(Action<int, string> handler);
        INewBehaviorTestEngineInstanceBuilder LogHandler(Func<int, string, bool> handler);
        INewBehaviorTestEngineInstanceBuilder LogHandler(Action<string> handler);
        INewBehaviorTestEngineInstanceBuilder LogHandler(Func<string, bool> handler);
        INewBehaviorTestEngineInstanceBuilder ErrorHandler(Action<string> handler);
        INewBehaviorTestEngineInstanceBuilder ErrorHandler(Func<string, bool> handler);

        INewBehaviorTestEngineInstance Build();
    }
}

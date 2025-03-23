namespace SymOntoClay.BaseTestLib
{
    public interface INewBehaviorTestEngineInstanceBuilder
    {
        /// <summary>
        /// Sets using default root directory.
        /// Its a temporary directory with a random name.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        INewBehaviorTestEngineInstanceBuilder UseDefaultRootDirectory();

        /// <summary>
        /// Sets using a custom root directory.
        /// </summary>
        /// <param name="rootDir">Set root directory.</param>
        /// <returns>The instance of the builder.</returns>
        INewBehaviorTestEngineInstanceBuilder UseCustomRootDirectory(string rootDir);

        /// <summary>
        /// Sets not to use standard library.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        INewBehaviorTestEngineInstanceBuilder DontUseStandardLibrary();

        /// <summary>
        /// Sets using standard library mode.
        /// </summary>
        /// <param name="useStandardLibrary">Set using standard library mode.</param>
        /// <returns>The instance of the builder.</returns>
        INewBehaviorTestEngineInstanceBuilder SetUsingStandardLibrary(KindOfUsingStandardLibrary useStandardLibrary);

        /// <summary>
        /// Disables using timeout in the Run method of created instance.
        /// Only for using with handlers which return boolean result.
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        INewBehaviorTestEngineInstanceBuilder DontUseTimeoutToEnd();

        /// <summary>
        /// Enables using timeout and sets 5000 milliseconds as a timeout.
        /// Only for using with handlers which don't return any result.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        INewBehaviorTestEngineInstanceBuilder UseDefaultTimeoutToEnd();

        /// <summary>
        /// Enables using timeout and sets a custom timeout.
        /// Only for using with handlers which don't return any result.
        /// </summary>
        /// <param name="timeoutToEnd">Set custom timeout.</param>
        /// <returns>The instance of the builder.</returns>
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

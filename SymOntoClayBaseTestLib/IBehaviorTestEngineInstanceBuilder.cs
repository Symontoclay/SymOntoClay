namespace SymOntoClay.BaseTestLib
{
    public interface IBehaviorTestEngineInstanceBuilder
    {
        /// <summary>
        /// Sets using default root directory.
        /// Its a temporary directory with a random name.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder UseDefaultRootDirectory();

        /// <summary>
        /// Sets using a custom root directory.
        /// </summary>
        /// <param name="rootDir">Set root directory.</param>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder UseCustomRootDirectory(string rootDir);

        /// <summary>
        /// Sets not to use standard library.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder DontUseStandardLibrary();

        /// <summary>
        /// Sets using standard library mode.
        /// </summary>
        /// <param name="useStandardLibrary">Set using standard library mode.</param>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder SetUsingStandardLibrary(KindOfUsingStandardLibrary useStandardLibrary);

        /// <summary>
        /// Disables using timeout in the Run method of created instance.
        /// Only for using with handlers which return boolean result.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder DontUseTimeoutToEnd();

        /// <summary>
        /// Enables using timeout and sets 5000 milliseconds as a timeout.
        /// Only for using with handlers which don't return any result.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder UseDefaultTimeoutToEnd();

        /// <summary>
        /// Enables using timeout and sets a custom timeout.
        /// Only for using with handlers which don't return any result.
        /// </summary>
        /// <param name="timeoutToEnd">Set custom timeout.</param>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder UseTimeoutToEnd(int timeoutToEnd);

        /// <summary>
        /// Disables HTN execution.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder DisableHtnPlanExecution();

        /// <summary>
        /// Enables HTN execution.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder EnableHtnPlanExecution();

        /// <summary>
        /// Enables HTN execution, but limits count of iterations.
        /// </summary>
        /// <param name="htnIterationsMaxCount">Set limit of count of iterations.</param>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder SethHtnIterationsMaxCount(int htnIterationsMaxCount);

        /// <summary>
        /// Enables NLP.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder EnableNlp();

        /// <summary>
        /// Disables NLP.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder DisableNlp();

        /// <summary>
        /// Disables categories.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder DisableCategories();

        /// <summary>
        /// Enables categories and sets a list with used categories.
        /// </summary>
        /// <param name="categories">Set list with used categories.</param>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder UseCategories(List<string> categories);

        /// <summary>
        /// Resets platform listener.
        /// It is the default setting.
        /// </summary>
        /// <returns>The instance of the builder (self reference).</returns>
        IBehaviorTestEngineInstanceBuilder DontUsePlatformListener();
        IBehaviorTestEngineInstanceBuilder UsePlatformListener(object platformListener);
        IBehaviorTestEngineInstanceBuilder TestedCode(string fileContent);
        IBehaviorTestEngineInstanceBuilder LogHandler(Action<int, string> handler);
        IBehaviorTestEngineInstanceBuilder LogHandler(Func<int, string, bool> handler);
        IBehaviorTestEngineInstanceBuilder LogHandler(Action<string> handler);
        IBehaviorTestEngineInstanceBuilder LogHandler(Func<string, bool> handler);
        IBehaviorTestEngineInstanceBuilder ErrorHandler(Action<string> handler);

        /// <summary>
        /// Bild test engine instance.
        /// </summary>
        /// <returns>The built instance.</returns>
        IBehaviorTestEngineInstance Build();
    }
}

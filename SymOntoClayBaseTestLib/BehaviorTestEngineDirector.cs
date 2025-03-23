namespace SymOntoClay.BaseTestLib
{
    public static class BehaviorTestEngineDirector
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static IBehaviorTestEngineInstance CreateMinimalInstance(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }
    }
}

namespace SymOntoClay.BaseTestLib
{
    public static class NewBehaviorTestEngineDirector
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static INewBehaviorTestEngineInstance CreateMinimalInstance(this INewBehaviorTestEngineInstanceBuilder builder, string fileContent, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }
    }
}

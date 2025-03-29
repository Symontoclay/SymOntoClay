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

        public static IBehaviorTestEngineInstance CreateMinimalInstance(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Action<int, string> logHandler)
        {
            builder.UseDefaultTimeoutToEnd();
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithCategories(this IBehaviorTestEngineInstanceBuilder builder,
            string fileContent, List<string> categories, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.UseCategories(categories);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithImportStandardLibrary(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.SetUsingStandardLibrary(KindOfUsingStandardLibrary.Import);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }
    }
}

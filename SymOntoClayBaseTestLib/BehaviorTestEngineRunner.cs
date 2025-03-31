namespace SymOntoClay.BaseTestLib
{
    public static class BehaviorTestEngineRunner
    {
        public static bool RunMinimalInstance(string fileContent, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstance(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceTimeoutBased(string fileContent, Action<int, string> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstance(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceTimeoutBased(string fileContent, Action<string> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstance(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithCategories(string fileContent, List<string> categories, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithCategories(fileContent, categories, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithImportStandardLibrary(string fileContent, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithImportStandardLibrary(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceTimeoutBasedWithImportStandardLibrary(string fileContent, Action<int, string> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithImportStandardLibrary(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithPlatformListener(string fileContent, Func<int, string, bool> logHandler, object platformListener)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithPlatformListener(fileContent, logHandler, platformListener);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceTimeoutBasedWithPlatformListener(string fileContent, Action<int, string> logHandler, object platformListener)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithPlatformListener(fileContent, logHandler, platformListener);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithOneHtnIteration(string fileContent, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithOneHtnIteration(fileContent, logHandler);
            return testInstance.Run();
        }
    }
}

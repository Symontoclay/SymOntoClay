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
    }
}

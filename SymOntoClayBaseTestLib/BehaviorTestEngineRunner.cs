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
    }
}

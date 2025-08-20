namespace SymOntoClay.BaseTestLib
{
    public static class BehaviorTestEngineInstanceHelper
    {
        public const int TimeoutInMinutes = 5;

        public static bool ShouldBeCancelledByTimeout(DateTime lastIterationTime)
        {
            return DateTime.Now.Subtract(lastIterationTime).Minutes > TimeoutInMinutes;
        }
    }
}

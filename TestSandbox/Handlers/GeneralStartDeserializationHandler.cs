namespace TestSandbox.Handlers
{
    public class GeneralStartDeserializationHandler : BaseGeneralStartHandler
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public GeneralStartDeserializationHandler() 
        { 
        }
    }
}

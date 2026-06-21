namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorStackHandler
    {
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _globalLogger.Info("Begin");

            _globalLogger.Info("End");
        }

        private void Case1()
        {
            var list = new ThreadExecutorList();

            var executor = new ThreadExecutorStub();



        }
    }
}

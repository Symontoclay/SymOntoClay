namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorStackHandler
    {
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _globalLogger.Info("Begin");

            Case1();

            _globalLogger.Info("End");
        }

        private void Case1()
        {
            var list = new ThreadExecutorList();

            var executor = new ThreadExecutorStub();

            list.Add(executor);

            executor.Run();
        }
    }
}

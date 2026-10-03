using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System.Threading;
using TestSandbox.ThreadExecutorStackInv;

namespace TestSandbox.Handlers
{
    public class TstAltFunctorHandler
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
            var logger = new MonitorLoggerNLogImplementation();

            var serializationAnchor = new SerializationAnchor();

            var cancellationTokenSourceContext = new CancellationTokenSourceContext();

            var threadingSettings = new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 1
            };

            var threadPool = new CustomThreadPool(threadingSettings, cancellationTokenSourceContext);

            LoggedAltFunctorWithoutResult.Run(logger, "2C2D2518-9C7B-4ED5-AB8C-70B78B8A7784",
            (IMonitorLogger loggerValue) => {
                loggerValue.Info("71AADDDC-83F9-4E90-8F5D-621873D2DC0B", "Run!!!!!");
            },
            threadPool, cancellationTokenSourceContext, serializationAnchor);

            Thread.Sleep(1000);
        }
    }
}

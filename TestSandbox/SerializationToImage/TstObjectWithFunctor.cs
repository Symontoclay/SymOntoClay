using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Core;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System.Threading;

namespace TestSandbox.SerializationToImage
{
    [WorldRoot]
    public class TstObjectWithFunctor
    {
        public TstObjectWithFunctor()
        {
            _logger = new MonitorLoggerNLogImplementation();
            _serializationAnchor = new SerializationAnchor();
            _cancellationTokenSourceContext = new CancellationTokenSourceContext();
            _threadingSettings = new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 1
            };
            _threadPool = new CustomThreadPool(_threadingSettings, _cancellationTokenSourceContext);
            _loggedAltFunctorWithoutResult = new LoggedAltFunctorWithoutResult(_logger, "60C84C46-066E-4574-B481-CDFB44D85FF4",
                (IMonitorLogger loggerValue) => {
                    loggerValue.Info("FDE3FD00-3B5B-4F07-A77C-EF88C108B8D0", "Run!!!!!");
                },
                _threadPool, _cancellationTokenSourceContext, _serializationAnchor);
        }

        private IMonitorLogger _logger;
        private SerializationAnchor _serializationAnchor;
        private CancellationTokenSourceContext _cancellationTokenSourceContext;
        private CustomThreadPoolSettings _threadingSettings;
        private CustomThreadPool _threadPool;
        private LoggedAltFunctorWithoutResult _loggedAltFunctorWithoutResult;

        public void Run()
        {
            _loggedAltFunctorWithoutResult.Run();
        }
    }
}

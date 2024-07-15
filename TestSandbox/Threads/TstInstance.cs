using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;

namespace TestSandbox.Threads
{
    public class TstInstance
    {
        private class InternalData
        {

        }

        public TstInstance(IMonitorLogger logger, IActiveObjectContext activeObjectContext, ICustomThreadPool threadPool)
        {
            _logger = logger;
            _activeObjectContext = activeObjectContext;
            _threadPool = threadPool;
            _data = new InternalData();
        }

        private readonly IMonitorLogger _logger;
        private readonly IActiveObjectContext _activeObjectContext;
        private readonly ICustomThreadPool _threadPool;
        private readonly InternalData _data;

        private class Method1LocalContext
        {
        }

        public IMethodResponse Method1()
        {
            return LoggedCodeChunkFunctorWithoutResult<InternalData, Method1LocalContext>.Run(_logger, "9090DD1A-81C0-4789-889A-9D7A24D8DDD0", _data,
                (loggerValue, codeChunksContext, globalContextValue, localContextValue) => {
                    codeChunksContext.CreateCodeChunk("240D8A34-918B-4949-AFAF-7A72F5F8AF93", () =>
                    {
                        loggerValue.Info("33BED06D-A69C-4945-85F7-0BB17FAB8EC4", "Chunk1");
                    });

                    codeChunksContext.CreateCodeChunk("F1B20977-05AB-400A-94FD-CFBE5B84B5DF", (currentCodeChunk) =>
                    {
                        _logger.Info("819B488D-8752-4337-A4F1-621D8471EF6B", "Chunk2");

                        SyncMethodReponseResolver.Run(loggerValue, "413EB4F7-56FD-44C2-813B-8A35F1C65DCA", currentCodeChunk, () => {
                            return PrivateMethod1(globalContextValue, currentCodeChunk);
                        });
                    });
                }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        private static IMethodResponse PrivateMethod1(InternalData data, ICodeChunk currentCodeChunk)
        {
            return LoggedSincCodeChunkFunctorWithoutResult.Run(() => {
                throw new NotImplementedException("F1E9F6E2-BED9-451E-9CCC-B87B2E706F92");
            }).ToMethodResponse();
        }
    }
}

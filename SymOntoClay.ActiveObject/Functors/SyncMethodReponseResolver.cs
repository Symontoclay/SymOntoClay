using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public class SyncMethodReponseResolver
    {
        public static SyncMethodReponseResolver Run(IMonitorLogger logger, string id, ICodeChunk codeChunk, Func<IMethodResponse> func)
        {
            var resolver = new SyncMethodReponseResolver(logger, id, codeChunk, func);
            resolver.Run();
            return resolver;
        }

        public SyncMethodReponseResolver(IMonitorLogger logger, string id, ICodeChunk codeChunk, Func<IMethodResponse> func)
        {
            _logger = logger;
            _id = id;
            _codeChunk = codeChunk;
            _func = func;
        }

        private readonly IMonitorLogger _logger;
        private readonly string _id;
        private readonly ICodeChunk _codeChunk;
        private readonly Func<IMethodResponse> _func;
        private IMethodResponse _methodResponse;

        public void Run()
        {
            if(_methodResponse == null)
            {
                _methodResponse = _func();
            }

            _methodResponse.Wait();
        }
    }
}

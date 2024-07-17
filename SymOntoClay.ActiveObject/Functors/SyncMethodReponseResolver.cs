using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public class SyncMethodReponseResolver
    {
        public static void Run(IMonitorLogger logger, string id, ICodeChunk codeChunk, Func<IMethodResponse> func)
        {
            var resolver = new SyncMethodReponseResolver(logger, id, codeChunk, func);
            resolver.Run();
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

    public class SyncMethodReponseResolver<TResult>
    {
        public static TResult Run(IMonitorLogger logger, string id, ICodeChunk codeChunk, Func<IMethodResponse<TResult>> func)
        {
            var resolver = new SyncMethodReponseResolver<TResult>(logger, id, codeChunk, func);
            return resolver.Run();
        }

        public SyncMethodReponseResolver(IMonitorLogger logger, string id, ICodeChunk codeChunk, Func<IMethodResponse<TResult>> func)
        {
            _logger = logger;
            _id = id;
            _codeChunk = codeChunk;
            _func = func;
        }

        private readonly IMonitorLogger _logger;
        private readonly string _id;
        private readonly ICodeChunk _codeChunk;
        private readonly Func<IMethodResponse<TResult>> _func;
        private IMethodResponse<TResult> _methodResponse;

        public TResult Run()
        {
            if (_methodResponse == null)
            {
                _methodResponse = _func();
            }

            _methodResponse.Wait();

            return _methodResponse.Result;
        }
    }
}

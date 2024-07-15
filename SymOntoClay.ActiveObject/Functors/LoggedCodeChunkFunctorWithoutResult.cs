using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.ActiveObject.Functors
{
    public class LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext> : BaseFunctor
        where TLocalContext : class, new()
    {
        public LoggedCodeChunkFunctorWithoutResult(IMonitorLogger logger, string codeChunksContextId, TGlobalContext globalContext, 
            Action<IMonitorLogger, CodeChunksContext, TGlobalContext, TLocalContext> action, 
            IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, context, threadPool)
        {
            _codeChunksContext = new CodeChunksContext(codeChunksContextId);
            _action = action;
            _logger = logger;
            _globalContext = globalContext;
            _localContext = new TLocalContext();
        }

        private readonly Action<IMonitorLogger, CodeChunksContext, TGlobalContext, TLocalContext> _action;
        private readonly CodeChunksContext _codeChunksContext;
        private readonly IMonitorLogger _logger;
        private readonly TGlobalContext _globalContext;
        private readonly TLocalContext _localContext;

        /// <inheritdoc/>
        protected override void OnRun(CancellationToken cancellationToken)
        {
            _action(_logger, _codeChunksContext, _globalContext, _localContext);

            _codeChunksContext.Run();
        }
    }
}

using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System.Threading;
using System;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;

namespace SymOntoClay.ActiveObject.Functors
{
    //public class LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext> : BaseFunctor
    //    where TLocalContext : class, new()
    //{
    //    public static LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext> Run(IMonitorLogger logger, string codeChunksContextId, TGlobalContext globalContext,
    //        Action<CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action,
    //        IActiveObjectContext context, ICustomThreadPool threadPool)
    //    {
    //        var functor = new LoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext>(logger, codeChunksContextId, globalContext,
    //        action, context, threadPool);
    //        functor.Run();
    //        return functor;
    //    }

    //    public LoggedCodeChunkFunctorWithoutResult(IMonitorLogger logger, string codeChunksContextId, TGlobalContext globalContext,
    //        Action<CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> action,
    //        IActiveObjectContext context, ICustomThreadPool threadPool)
    //        : base(logger, context, threadPool)
    //    {
    //        _localContext = new TLocalContext();

    //        _codeChunksContext = new CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>(codeChunksContextId, logger, globalContext, _localContext);
    //        _action = action;
    //        _logger = logger;
    //        _globalContext = globalContext;
    //    }

    //    private readonly Action<CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext>> _action;
    //    private readonly CodeChunksContext<IMonitorLogger, TGlobalContext, TLocalContext> _codeChunksContext;
    //    private readonly IMonitorLogger _logger;
    //    private readonly TGlobalContext _globalContext;
    //    private readonly TLocalContext _localContext;

    //    /// <inheritdoc/>
    //    protected override void OnRun(CancellationToken cancellationToken)
    //    {
    //        _action(_codeChunksContext);

    //        _codeChunksContext.Run();
    //    }
    //}
}

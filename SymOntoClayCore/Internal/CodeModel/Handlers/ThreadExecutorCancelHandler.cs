using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel.Handlers
{
    public class ThreadExecutorCancelHandler : BaseSpecialHandler
    {
        public ThreadExecutorCancelHandler(IThreadExecutor threadExecutor)
        { 
            _threadExecutor = threadExecutor;
        }

        private readonly IThreadExecutor _threadExecutor;

        /// <inheritdoc/>
        public override ValueCallResult Call(IMonitorLogger logger, KindOfValueConversion kindOfValueConversion, IList<Value> paramsList, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return NCall(logger);
        }

        /// <inheritdoc/>
        public override ValueCallResult Call(IMonitorLogger logger, KindOfValueConversion kindOfValueConversion, IDictionary<string, Value> paramsDict, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            return NCall(logger);
        }

        private ValueCallResult NCall(IMonitorLogger logger)
        {
            _threadExecutor.Cancel();

            return new ValueCallResult(NullValue.Instance);
        }
    }
}

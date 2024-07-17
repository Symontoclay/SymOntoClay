using SymOntoClay.ActiveObject.Functors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public class MethodResponseOfBaseLoggedSyncCodeChunkFunctor : IMethodResponse
    {
        public MethodResponseOfBaseLoggedSyncCodeChunkFunctor(BaseLoggedSyncCodeChunkFunctor source)
        {
            _source = source;
        }

        private readonly BaseLoggedSyncCodeChunkFunctor _source;

        /// <inheritdoc/>
        public Task Task => Task.CompletedTask;

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }

    public class MethodResponseOfBaseLoggedSyncCodeChunkFunctor<TResult> : IMethodResponse<TResult>
    {
        public MethodResponseOfBaseLoggedSyncCodeChunkFunctor(BaseLoggedSyncCodeChunkFunctor<TResult> source)
        {
            _source = source;
        }

        private readonly BaseLoggedSyncCodeChunkFunctor<TResult> _source;

        /// <inheritdoc/>
        public Task<TResult> Task => throw new NotImplementedException("37A806A9-FB63-48DA-B164-F9E1458665E7");

        /// <inheritdoc/>
        public TResult Result => throw new NotImplementedException("15835EAD-2EBB-4862-B224-2F45453EF624");

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }
}

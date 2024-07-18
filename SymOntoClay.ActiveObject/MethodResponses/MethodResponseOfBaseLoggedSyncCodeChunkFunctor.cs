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
        public Task<TResult> Task => Task<TResult>.FromResult(_source.GetResult());

        /// <inheritdoc/>
        public TResult Result => _source.GetResult();

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }
}

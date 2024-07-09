using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public class MethodResponseOfBaseFunctor : IMethodResponse
    {
        public MethodResponseOfBaseFunctor(BaseFunctor source)
        {
            _source = source;
        }

        private BaseFunctor _source;

        /// <inheritdoc/>
        public Task Task => _source.TaskValue.StandardTask;
    }

    public class MethodResponseOfBaseFunctor<TResult> : IMethodResponse<TResult>
    {
        public MethodResponseOfBaseFunctor(BaseFunctor<TResult> source)
        {
            _source = source;
        }

        private BaseFunctor<TResult> _source;

        /// <inheritdoc/>
        public Task<TResult> Task => _source.TaskValue.StandardTaskWithResult;

        /// <inheritdoc/>
        public TResult Result => _source.Result;
    }
}

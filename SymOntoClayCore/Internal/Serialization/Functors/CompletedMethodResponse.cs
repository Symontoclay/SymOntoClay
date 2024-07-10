using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public class CompletedMethodResponse : IMethodResponse
    {
        public static CompletedMethodResponse Instance = new CompletedMethodResponse();

        /// <inheritdoc/>
        public Task Task => Task.CompletedTask;

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }

    public class CompletedMethodResponse<TResult> : IMethodResponse<TResult>
    {
        public static CompletedMethodResponse<TResult> Instance = new CompletedMethodResponse<TResult>();

        public CompletedMethodResponse()
            : this(default(TResult))
        {
        }

        public CompletedMethodResponse(TResult result)
        {
            _result = result;
        }

        private readonly TResult _result;

        /// <inheritdoc/>
        public Task<TResult> Task => Task<TResult>.FromResult(_result);

        /// <inheritdoc/>
        public TResult Result => _result;

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }
}

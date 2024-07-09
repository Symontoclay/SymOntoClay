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

        /// <inheritdoc/>
        public Task<TResult> Task => Task<TResult>.FromResult(default(TResult));

        /// <inheritdoc/>
        public TResult Result => default;

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }
}

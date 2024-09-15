using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public class CompletedSyncMethodResponse: ISyncMethodResponse
    {
        public static CompletedSyncMethodResponse Instance = new CompletedSyncMethodResponse();
    }

    public class CompletedSyncMethodResponse<TResult> : ISyncMethodResponse<TResult>
    {
        public CompletedSyncMethodResponse()
            : this(default(TResult))
        {
        }

        public CompletedSyncMethodResponse(TResult result)
        {
            _result = result;
        }

        private readonly TResult _result;

        /// <inheritdoc/>
        public TResult Result => _result;
    }
}

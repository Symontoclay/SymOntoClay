using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public abstract partial class BaseCodeChunkWithResult<TResult>
    {
        protected BaseCodeChunkWithResult(ICodeChunksContextWithResult<TResult> codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        protected abstract void OnRunAction();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunkWithResult<TResult> : ICodeChunk
    {
        public CodeChunkWithResult(CodeChunksContext codeChunksFactory, string id, Func<TResult> func)
        {
            _codeChunksFactory = codeChunksFactory;
            _func = func;
        }

        private bool _wasExecuted;
        private readonly CodeChunksContext _codeChunksFactory;
        private readonly Func<TResult> _func;

        /// <inheritdoc/>
        public void Run()
        {
            if (_wasExecuted)
            {
                return;
            }

            _wasExecuted = true;

            _func();
        }
    }
}

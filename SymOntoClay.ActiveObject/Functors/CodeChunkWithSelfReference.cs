using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunkWithSelfReference: ICodeChunk
    {
        public CodeChunkWithSelfReference(CodeChunksContext codeChunksFactory, string id, Action<ICodeChunk> action)
        {
            _codeChunksFactory = codeChunksFactory;
            _action = action;
        }

        private bool _wasExecuted;
        private readonly CodeChunksContext _codeChunksFactory;
        private readonly Action<ICodeChunk> _action;

        /// <inheritdoc/>
        public void Run()
        {
            if (_wasExecuted)
            {
                return;
            }

            _wasExecuted = true;

            _action(this);
        }
    }
}

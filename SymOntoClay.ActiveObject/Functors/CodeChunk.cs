using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunk
    {
        public CodeChunk(CodeChunksContext codeChunksFactory, Action<CodeChunk> action)
        {
            _codeChunksFactory = codeChunksFactory;
            _action = action;
        }

        private bool _wasExecuted;
        private readonly CodeChunksContext _codeChunksFactory;
        private readonly Action<CodeChunk> _action;

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

using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunk : ICodeChunk
    {
        public CodeChunk(ICodeChunksContext codeChunksFactory, string id, Action action)
        {
            _codeChunksFactory = codeChunksFactory;
            _action = action;
        }

        private bool _wasExecuted;
        private readonly ICodeChunksContext _codeChunksFactory;
        private readonly Action _action;

        /// <inheritdoc/>
        public void Run()
        {
            if (_wasExecuted)
            {
                return;
            }

            _wasExecuted = true;

            _action();
        }
    }
}

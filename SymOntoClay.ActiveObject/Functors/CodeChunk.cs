using System;
using System.Collections.Generic;

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
        private readonly List<ICodeChunk> _children = new List<ICodeChunk>();

        /// <inheritdoc/>
        public void AddChild(ICodeChunk child)
        {
            if(_children.Contains(child))
            {
                return;
            }

            _children.Add(child);
        }

        /// <inheritdoc/>
        public void Run()
        {
            if (_wasExecuted)
            {
                return;
            }

            _action();

            foreach (var child in _children)
            {
                child.Run();
            }

            _wasExecuted = true;
        }
    }
}

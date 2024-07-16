using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunkWithSelfReference : ICodeChunk
    {
        public CodeChunkWithSelfReference(ICodeChunksContext codeChunksFactory, string id, Action<ICodeChunk> action)
        {
            _codeChunksFactory = codeChunksFactory;
            _action = action;
        }

        private bool _isFinished;
        private bool _actionIsFinished;
        private readonly ICodeChunksContext _codeChunksFactory;
        private readonly Action<ICodeChunk> _action;
        private readonly List<ICodeChunk> _children = new List<ICodeChunk>();

        /// <inheritdoc/>
        public void AddChild(ICodeChunk child)
        {
            if (_children.Contains(child))
            {
                return;
            }

            _children.Add(child);
        }

        /// <inheritdoc/>
        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            if (!_actionIsFinished)
            {
                _action(this);

                _actionIsFinished = true;
            }
            
            foreach(var child in _children)
            {
                if (child.IsFinished)
                {
                    continue;
                }

                child.Run();
            }

            _isFinished = true;
        }

        /// <inheritdoc/>
        public bool IsFinished => _isFinished;
    }
}

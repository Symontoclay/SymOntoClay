using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseCodeChunkWithSelfReference : ICodeChunkWithSelfReference
    {
        private bool _isFinished;
        private bool _actionIsFinished;
        private List<IBaseCodeChunk> _children = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action action);

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);

        protected void AddChildCodeChunk(IBaseCodeChunk chunk)
        {
            _children.Add(chunk);
        }

        protected abstract void OnRunAction();

        /// <inheritdoc/>
        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            if (!_actionIsFinished)
            {
                OnRunAction();

                _actionIsFinished = true;
            }

            foreach (var child in _children)
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

﻿using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public abstract partial class BaseCodeChunkWithResultAndSelfReference<TResult>: ICodeChunkWithResultAndSelfReference<TResult>
    {
        protected BaseCodeChunkWithResultAndSelfReference(ICodeChunksContextWithResult<TResult> codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        private bool _isFinished;
        private bool _actionIsFinished;
        
        private ICodeChunksContextWithResult<TResult> _codeChunksContext;

        private List<IBaseCodeChunk> _children = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action action);

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);

        protected void AddChildCodeChunk(IBaseCodeChunk chunk)
        {
            _children.Add(chunk);
        }

        /// <inheritdoc/>
        public void Finish(TResult result)
        {
            _isFinished = true;
            _codeChunksContext.Finish(result);
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

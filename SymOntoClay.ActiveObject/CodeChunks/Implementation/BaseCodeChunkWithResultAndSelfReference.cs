using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract class BaseCodeChunkWithResultAndSelfReference<TResult>
    {
        protected BaseCodeChunkWithResultAndSelfReference(IBaseCodeChunksContextWithResult<TResult> codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        private bool _isFinished;
        private bool _actionIsFinished;

        private IBaseCodeChunksContextWithResult<TResult> _codeChunksContext;

        private List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        protected void AddChildCodeChunk(IBaseCodeChunk chunk)
        {
            _chunks.Add(chunk);
        }

        public void Finish(TResult result)
        {
            _isFinished = true;
            _codeChunksContext.Finish(result);
        }

        protected abstract void OnRunAction();

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

            if (_isFinished)
            {
                return;
            }

            foreach (var chunk in _chunks)
            {
                if (_isFinished)
                {
                    return;
                }

                if (chunk.IsFinished)
                {
                    continue;
                }

                chunk.Run();
            }

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}

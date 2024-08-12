using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public abstract partial class BaseCodeChunksContextWithResult<TResult>: ICodeChunksContextWithResult<TResult>
    {
        private readonly List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action action);

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);

        protected void AddCodeChunk(IBaseCodeChunk chunk)
        {
            _chunks.Add(chunk);
        }

        /// <inheritdoc/>
        public void Finish(TResult result)
        {
            _result = result;
            _isFinished = true;
        }

        private bool _isFinished;
        private TResult _result = default(TResult);

        /// <inheritdoc/>
        public TResult Result => _result;

        /// <inheritdoc/>
        public void Run()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Run();

                if (_isFinished)
                {
                    return;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsFinished => _isFinished;
    }
}

using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public abstract partial class BaseCodeChunksContext: ICodeChunksContext
    {
        private List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action action);

        /// <inheritdoc/>
        public abstract void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);

        protected void AddCodeChunk(IBaseCodeChunk chunk)
        {
            _chunks.Add(chunk);
        }

        /// <inheritdoc/>
        public void Finish()
        {
            _isFinished = true;
        }

        private bool _isFinished;

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
    }
}

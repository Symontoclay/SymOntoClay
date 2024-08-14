using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseCodeChunksContextWithResult<TResult>
    {
        private readonly List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        protected void AddCodeChunk(IBaseCodeChunk chunk)
        {
            _chunks.Add(chunk);
        }

        public void Finish(TResult result)
        {
            _result = result;
            _isFinished = true;
        }

        private bool _isFinished;
        private TResult _result = default;

        public TResult Result => _result;

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

        public bool IsFinished => _isFinished;
    }
}

using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract class BaseCodeChunksContext
    {
        private List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        protected void AddCodeChunk(IBaseCodeChunk chunk)
        {
            _chunks.Add(chunk);
        }

        public void Finish()
        {
            _isFinished = true;
        }

        private bool _isFinished;

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

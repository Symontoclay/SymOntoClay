using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunksContext: ICodeChunksContext
    {
        public CodeChunksContext(string id)
        {

        }

        private readonly List<ICodeChunk> _chunks = new List<ICodeChunk>();

        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunk(this, chunkId, action);
            _chunks.Add(chunk);
        }

        public void CreateCodeChunk(string chunkId, ICodeChunk parent, Action action)
        {
            var chunk = new CodeChunk(this, chunkId, action);
            parent.AddChild(chunk);
        }

        public void CreateCodeChunk(string chunkId, Action<ICodeChunk> action)
        {
            var chunk = new CodeChunkWithSelfReference(this, chunkId, action);
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

                if(_isFinished)
                {
                    return;
                }
            }
        }
    }

    public class CodeChunksContext<TResult> : ICodeChunksContext
    {
        public CodeChunksContext(string id)
        {

        }

        private readonly List<ICodeChunk> _chunks = new List<ICodeChunk>();

        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunk(this, chunkId, action);
            _chunks.Add(chunk);
        }

        public void CreateCodeChunk(string chunkId, Action<ICodeChunk> action)
        {
            var chunk = new CodeChunkWithSelfReference(this, chunkId, action);
            _chunks.Add(chunk);
        }

        public void Finish(TResult result)
        {
            _result = result;
            _isFinished = true;
        }

        private bool _isFinished;
        private TResult _result = default(TResult);

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
    }
}

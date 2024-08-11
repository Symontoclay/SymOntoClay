using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public class CodeChunksContextWithResult<TResult> : ICodeChunksContextWithResult<TResult>
    {
        public CodeChunksContextWithResult(string id)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;

        private readonly List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunkWithResult<TResult>(chunkId, this, action);
            _chunks.Add(chunk);
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            var chunk = new CodeChunkWithResultAndSelfReference<TResult>(chunkId, this, action);
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

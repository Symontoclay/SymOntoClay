using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class CodeChunksContextWithResult<TResult> : BaseCodeChunksContextWithResult<TResult>, ICodeChunksContextWithResult<TResult>
    {
        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            AddCodeChunk(new CodeChunkWithResult<TResult>(chunkId, this, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            AddCodeChunk(new CodeChunkWithResultAndSelfReference<TResult>(chunkId, this, action));
        }
    }

    public class CodeChunksContextWithResult<T1, T2, T3, TResult> : BaseCodeChunksContextWithResult<TResult>, ICodeChunksContextWithResult<T1, T2, T3, TResult>
    {
        public CodeChunksContextWithResult(T1 arg1, T2 arg2, T3 arg3)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action)
        {
            AddCodeChunk(new CodeChunkWithResult<T1, T2, T3, TResult>(chunkId, this, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action)
        {
            AddCodeChunk(new CodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>(chunkId, this, _arg1, _arg2, _arg3, action));
        }
    }
}

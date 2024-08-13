using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunksContextWithResult<TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);
    }
}

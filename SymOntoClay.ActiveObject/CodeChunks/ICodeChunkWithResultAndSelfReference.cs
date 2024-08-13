using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunkWithResultAndSelfReference<TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);
    }
}

using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunkWithResultAndSelfReference<TResult> : IBaseCodeChunkWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);
    }
}

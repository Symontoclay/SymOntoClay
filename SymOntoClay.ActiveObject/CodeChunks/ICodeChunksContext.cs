using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunksContext : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
    }

    public interface ICodeChunksContext<T1, T2, T3> : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action);
    }
}

using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunkWithSelfReference : IBaseCodeChunkWithoutResult
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
    }

    public interface ICodeChunkWithSelfReference<T> : IBaseCodeChunkWithoutResult
    {
        void CreateCodeChunk(string chunkId, Action<T> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T>, T> action);
    }

    public interface ICodeChunkWithSelfReference<T1, T2> : IBaseCodeChunkWithoutResult
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2>, T1, T2> action);
    }

    public interface ICodeChunkWithSelfReference<T1, T2, T3> : IBaseCodeChunkWithoutResult
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action);
    }
}

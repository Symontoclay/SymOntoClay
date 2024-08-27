using SymOntoClay.ActiveObject.MethodResponses;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunksContext : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
        void CreateSyncCall(string chunkId, Func<ISyncMethodResponse> handler);
    }

    public interface ICodeChunksContext<T> : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action<T> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T>, T> action);
        void CreateSyncCall(string chunkId, Func<T, ISyncMethodResponse> handler);
    }

    public interface ICodeChunksContext<T1, T2> : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2>, T1, T2> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, ISyncMethodResponse> handler);
    }

    public interface ICodeChunksContext<T1, T2, T3> : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, T3, ISyncMethodResponse> handler);
    }
}

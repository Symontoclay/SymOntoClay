using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunksContextWithResult<TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);
    }

    public interface ICodeChunksContextWithResult<T, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> action);
    }

    public interface ICodeChunksContextWithResult<T1, T2, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> action);
    }

    public interface ICodeChunksContextWithResult<T1, T2, T3, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action);
    }
}

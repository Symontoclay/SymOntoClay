﻿using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunkWithResultAndSelfReference<TResult> : IBaseCodeChunkWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);
    }

    public interface ICodeChunkWithResultAndSelfReference<T, TResult> : IBaseCodeChunkWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> action);
    }

    public interface ICodeChunkWithResultAndSelfReference<T1, T2, TResult> : IBaseCodeChunkWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> action);
    }

    public interface ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult> : IBaseCodeChunkWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action);
    }
}
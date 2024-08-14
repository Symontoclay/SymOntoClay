﻿using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunkWithSelfReference : IBaseCodeChunk
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
    }

    public interface ICodeChunkWithSelfReference<T1, T2, T3> : IBaseCodeChunk
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action);
    }
}

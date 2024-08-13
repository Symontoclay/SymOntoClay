using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunkWithSelfReference : IBaseCodeChunk
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
    }
}

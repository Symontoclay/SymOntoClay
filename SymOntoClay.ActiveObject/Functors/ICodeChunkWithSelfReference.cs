using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public interface ICodeChunkWithSelfReference : IBaseCodeChunk
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
    }
}

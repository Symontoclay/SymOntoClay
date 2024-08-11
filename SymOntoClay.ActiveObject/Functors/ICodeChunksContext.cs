using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public interface ICodeChunksContext: IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
    }
}

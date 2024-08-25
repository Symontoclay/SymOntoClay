using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class CodeChunksContext : BaseCodeChunksContext, ICodeChunksContext
    {
        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            AddCodeChunk(new CodeChunk(chunkId, this, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action)
        {
            AddCodeChunk(new CodeChunkWithSelfReference(chunkId, this, action));
        }
    }

    public partial class CodeChunksContext<T1, T2, T3> : BaseCodeChunksContext, ICodeChunksContext<T1, T2, T3>
    {
        public CodeChunksContext(T1 arg1, T2 arg2, T3 arg3)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action)
        {
            AddCodeChunk(new CodeChunk<T1, T2, T3>(chunkId, this, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action)
        {
            AddCodeChunk(new CodeChunkWithSelfReference<T1, T2, T3>(chunkId, this, _arg1, _arg2, _arg3, action));
        }
    }
}

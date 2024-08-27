using SymOntoClay.ActiveObject.MethodResponses;
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

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<ISyncMethodResponse> handler)
        {
            throw new NotImplementedException("----CreateSyncCall|||||");
        }
    }

    public partial class CodeChunksContext<T> : BaseCodeChunksContext, ICodeChunksContext<T>
    {
        public CodeChunksContext(T arg1)
        {
            _arg1 = arg1;
        }

        private T _arg1;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T> action)
        {
            AddCodeChunk(new CodeChunk<T>(chunkId, this, _arg1, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T>, T> action)
        {
            AddCodeChunk(new CodeChunkWithSelfReference<T>(chunkId, this, _arg1, action));
        }

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<T, ISyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResulttForMethodWithoutResult<T>(chunkId, this, _arg1, handler));
        }
    }

    public partial class CodeChunksContext<T1, T2> : BaseCodeChunksContext, ICodeChunksContext<T1, T2>
    {
        public CodeChunksContext(T1 arg1, T2 arg2)
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        private T1 _arg1;
        private T2 _arg2;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2> action)
        {
            AddCodeChunk(new CodeChunk<T1, T2>(chunkId, this, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2>, T1, T2> action)
        {
            AddCodeChunk(new CodeChunkWithSelfReference<T1, T2>(chunkId, this, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<T1, T2, ISyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResulttForMethodWithoutResult<T1, T2>(chunkId, this, _arg1, _arg2, handler));
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

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<T1, T2, T3, ISyncMethodResponse> handler)
        {
            throw new NotImplementedException("----CreateSyncCall|||||");
        }
    }
}

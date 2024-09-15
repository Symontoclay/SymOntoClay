using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public void CreateSyncCall(string chunkId, Func<IDrivenSyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithoutResult(chunkId, this, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>(chunkId, this, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>(chunkId, this, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithoutResult(chunkId, this, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<IMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>(chunkId, this, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>(chunkId, this, preHandler, postHandler));
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
        public void CreateSyncCall(string chunkId, Func<T, IDrivenSyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithoutResult<T>(chunkId, this, _arg1, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResult<T, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>, T, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<T, IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T>(chunkId, this, _arg1, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResult<T, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>, T, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
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
        public void CreateSyncCall(string chunkId, Func<T1, T2, IDrivenSyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>(chunkId, this, _arg1, _arg2, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>, T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<T1, T2, IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>(chunkId, this, _arg1, _arg2, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>, T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
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
        public void CreateSyncCall(string chunkId, Func<T1, T2, T3, IDrivenSyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>(chunkId, this, _arg1, _arg2, _arg3, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2, T3, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>, T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<T1, T2, T3, IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>(chunkId, this, _arg1, _arg2, _arg3, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2, T3, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>, T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }
    }
}

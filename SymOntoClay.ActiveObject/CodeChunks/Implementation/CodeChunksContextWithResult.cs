using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class CodeChunksContextWithResult<TResult> : BaseCodeChunksContextWithResult<TResult>, ICodeChunksContextWithResult<TResult>
    {
        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            AddCodeChunk(new CodeChunkWithResult<TResult>(chunkId, this, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            AddCodeChunk(new CodeChunkWithResultAndSelfReference<TResult>(chunkId, this, action));
        }

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<ISyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithoutResult<TResult>(chunkId, this, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<ISyncMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResult<TResult, MethodResult>(chunkId, this, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>(chunkId, this, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithoutResult<TResult>(chunkId, this, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<IMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResult<TResult, MethodResult>(chunkId, this, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>(chunkId, this, preHandler, postHandler));
        }
    }

    public class CodeChunksContextWithResult<T, TResult> : BaseCodeChunksContextWithResult<TResult>, ICodeChunksContextWithResult<T, TResult>
    {
        public CodeChunksContextWithResult(T arg1)
        {
            _arg1 = arg1;
        }

        private T _arg1;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T> action)
        {
            AddCodeChunk(new CodeChunkWithResult<T, TResult>(chunkId, this, _arg1, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> action)
        {
            AddCodeChunk(new CodeChunkWithResultAndSelfReference<T, TResult>(chunkId, this, _arg1, action));
        }

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<T, ISyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithoutResult<T, TResult>(chunkId, this, _arg1, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T, ISyncMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResult<T, TResult, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>, T, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<T, IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithoutResult<T, TResult>(chunkId, this, _arg1, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResult<T, TResult, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>, T, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>(chunkId, this, _arg1, preHandler, postHandler));
        }
    }

    public class CodeChunksContextWithResult<T1, T2, TResult> : BaseCodeChunksContextWithResult<TResult>, ICodeChunksContextWithResult<T1, T2, TResult>
    {
        public CodeChunksContextWithResult(T1 arg1, T2 arg2)
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        private T1 _arg1;
        private T2 _arg2;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2> action)
        {
            AddCodeChunk(new CodeChunkWithResult<T1, T2, TResult>(chunkId, this, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> action)
        {
            AddCodeChunk(new CodeChunkWithResultAndSelfReference<T1, T2, TResult>(chunkId, this, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<T1, T2, ISyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, TResult>(chunkId, this, _arg1, _arg2, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, ISyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResult<T1, T2, TResult, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>, T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<T1, T2, IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, TResult>(chunkId, this, _arg1, _arg2, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResult<T1, T2, TResult, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>, T1, T2, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>(chunkId, this, _arg1, _arg2, preHandler, postHandler));
        }
    }

    public class CodeChunksContextWithResult<T1, T2, T3, TResult> : BaseCodeChunksContextWithResult<TResult>, ICodeChunksContextWithResult<T1, T2, T3, TResult>
    {
        public CodeChunksContextWithResult(T1 arg1, T2 arg2, T3 arg3)
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
            AddCodeChunk(new CodeChunkWithResult<T1, T2, T3, TResult>(chunkId, this, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action)
        {
            AddCodeChunk(new CodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>(chunkId, this, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        public void CreateSyncCall(string chunkId, Func<T1, T2, T3, ISyncMethodResponse> handler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, T3, TResult>(chunkId, this, _arg1, _arg2, _arg3, handler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResult<T1, T2, T3, TResult, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>, T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new SyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall(string chunkId, Func<T1, T2, T3, IMethodResponse> handler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, T3, TResult>(chunkId, this, _arg1, _arg2, _arg3, handler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResult<T1, T2, T3, TResult, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }

        /// <inheritdoc/>
        public void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>, T1, T2, T3, MethodResult> postHandler)
        {
            AddCodeChunk(new AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>(chunkId, this, _arg1, _arg2, _arg3, preHandler, postHandler));
        }
    }
}

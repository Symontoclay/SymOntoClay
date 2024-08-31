using SymOntoClay.ActiveObject.MethodResponses;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunksContextWithResult<TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);
        void CreateSyncCall(string chunkId, Func<ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<ISyncMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>, MethodResult> postHandler);
    }

    public interface ICodeChunksContextWithResult<T, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> action);
        void CreateSyncCall(string chunkId, Func<T, ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T, ISyncMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>, T, MethodResult> postHandler);
    }

    public interface ICodeChunksContextWithResult<T1, T2, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, ISyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>, T1, T2, MethodResult> postHandler);
    }

    public interface ICodeChunksContextWithResult<T1, T2, T3, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, T3, ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>, T1, T2, T3, MethodResult> postHandler);
    }
}

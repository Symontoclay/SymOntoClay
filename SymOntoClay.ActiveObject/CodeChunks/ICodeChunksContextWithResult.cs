using SymOntoClay.ActiveObject.MethodResponses;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunksContextWithResult<TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action);
        void CreateSyncCall(string chunkId, Func<IDrivenSyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>, MethodResult> postHandler);
        void CreateAsyncCall(string chunkId, Func<IAsyncMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<IAsyncMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>, MethodResult> postHandler);
    }

    public interface ICodeChunksContextWithResult<T, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> action);
        void CreateSyncCall(string chunkId, Func<T, IDrivenSyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>, T, MethodResult> postHandler);
        void CreateAsyncCall(string chunkId, Func<T, IAsyncMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IAsyncMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>, T, MethodResult> postHandler);
    }

    public interface ICodeChunksContextWithResult<T1, T2, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, IDrivenSyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>, T1, T2, MethodResult> postHandler);
        void CreateAsyncCall(string chunkId, Func<T1, T2, IAsyncMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IAsyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>, T1, T2, MethodResult> postHandler);
    }

    public interface ICodeChunksContextWithResult<T1, T2, T3, TResult> : IBaseCodeChunksContextWithResult<TResult>
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, T3, IDrivenSyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IDrivenSyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>, T1, T2, T3, MethodResult> postHandler);
        void CreateAsyncCall(string chunkId, Func<T1, T2, T3, IAsyncMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IAsyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>, T1, T2, T3, MethodResult> postHandler);
    }
}

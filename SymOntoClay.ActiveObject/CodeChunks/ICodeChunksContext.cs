using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.MethodResponses;
using System;
using System.Security.Cryptography;

namespace SymOntoClay.ActiveObject.CodeChunks
{
    public interface ICodeChunksContext : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
        void CreateSyncCall(string chunkId, Func<ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<ISyncMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>, MethodResult> postHandler);
        
        void CreateAsyncCall(string chunkId, Func<IMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<IMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>, MethodResult> postHandler);
    }

    public interface ICodeChunksContext<T> : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action<T> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T>, T> action);
        void CreateSyncCall(string chunkId, Func<T, ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T, ISyncMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>, T, MethodResult> postHandler);

        void CreateAsyncCall(string chunkId, Func<T, IMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>, T, MethodResult> postHandler);
    }

    public interface ICodeChunksContext<T1, T2> : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2>, T1, T2> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, ISyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>, T1, T2, MethodResult> postHandler);

        void CreateAsyncCall(string chunkId, Func<T1, T2, IMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>, T1, T2, MethodResult> postHandler);
    }

    public interface ICodeChunksContext<T1, T2, T3> : IBaseCodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action);
        void CreateSyncCall(string chunkId, Func<T1, T2, T3, ISyncMethodResponse> handler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler);
        void CreateSyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>, T1, T2, T3, MethodResult> postHandler);

        void CreateAsyncCall(string chunkId, Func<T1, T2, T3, IMethodResponse> handler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler);
        void CreateAsyncCall<MethodResult>(string chunkId, Func<T1, T2, T3, IMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>, T1, T2, T3, MethodResult> postHandler);
    }
}

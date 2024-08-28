using SymOntoClay.Serialization;
using System.Security.Cryptography;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class SyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResult
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResult(string id, ICodeChunksContext codeChunksContext)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;


    }

    public class SyncCallCodeChunkWithoutResultForMethodWithResult<T, MethodResult>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResult<T>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResult(string id, ICodeChunksContext<T> codeChunksContext)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;


    }

    public class SyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2, MethodResult>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResult(string id, ICodeChunksContext<T1, T2> codeChunksContext)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;


    }

    public class SyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2, T3, MethodResult>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResult<T1, T2, T3>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResult(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;


    }
}

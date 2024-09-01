using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class SyncCallCodeChunkWithoutResultForMethodWithoutResult: 
        BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult, 
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext codeChunksContext, Func<ISyncMethodResponse> handler)
        {
            _id = id;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private Func<ISyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override ISyncMethodResponse OnRunHandler()
        {
            return _handler();
        }
    }

    public partial class SyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult, 
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Func<T, ISyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T _arg1;

        private Func<T, ISyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override ISyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1);
        }
    }

    public partial class SyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2> 
        : BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult, 
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, ISyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T1 _arg1;
        private T2 _arg2;

        private Func<T1, T2, ISyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override ISyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2);
        }
    }

    public partial class SyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult,
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, ISyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        private Func<T1, T2, T3, ISyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override ISyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2, _arg3);
        }
    }
}

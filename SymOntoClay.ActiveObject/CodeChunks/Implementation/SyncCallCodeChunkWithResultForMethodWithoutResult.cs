using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class SyncCallCodeChunkWithResultForMethodWithoutResult<TResult> :
        BaseSyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        ISyncCallCodeChunkWithResultForMethodWithoutResult<TResult>
    {
        public SyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Func<IDrivenSyncMethodResponse> handler)
            : base(codeChunksContext)
        {
            _id = id;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private Func<IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler();
        }
    }

    public partial class SyncCallCodeChunkWithResultForMethodWithoutResult<T, TResult> :
        BaseSyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        ISyncCallCodeChunkWithResultForMethodWithoutResult<T, TResult>
    {
        public SyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<T, TResult> codeChunksContext, T arg1, Func<T, IDrivenSyncMethodResponse> handler)
            : base(codeChunksContext)
        {
            _id = id;
            _arg1 = arg1;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T _arg1;

        private Func<T, IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1);
        }
    }

    public partial class SyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, TResult> :
        BaseSyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        ISyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, TResult>
    {
        public SyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<T1, T2, TResult> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, IDrivenSyncMethodResponse> handler)
            : base(codeChunksContext)
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

        private Func<T1, T2, IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2);
        }
    }

    public partial class SyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, T3, TResult> :
        BaseSyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        ISyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, T3, TResult>
    {
        public SyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<T1, T2, T3, TResult> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, IDrivenSyncMethodResponse> handler)
            : base(codeChunksContext)
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

        private Func<T1, T2, T3, IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2, _arg3);
        }
    }
}

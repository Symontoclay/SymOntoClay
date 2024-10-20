﻿using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class SyncCallCodeChunkWithoutResultForMethodWithoutResult: 
        BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult, 
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext codeChunksContext, Func<IDrivenSyncMethodResponse> handler)
        {
            _id = id;
            _handler = handler;
        }

        private string _id;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler();
        }
    }

    public class SyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult, 
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Func<T, IDrivenSyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _handler = handler;
        }

        private string _id;

        private T _arg1;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<T, IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1);
        }
    }

    public class SyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2> 
        : BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult, 
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, IDrivenSyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _handler = handler;
        }

        private string _id;

        private T1 _arg1;
        private T2 _arg2;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<T1, T2, IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2);
        }
    }

    public class SyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
        : BaseSyncCallCodeChunkWithoutResultForMethodWithoutResult,
        ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, IDrivenSyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _handler = handler;
        }

        private string _id;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<T1, T2, T3, IDrivenSyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IDrivenSyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2, _arg3);
        }
    }
}

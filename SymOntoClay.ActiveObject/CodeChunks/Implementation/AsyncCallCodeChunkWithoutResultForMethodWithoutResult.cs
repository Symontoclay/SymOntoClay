﻿using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class AsyncCallCodeChunkWithoutResultForMethodWithoutResult :
        BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext codeChunksContext, Func<IAsyncMethodResponse> handler)
        {
            _id = id;
            _handler = handler;
        }

        private string _id;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<IAsyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IAsyncMethodResponse OnRunHandler()
        {
            return _handler();
        }
    }

    public class AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
        : BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Func<T, IAsyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _handler = handler;
        }

        private string _id;

        private T _arg1;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<T, IAsyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IAsyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1);
        }
    }

    public class AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>
        : BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, IAsyncMethodResponse> handler)
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
        private Func<T1, T2, IAsyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IAsyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2);
        }
    }

    public class AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
        : BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, IAsyncMethodResponse> handler)
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
        private Func<T1, T2, T3, IAsyncMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IAsyncMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2, _arg3);
        }
    }
}

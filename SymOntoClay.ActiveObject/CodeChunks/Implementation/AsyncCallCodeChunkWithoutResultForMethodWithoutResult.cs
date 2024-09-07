﻿using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class AsyncCallCodeChunkWithoutResultForMethodWithoutResult :
        BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext codeChunksContext, Func<IMethodResponse> handler)
        {
            _id = id;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private Func<IMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IMethodResponse OnRunHandler()
        {
            return _handler();
        }
    }

    public partial class AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
        : BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Func<T, IMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T _arg1;

        private Func<T, IMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IMethodResponse OnRunHandler()
        {
            return _handler(_arg1);
        }
    }

    public partial class AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>
        : BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, IMethodResponse> handler)
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

        private Func<T1, T2, IMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2);
        }
    }

    public partial class AsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
        : BaseAsyncCallCodeChunkWithoutResultForMethodWithoutResult,
        IAsyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2, T3>
    {
        public AsyncCallCodeChunkWithoutResultForMethodWithoutResult(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, IMethodResponse> handler)
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

        private Func<T1, T2, T3, IMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2, _arg3);
        }
    }
}
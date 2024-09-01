using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class AsyncCallCodeChunkWithResultForMethodWithoutResult<TResult> :
        BaseAsyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        IAsyncCallCodeChunkWithResultForMethodWithoutResult<TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Func<IMethodResponse> handler)
            : base(codeChunksContext)
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

    public partial class AsyncCallCodeChunkWithResultForMethodWithoutResult<T, TResult> :
        BaseAsyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        IAsyncCallCodeChunkWithResultForMethodWithoutResult<T, TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<T, TResult> codeChunksContext, T arg1, Func<T, IMethodResponse> handler)
            : base(codeChunksContext)
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

    public partial class AsyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, TResult> :
        BaseAsyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        IAsyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<T1, T2, TResult> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, IMethodResponse> handler)
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

        private Func<T1, T2, IMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2);
        }
    }

    public partial class AsyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, T3, TResult> :
        BaseAsyncCallCodeChunkWithResultForMethodWithoutResult<TResult>,
        IAsyncCallCodeChunkWithResultForMethodWithoutResult<T1, T2, T3, TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithoutResult(string id, ICodeChunksContextWithResult<T1, T2, T3, TResult> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, IMethodResponse> handler)
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

        private Func<T1, T2, T3, IMethodResponse> _handler;

        /// <inheritdoc/>
        protected override IMethodResponse OnRunHandler()
        {
            return _handler(_arg1, _arg2, _arg3);
        }
    }
}

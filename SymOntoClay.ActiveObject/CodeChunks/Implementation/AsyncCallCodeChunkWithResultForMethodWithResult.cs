using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class AsyncCallCodeChunkWithResultForMethodWithResult<TResult, MethodResult>
        : BaseAsyncCallCodeChunkWithResultForMethodWithResult<TResult, MethodResult>,
        IAsyncCallCodeChunkWithResultForMethodWithResult<TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResult(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Func<IMethodResponse<MethodResult>> preHandler, Action<MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        [SocSerializableActionKey]
        private string _id;

        private Func<IMethodResponse<MethodResult>> _preHandler;
        private Action<MethodResult> _postHandler;

        protected override IMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler();
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(methodResult);
        }
    }

    public partial class AsyncCallCodeChunkWithResultForMethodWithResult<T, TResult, MethodResult>
        : BaseAsyncCallCodeChunkWithResultForMethodWithResult<TResult, MethodResult>,
        IAsyncCallCodeChunkWithResultForMethodWithResult<T, TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResult(string id, ICodeChunksContextWithResult<T, TResult> codeChunksContext, T arg1, Func<T, IMethodResponse<MethodResult>> preHandler, Action<T, MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _arg1 = arg1;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T _arg1;

        private Func<T, IMethodResponse<MethodResult>> _preHandler;
        private Action<T, MethodResult> _postHandler;

        protected override IMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(_arg1, methodResult);
        }
    }

    public partial class AsyncCallCodeChunkWithResultForMethodWithResult<T1, T2, TResult, MethodResult>
        : BaseAsyncCallCodeChunkWithResultForMethodWithResult<TResult, MethodResult>,
        IAsyncCallCodeChunkWithResultForMethodWithResult<T1, T2, TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResult(string id, ICodeChunksContextWithResult<T1, T2, TResult> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T1 _arg1;
        private T2 _arg2;

        private Func<T1, T2, IMethodResponse<MethodResult>> _preHandler;
        private Action<T1, T2, MethodResult> _postHandler;

        protected override IMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1, _arg2);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(_arg1, _arg2, methodResult);
        }
    }

    public partial class AsyncCallCodeChunkWithResultForMethodWithResult<T1, T2, T3, TResult, MethodResult>
        : BaseAsyncCallCodeChunkWithResultForMethodWithResult<TResult, MethodResult>,
        IAsyncCallCodeChunkWithResultForMethodWithResult<T1, T2, T3, TResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResult(string id, ICodeChunksContextWithResult<T1, T2, T3, TResult> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, IMethodResponse<MethodResult>> preHandler, Action<T1, T2, T3, MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        private Func<T1, T2, T3, IMethodResponse<MethodResult>> _preHandler;
        private Action<T1, T2, T3, MethodResult> _postHandler;

        protected override IMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1, _arg2, _arg3);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(_arg1, _arg2, _arg3, methodResult);
        }
    }
}

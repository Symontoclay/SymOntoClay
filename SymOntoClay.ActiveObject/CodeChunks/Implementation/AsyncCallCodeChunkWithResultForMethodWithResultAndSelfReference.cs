using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult> :
            BaseAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>,
            IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Func<IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>, MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<IAsyncMethodResponse<MethodResult>> _preHandler;

        [SocSerializableActionMember(nameof(_id), 1)]
        private Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>, MethodResult> _postHandler;

        private string _id;

        protected override IAsyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler();
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, methodResult);
        }
    }

    public class AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult> :
        BaseAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>,
        IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference(string id, ICodeChunksContextWithResult<T, TResult> codeChunksContext, T arg1, Func<T, IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>, T, MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _arg1 = arg1;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        private string _id;

        private T _arg1;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<T, IAsyncMethodResponse<MethodResult>> _preHandler;

        [SocSerializableActionMember(nameof(_id), 1)]
        private Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T, TResult, MethodResult>, T, MethodResult> _postHandler;

        protected override IAsyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, _arg1, methodResult);
        }
    }

    public class AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult> :
        BaseAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>,
        IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference(string id, ICodeChunksContextWithResult<T1, T2, TResult> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>, T1, T2, MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        private string _id;

        private T1 _arg1;
        private T2 _arg2;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<T1, T2, IAsyncMethodResponse<MethodResult>> _preHandler;

        [SocSerializableActionMember(nameof(_id), 1)]
        private Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, TResult, MethodResult>, T1, T2, MethodResult> _postHandler;

        protected override IAsyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1, _arg2);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, _arg1, _arg2, methodResult);
        }
    }

    public class AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult> :
        BaseAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>,
        IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>
    {
        public AsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference(string id, ICodeChunksContextWithResult<T1, T2, T3, TResult> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, IAsyncMethodResponse<MethodResult>> preHandler, Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>, T1, T2, T3, MethodResult> postHandler)
            : base(codeChunksContext)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }

        private string _id;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        [SocSerializableActionMember(nameof(_id), 0)]
        private Func<T1, T2, T3, IAsyncMethodResponse<MethodResult>> _preHandler;

        [SocSerializableActionMember(nameof(_id), 1)]
        private Action<IAsyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<T1, T2, T3, TResult, MethodResult>, T1, T2, T3, MethodResult> _postHandler;

        protected override IAsyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1, _arg2, _arg3);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, _arg1, _arg2, _arg3, methodResult);
        }
    }
}

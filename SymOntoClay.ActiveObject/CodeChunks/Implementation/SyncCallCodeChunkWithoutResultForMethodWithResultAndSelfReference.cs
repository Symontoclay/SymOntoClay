using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>:
        BaseSyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference(string id, ICodeChunksContext codeChunksContext, Func<ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>, MethodResult> postHandler)
             : base(codeChunksContext)
        {
            _id = id;
            _preHandler = preHandler;
            _postHandler = postHandler;
        }
        
        [SocSerializableActionKey]
        private string _id;

        private Func<ISyncMethodResponse<MethodResult>> _preHandler;
        private Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>, MethodResult> _postHandler;

        protected override ISyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler();
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, methodResult);
        }
    }

    public partial class SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult> :
        BaseSyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Func<T, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>, T, MethodResult> postHandler)
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

        private Func<T, ISyncMethodResponse<MethodResult>> _preHandler; 
        private Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T, MethodResult>, T, MethodResult> _postHandler;

        protected override ISyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, _arg1, methodResult);
        }
    }

    public partial class SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult> :
        BaseSyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>, T1, T2, MethodResult> postHandler)
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

        private Func<T1, T2, ISyncMethodResponse<MethodResult>> _preHandler;
        private Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, MethodResult>, T1, T2, MethodResult> _postHandler;

        protected override ISyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1, _arg2);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, _arg1, _arg2, methodResult);
        }
    }

    public partial class SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult> :
        BaseSyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>,
        ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>
    {
        public SyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> preHandler, Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>, T1, T2, T3, MethodResult> postHandler)
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

        private Func<T1, T2, T3, ISyncMethodResponse<MethodResult>> _preHandler;
        private Action<ISyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<T1, T2, T3, MethodResult>, T1, T2, T3, MethodResult> _postHandler;

        protected override ISyncMethodResponse<MethodResult> OnRunPreHandler()
        {
            return _preHandler(_arg1, _arg2, _arg3);
        }

        protected override void OnRunPostHandler(MethodResult methodResult)
        {
            _postHandler(this, _arg1, _arg2, _arg3, methodResult);
        }
    }
}
